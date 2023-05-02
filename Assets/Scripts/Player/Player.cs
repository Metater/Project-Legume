using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private List<PlayerComponent> components;
    [SerializeField] private List<GameObject> invisibleToSelf;

    private GameManager manager;

    private readonly Dictionary<Type, PlayerComponent> cachedComponents = new();

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>(true);

        components.ForEach(c => c.Init(manager, this));
    }
    private void Start() => components.ForEach(c => c.PlayerStart());
    private void Update() => components.ForEach(c => c.PlayerUpdate());
    private void LateUpdate() => components.ForEach(c => c.PlayerLateUpdate());

    public override void OnStartLocalPlayer()
    {
        // Hide certain visuals to self
        invisibleToSelf.ForEach(go => go.SetActive(false));

        // Maintain local player reference
        manager.Get<PlayerManager>().SetLocalPlayer(this);
    }
    public override void OnStartClient()
    {
        // Maintain player lookup
        manager.Get<PlayerManager>().Players.Add(this);
    }
    public override void OnStartServer()
    {
        // Maintain player lookup
        manager.Get<PlayerManager>().Players.Add(this);
    }
    public override void OnStopLocalPlayer()
    {
        // Maintain local player reference
        manager.Get<PlayerManager>().SetLocalPlayer(null);
    }
    public override void OnStopClient()
    {
        // Maintain player lookup
        manager.Get<PlayerManager>().Players.Remove(this);
    }
    public override void OnStopServer()
    {
        // Maintain player lookup
        manager.Get<PlayerManager>().Players.Remove(this);
    }

    public T Get<T>() where T : PlayerComponent
    {
        Type desiredType = typeof(T);

        if (cachedComponents.TryGetValue(desiredType, out var component))
        {
            return (T)component;
        }

        foreach (var c in components)
        {
            if (c.GetType() == desiredType)
            {
                cachedComponents[desiredType] = c;
                return (T)c;
            }
        }

        Debug.LogError($"Player does not have component {desiredType}");

        return null;
    }
}
