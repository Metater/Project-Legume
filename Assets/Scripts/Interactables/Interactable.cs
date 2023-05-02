using Mirror;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    [SerializeField] private Color crosshairHoverColor = new(0.1921569f, 0.6901961f, 0.2029286f, 0.4980392f);
    [SerializeField] private Color crosshairInteractingColor = new(0.5602263f, 0.1921569f, 0.6901961f, 0.4980392f);
    [SerializeField] private float maxInteractionDistance = 4f;
    protected GameManager manager;
    public Color CrosshairHoverColor => crosshairHoverColor;
    public Color CrosshairInteractingColor => crosshairInteractingColor;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>(true);

        InteractableAwake();
    }
    private void Start() => InteractableStart();
    private void Update() => InteractableUpdate();
    private void LateUpdate() => InteractableLateUpdate();

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
