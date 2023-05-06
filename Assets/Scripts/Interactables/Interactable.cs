using Mirror;
using UnityEditor;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    [SerializeField] private float maxInteractionDistance = 4f;
    protected GameManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>(true);

        InteractableAwake();
    }
    private void Start() => InteractableStart();
    private void Update() => InteractableUpdate();
    private void LateUpdate() => InteractableLateUpdate();
    private void OnValidate()
    {
        // Ensure all items and children of items have the "Interactable" layer set
        EditorApplication.delayCall += () =>
        {
            if (Application.isPlaying || !gameObject.activeInHierarchy)
            {
                return;
            }

            int layer = LayerMask.NameToLayer("Interactable");
            gameObject.layer = layer;
            var children = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                child.gameObject.layer = layer;
            }
        };
    }

    protected virtual void InteractableAwake() { }
    protected virtual void InteractableStart() { }
    protected virtual void InteractableUpdate() { }
    protected virtual void InteractableLateUpdate() { }

    protected bool IsInteractionPointWithinBounds(Vector3 interactionPoint)
    {
        float currentInteractionDistance = Vector3.Distance(transform.position, interactionPoint);
        return currentInteractionDistance < maxInteractionDistance;
    }

    public abstract void ServerLeftMouseButtonDown(Player player, float interactionDistance, Vector3 interactionPointOffset);
    public abstract void ServerLeftMouseButtonUp(Player player);

    protected static Vector3 GetPlayerInteractionPoint(Player player, float interactionDistance, Vector3 interactionPointOffset)
    {
        Transform handsTransform = player.Get<PlayerMovement>().HandsTransform;
        Vector3 interactionPoint = handsTransform.position + (handsTransform.forward * interactionDistance) - interactionPointOffset;
        return interactionPoint;
    }
}
