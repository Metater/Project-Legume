using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // [SerializeField] private Color item color = new(0.2039216f, 0.5647059f, 0.8117647f, 0.4980392f);
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

    // Events:
    // public events

    // Methods:
    // Unity events or wrapped Unity events, ie: Start, ManagerStart
    // Mirror callbacks, ie: OnStartClient
    // private methods
    // protected virtual methods
    // protected abstract methods
    // protected methods
    // protected virtual methods
    // public abstract methods
    // public methods
    // public Mirror Commands
    // public Mirror Rpcs
    // public Mirror TargetRpcs
    // public Server methods
    // public, protected, and private static methods


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
