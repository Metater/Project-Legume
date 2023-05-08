using Mirror;
using UnityEditor;
using UnityEngine;

public abstract class Item : NetworkBehaviour
{
    private const float PickupSyncSeconds = 0.5f;
    [SerializeField] private float dropForceMultiplier;
    [SerializeField] private GameObject modelGameObject;
    private OwnedRigidbody ownedRigidbody;
    private NetworkTransform networkTransform;
    protected GameManager manager;
    [SyncVar(hook = nameof(OnHolderChanged))] private NetworkIdentity holderNetIdentitySynced = null;
    [SyncVar(hook = nameof(OnIsVisibleChanged))] private bool isVisible = true;
    public bool IsHeld => holderNetIdentitySynced != null;

    private void Awake()
    {
        ownedRigidbody = GetComponent<OwnedRigidbody>();
        networkTransform = GetComponent<NetworkTransform>();
        manager = FindObjectOfType<GameManager>(true);

        ItemAwake();
    }
    private void Start() => ItemStart();
    private void Update()
    {
        ItemUpdate();
    }
    private void LateUpdate() => ItemLateUpdate();
    private void OnValidate()
    {
        // Ensure all items and children of items have the "Item" layer set
        EditorApplication.delayCall += () =>
        {
            try
            {
                if (Application.isPlaying || !gameObject.activeInHierarchy)
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            int layer = LayerMask.NameToLayer("Item");
            gameObject.layer = layer;
            var children = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                child.gameObject.layer = layer;
            }
        };
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            return;
        }

        // Maintain item lookup
        manager.Get<ItemManager>().Items.Add(this);

        ownedRigidbody.EnableColliders();
        ownedRigidbody.Disable();
    }
    public override void OnStartServer()
    {
        // Maintain item lookup
        manager.Get<ItemManager>().Items.Add(this);

        netIdentity.AssignClientAuthority(NetworkServer.localConnection);

        ownedRigidbody.EnableColliders();
        ownedRigidbody.Enable();
    }
    public override void OnStopClient()
    {
        // Maintain item lookup
        manager.Get<ItemManager>().Items.Remove(this);
    }
    public override void OnStopServer()
    {
        // Maintain item lookup
        manager.Get<ItemManager>().Items.Remove(this);
    }

    private void OnHolderChanged(NetworkIdentity oldHolder, NetworkIdentity newHolder)
    {
        if (newHolder != null)
        {
            // Pickup
            ownedRigidbody.DisableColliders();
            ownedRigidbody.Disable();

            foreach (var item in manager.Get<ItemManager>().Items.Refs)
            {
                if (item.ownedRigidbody.Rigidbody != null)
                {
                    item.ownedRigidbody.Rigidbody.WakeUp();
                }
            }
        }
        else
        {
            // Drop
            if (manager.Get<PlayerManager>().LocalPlayer.netId == oldHolder.netId)
            {
                ownedRigidbody.EnableColliders();
                ownedRigidbody.Enable();
            }
            else
            {
                ownedRigidbody.EnableColliders();
            }
        }
    }
    private void OnIsVisibleChanged(bool _,  bool newIsVisible)
    {
        modelGameObject.SetActive(newIsVisible);
    }

    protected virtual void ItemAwake() { }
    protected virtual void ItemStart() { }
    protected virtual void ItemUpdate() { }
    protected virtual void ItemLateUpdate() { }
    protected abstract bool ServerItemPickup(Player player);
    protected abstract bool ServerItemDrop(Player player);

    [Server]
    public bool ServerPickup(Player player)
    {
        if (ServerItemPickup(player))
        {
            holderNetIdentitySynced = player.netIdentity;
        }

        return IsHeld;
    }
    [Server]
    public bool ServerDrop(Player player)
    {
        if (ServerItemDrop(player))
        {
            holderNetIdentitySynced = null;
        }

        return !IsHeld;
    }
    [Server]
    public void ServerUpdateVisibility(bool isVisible)
    {
        this.isVisible = isVisible;
    }

    [Client]
    public void ClientAddDropForce(Vector3 vector, Vector3 velocity)
    {
        ownedRigidbody.Rigidbody.AddForce((vector * dropForceMultiplier) + velocity, ForceMode.Impulse);
    }
}
