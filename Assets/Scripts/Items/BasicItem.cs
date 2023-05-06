using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicItem : Item
{
    [Server]
    protected override bool ServerItemPickup(Player player)
    {
        return true;
    }
    [Server]
    protected override bool ServerItemDrop(Player player)
    {
        return true;
    }
}
