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
        player.Get<PlayerInteraction>().TargetAcceptInteraction(netIdentity, InteractionType.EKey);
        TargetOpenUi(player.connectionToClient);
    }
    public override void ServerCancelInteraction(Player player)
    {

    }

    [TargetRpc]
    private void TargetOpenUi(NetworkConnectionToClient _)
    {
        panel.Open();
    }
}
