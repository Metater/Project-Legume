using Mirror;
using UnityEngine;

public class PlayerInteraction : PlayerComponent
{
    [SerializeField] private Color crosshairHoverColor = new(0.1921569f, 0.6901961f, 0.2029286f, 0.4980392f);
    [SerializeField] private Color crosshairInteractingColor = new(0.5602263f, 0.1921569f, 0.6901961f, 0.4980392f);
    [SerializeField] private float reachDistance;
    private Interactable targetInteractable = null;
    private InteractionType targetInteractionType = InteractionType.Click;

    public override void PlayerUpdate()
    {
        if (!isLocalPlayer)
        {
            targetInteractable = null;

            return;
        }

        if (!manager.Get<PhaseManager>().ShouldPlayerMove)
        {
            if (targetInteractable != null)
            {
                CmdCancelInteraction(targetInteractable.netIdentity);
                targetInteractable = null;
            }

            return;
        }

        // TODO Fix
        if (targetInteractable != null)
        {
            switch (targetInteractionType)
            {
                case InteractionType.Click:
                    if (!Input.GetMouseButton(0))
                    {
                        CmdCancelInteraction(targetInteractable.netIdentity);
                        targetInteractable = null;
                    }
                    break;
                case InteractionType.EKey:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        CmdCancelInteraction(targetInteractable.netIdentity);
                        targetInteractable = null;
                    }
                    break;
            }
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (targetInteractable == null && Physics.Raycast(ray, out var hit, reachDistance))
        {
            if (hit.transform.TryGetComponent<InteractableGameObject>(out var targetGameObject))
            {
                NetworkIdentity interactableNetIdentity = targetGameObject.Interactable.netIdentity;
                float interactionDistance = hit.distance + 0.3f; // Why the 0.3f? Absolutely no clue!!!
                Vector3 interactionPointOffset = hit.point - targetGameObject.transform.position;

                if (Input.GetMouseButtonDown(0))
                {
                    CmdLeftMouseButtonDown(interactableNetIdentity, interactionDistance, interactionPointOffset);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    CmdEKeyDown(interactableNetIdentity);
                }

                manager.Get<CrosshairManager>().SetColor(crosshairHoverColor);
            }
        }

        if (targetInteractable != null)
        {
            manager.Get<CrosshairManager>().SetColor(crosshairInteractingColor);
        }
    }

    [Client]
    public bool ClientTryUseEscapeKeyDown()
    {
        if (targetInteractable != null && targetInteractionType == InteractionType.EKey)
        {
            CmdCancelInteraction(targetInteractable.netIdentity);
            targetInteractable = null;
            return true;
        }

        return false;
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
    private void CmdEKeyDown(NetworkIdentity interactableNetIdentity)
    {
        if (!interactableNetIdentity.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        interactable.ServerEKeyDown(player);
    }
    [Command]
    private void CmdCancelInteraction(NetworkIdentity interactableNetIdentity)
    {
        if (!interactableNetIdentity.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        interactable.ServerCancelInteraction(player);
    }

    [TargetRpc]
    public void RpcAcceptInteraction(NetworkIdentity interactableNetIdentity, InteractionType interactionType)
    {
        if (!interactableNetIdentity.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        targetInteractable = interactable;
        targetInteractionType = interactionType;
    }
    [TargetRpc]
    public void RpcCancelInteraction(NetworkIdentity interactableNetIdentity)
    {
        if (!interactableNetIdentity.TryGetComponent(out Interactable interactable))
        {
            return;
        }

        if (interactable == targetInteractable)
        {
            targetInteractable = null;
        }
    }
}
