using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Item : NetworkBehaviour
{
    [SerializeField] protected float dropForceMultiplier;
    [SerializeField] protected GameObject modelGameObject;
    protected GameManager manager;
    protected OwnedRigidbody ownedRigidbody;
    [SyncVar] private NetworkIdentity holderNetIdentitySynced = null;
    public bool IsHeld => holderNetIdentitySynced != null;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>(true);
        ownedRigidbody = GetComponent<OwnedRigidbody>();
    }
    private void OnValidate()
    {
        // Ensure all items and children of items have the "Item" layer set
        EditorApplication.delayCall += () =>
        {
            if (Application.isPlaying || !gameObject.activeInHierarchy)
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
        ownedRigidbody.EnableColliders();
    }
    public override void OnStartServer()
    {
        ownedRigidbody.EnableColliders();
        ownedRigidbody.Enable();
    }
    public override void OnStartAuthority()
    {
        ownedRigidbody.Enable();
    }
    public override void OnStopAuthority()
    {
        ownedRigidbody.Disable();
    }

    protected abstract void ItemPickup();

    [Server]
    public bool ServerPickup(Player player)
    {
        holderNetIdentitySynced = player.netIdentity;
        ownedRigidbody.DisableColliders();
        ownedRigidbody.Disable();
        ItemPickup();

        return true;
    }
    [Client]
    public void ClientPickup()
    {

    }
    [Server]
    public bool ServerDrop(Player player, Vector3 vector, Vector3 velocity)
    {
        holderNetIdentitySynced = null;
        ownedRigidbody.EnableColliders();
        ownedRigidbody.Enable();

        return true;
    }
}
