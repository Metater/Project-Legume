using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : Interactable
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float forceProportionalConstant;
    [SerializeField] private float forceDerivitiveConstant;
    private (Player player, float interactionDistance, Vector3 interactionPointOffset)? interactor = null;
    private float lastError = float.NaN;

    protected override void InteractableUpdate()
    {
        if (!isServer)
        {
            return;
        }

        rb.useGravity = interactor == null;

        if (interactor != null)
        {
            (Player player, float interactionDistance, Vector3 interactionPointOffset) = interactor.Value;

            if (player == null)
            {
                interactor = null;
                lastError = float.NaN;
            }
            else
            {
                Vector3 interactionPoint = GetPlayerInteractionPoint(player, interactionDistance, interactionPointOffset);
                if (!IsInteractionPointWithinBounds(interactionPoint))
                {
                    interactor = null;
                    lastError = float.NaN;

                    player.Get<PlayerInteraction>().RpcCancelInteraction(netIdentity);
                }
                else
                {
                    Vector3 vector = (interactionPoint - transform.position).normalized;
                    float error = Vector3.Distance(transform.position, interactionPoint);
                    float force = error * forceProportionalConstant;
                    if (!float.IsNaN(lastError))
                    {
                        force += ((error - lastError) / Time.deltaTime) * forceDerivitiveConstant;
                    }
                    lastError = error;
                    rb.AddForce(Time.deltaTime * force * vector, ForceMode.VelocityChange);
                }
            }
        }
    }

    [Server]
    public override void ServerLeftMouseButtonDown(Player player, float interactionDistance, Vector3 interactionPointOffset)
    {
        if (interactor == null)
        {
            interactor = (player, interactionDistance, interactionPointOffset);
            player.Get<PlayerInteraction>().RpcAcceptInteraction(netIdentity, InteractionType.Click);
        }
    }
    [Server]
    public override void ServerCancelInteraction(Player player)
    {
        if (interactor != null && player == interactor.Value.player)
        {
            interactor = null;
            lastError = float.NaN;
        }
    }
}
