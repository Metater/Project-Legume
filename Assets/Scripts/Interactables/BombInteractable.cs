using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombInteractable : Interactable
{
    private BombDefusalUiPanel panel;

    protected override void InteractableAwake()
    {
        panel = FindObjectOfType<BombDefusalUiPanel>(true);
    }

    [Server]
    public override void ServerEKeyDown(Player player)
    {
        player.Get<PlayerInteraction>().RpcAcceptInteraction(netIdentity, InteractionType.EKey);
        RpcOpenUi(player.connectionToClient);
    }
    public override void ServerCancelInteraction(Player player)
    {

    }

    [TargetRpc]
    private void RpcOpenUi(NetworkConnectionToClient _)
    {
        panel.Open();
    }
}
