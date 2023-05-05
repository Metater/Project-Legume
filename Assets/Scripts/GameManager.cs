using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Reminders:
    // Group any related things in your code
    // Don't prematurely optimise, be dumb

    [SerializeField] private List<Manager> managers;
    private readonly Dictionary<Type, Manager> cachedManagers = new();

    private void Awake() => managers.ForEach(c => c.Init(this));
    private void Start() => managers.ForEach(c => c.ManagerStart());
    private void Update() => managers.ForEach(c => c.ManagerUpdate());
    private void LateUpdate() => managers.ForEach(c => c.ManagerLateUpdate());

    public T Get<T>() where T : Manager
    {
        Type desiredType = typeof(T);

        if (cachedManagers.TryGetValue(desiredType, out var manager))
        {
            return (T)manager;
        }

        foreach (var m in managers)
        {
            if (m.GetType() == desiredType)
            {
                cachedManagers[desiredType] = m;
                return (T)m;
            }
        }

        Debug.LogError($"GameManager does not have manager {desiredType}");

        return null;
    }
}
