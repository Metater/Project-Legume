using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] private float angleSmoothTime;
    private (Player player, float interactionDistance, Vector3 localInteractionPoint)? interactor = null;
    private float angleVelocity = 0;

    protected override void InteractableUpdate()
    {
        if (!isServer)
        {
            return;
        }

        if (interactor != null)
        {
            (Player player, float interactionDistance, Vector3 localInteractionPoint) = interactor.Value;
            if (player == null)
            {
                interactor = null;
            }
            else
            {
                Vector3 interactionPoint = GetPlayerInteractionPoint(player, interactionDistance, localInteractionPoint);
                if (!IsInteractionPointWithinRadius(interactionPoint))
                {
                    interactor = null;

                    player.Get<PlayerInteraction>().RpcCancelInteraction(netIdentity);
                }
                else
                {
                    Vector3 vector = transform.position - interactionPoint;
                    float angle = Vector2.SignedAngle(Vector2.up, new Vector2(vector.x, -vector.z)) - 90f;
                    angle = Mathf.SmoothDampAngle(transform.localEulerAngles.y, angle, ref angleVelocity, angleSmoothTime);
                    transform.localEulerAngles = new Vector3(0, angle, 0);
                }
            }
        }
    }

    [Server]
    public override void ServerLeftMouseButtonDown(Player player, float interactionDistance, Vector3 localInteractionPoint)
    {
        interactor ??= (player, interactionDistance, localInteractionPoint);
    }

    [Server]
    public override void ServerLeftMouseButtonUp(Player player)
    {
        if (interactor != null && player == interactor.Value.player)
        {
            interactor = null;
        }
    }
}
