using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRefs<T> where T : NetworkBehaviour
{
    private readonly Dictionary<uint, T> refs = new();
    public IEnumerable<T> Refs => refs.Values;

    public void Add(T reference)
    {
        refs[reference.netId] = reference;
    }
    public void Remove(T reference)
    {
        refs.Remove(reference.netId);
    }

    public bool TryGetWithNetId(uint netId, out T reference)
    {
        return refs.TryGetValue(netId, out reference);
    }
}
