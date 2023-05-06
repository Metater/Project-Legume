using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Manager
{
    public NetRefs<Item> Items { get; private set; } = new();

    public event Action<Item> OnStartItem;
    public event Action<Item> OnStopItem;

    public void StartItem(Item item)
    {
        OnStartItem?.Invoke(item);
        Items.Add(item);
    }
    public void StopItem(Item item)
    {
        OnStopItem?.Invoke(item);
        Items.Remove(item);
    }
}
