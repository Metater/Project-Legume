using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : PlayerComponent
{
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

                    NetworkIdentity interactable = targetGameObject.Interactable.netIdentity;
                    float interactionDistance = hit.distance + 0.3f; // Why the 0.3f? Absolutely no clue!!!
                    Vector3 interactionPointOffset = hit.point - targetGameObject.transform.position;
                    CmdLeftMouseButtonDown(interactable, interactionDistance, interactionPointOffset);
                }

                manager.Get<CrosshairManager>().SetColor(targetGameObject.Interactable.CrosshairHoverColor);
            }
        }

        if (targetGameObject != null)
        {
            manager.Get<CrosshairManager>().SetColor(targetGameObject.Interactable.CrosshairInteractingColor);
        }
    }

    [Command]
    public void CmdLeftMouseButtonDown(NetworkIdentity interactable, float interactionDistance, Vector3 interactionPointOffset)
    {
        interactable.GetComponent<Interactable>().ServerLeftMouseButtonDown(player, interactionDistance, interactionPointOffset);
    }
    [Command]
    public void CmdLeftMouseButtonUp(NetworkIdentity interactable)
    {
        interactable.GetComponent<Interactable>().ServerLeftMouseButtonUp(player);
    }

    [TargetRpc]
    public void RpcCancelInteraction(NetworkIdentity interactable)
    {
        if (interactable.GetComponent<Interactable>() == targetGameObject.Interactable)
        {
            targetGameObject = null;
        }
    }
}
