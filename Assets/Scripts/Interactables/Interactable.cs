using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    protected GameManager manager;
    [SerializeField] private Color crosshairHoverColor = new(0.1921569f, 0.6901961f, 0.2029286f, 0.4980392f);
    [SerializeField] private Color crosshairInteractingColor = new(0.5602263f, 0.1921569f, 0.6901961f, 0.4980392f);
    [SerializeField] private float maxInteractionDistance = 4f;

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

    public abstract void ServerLeftMouseButtonDown(Player player, float interactionDistance, Vector3 localInteractionPoint);
    public abstract void ServerLeftMouseButtonUp(Player player);

    protected static Vector3 GetPlayerInteractionPoint(Player player, float interactionDistance, Vector3 localInteractionPoint)
    {
        Transform handsTransform = player.Get<PlayerMovement>().HandsTransform;
        Vector3 interactionPoint = handsTransform.position + (handsTransform.forward * interactionDistance) - localInteractionPoint;
        return interactionPoint;
    }
    protected bool IsInteractionPointWithinRadius(Vector3 interactionPoint)
    {
        float currentInteractionDistance = Vector3.Distance(transform.position, interactionPoint);
        return currentInteractionDistance < maxInteractionDistance;
    }

    public Color GetCrosshairHoverColor() => crosshairHoverColor;
    public Color GetCrosshairInteractingColor() => crosshairInteractingColor;
}
