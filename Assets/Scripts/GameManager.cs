using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO use properties to expose unity refernces

    // Reminders:
    // Group any related things in your code
    // Don't prematurely optimise, be dumb

    // Standard:

    // Fields:
    // private constants
    // [SerializeField] private and protected Unity references
    // Awake-initialized private and protected references, ie: GameManager
    // private readonly and protected readonly variables
    // private and protected variables

    // Properties:
    // public properties

    // Methods:
    // Unity events or wrapped Unity events, ie: Start, ManagerStart
    // Mirror callbacks, ie: OnStartClient
    // private methods
    // public methods


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
