using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : PlayerComponent
{
    [SerializeField] private Color crosshairHoverColor = new(0.1921569f, 0.6901961f, 0.2029286f, 0.4980392f);
    [SerializeField] private Color crosshairInteractingColor = new(0.5602263f, 0.1921569f, 0.6901961f, 0.4980392f);
    [SerializeField] private float reachDistance;
    private InteractableGameObject targetGameObject = null;

    public override void PlayerUpdate()
    {
        if (targetGameObject != null && Input.GetMouseButtonUp(0))
        {
            CmdLeftMouseButtonUp(targetGameObject.Interactable.netIdentity);

            targetGameObject = null;
        }

        if (!isLocalPlayer || !manager.Get<PhaseManager>().HasStarted || manager.Get<CursorManager>().IsCursorVisable)
        {
            targetGameObject = null;

            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (targetGameObject == null && Physics.Raycast(ray, out var hit, reachDistance))
        {
            if (hit.transform.TryGetComponent<InteractableGameObject>(out var targetGameObject))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    this.targetGameObject = targetGameObject;

                    NetworkIdentity interactableNetIdentity = targetGameObject.Interactable.netIdentity;
                    float interactionDistance = hit.distance + 0.3f; // Why the 0.3f? Absolutely no clue!!!
                    Vector3 interactionPointOffset = hit.point - targetGameObject.transform.position;
                    CmdLeftMouseButtonDown(interactableNetIdentity, interactionDistance, interactionPointOffset);
                }

                manager.Get<CrosshairManager>().SetColor(crosshairHoverColor);
            }
        }

        if (targetGameObject != null)
        {
            manager.Get<CrosshairManager>().SetColor(crosshairInteractingColor);
        }
    }

    [Command]
    private void CmdLeftMouseButtonDown(NetworkIdentity interactableNetIdentity, float interactionDistance, Vector3 interactionPointOffset)
    {
        if (!interactableNetIdentity.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        interactable.ServerLeftMouseButtonDown(player, interactionDistance, interactionPointOffset);
    }
    [Command]
    private void CmdLeftMouseButtonUp(NetworkIdentity interactableNetIdentity)
    {
        if (!interactableNetIdentity.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        interactable.ServerLeftMouseButtonUp(player);
    }

    [TargetRpc]
    public void RpcCancelInteraction(NetworkIdentity interactableNetIdentity)
    {
        if (!interactableNetIdentity.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        if (interactable == targetGameObject.Interactable)
        {
            targetGameObject = null;
        }
    }
}
