using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BombDefusalCursors : NetworkBehaviour
{
    [SerializeField] private RectTransform cursorPrefab;
    [SerializeField] private Transform cursorsTransform;
    [SerializeField] private float cursorSmoothTime;
    [SerializeField] private double cursorTimeoutSeconds;
    private readonly Dictionary<uint, BombDefusalCursor> cursors = new();

    private void Update()
    {
        // Find old cursors, update current cursors
        List<uint> old = new();
        double time = Time.unscaledTimeAsDouble;
        foreach (var cursor in cursors.Values)
        {
            if (time - cursor.lastUpdateTime > cursorTimeoutSeconds)
            {
                old.Add(cursor.netId);
            }
            else
            {
                cursor.rectTransform.position = Vector2.SmoothDamp(cursor.rectTransform.position, cursor.position, ref cursor.velocity, cursorSmoothTime);
            }
        }

        // Destroy old cursors
        for (int i = 0; i < old.Count; i++)
        {
            uint netId = old[i];
            Destroy(cursors[netId].rectTransform.gameObject);
            cursors.Remove(netId);
        }
    }

    [Command(channel = Channels.Unreliable, requiresAuthority = false)]
    public void CmdUpdateCursor(Vector2 position, NetworkConnectionToClient sender = null)
    {
        uint netId = sender.identity.netId;
        var clients = NetworkServer.connections.Values;
        foreach (var client in clients)
        {
            if (client.identity == null || client.identity.netId == netId)
            {
                continue;
            }

            TargetUpdateCursor(client, netId, position);
        }
    }

    [TargetRpc(channel = Channels.Unreliable)]
    private void TargetUpdateCursor(NetworkConnectionToClient _, uint netId, Vector2 position)
    {
        position = Camera.main.ViewportToScreenPoint(position);

        if (!cursors.TryGetValue(netId, out var cursor))
        {
            RectTransform rectTransform = Instantiate(cursorPrefab, position, Quaternion.identity, cursorsTransform);
            cursors[netId] = new(netId, rectTransform, position);
        }
        else
        {
            cursor.position = position;
            cursor.lastUpdateTime = Time.unscaledTimeAsDouble;
        } 
    }

    private class BombDefusalCursor
    {
        public readonly uint netId;
        public readonly RectTransform rectTransform;
        public Vector2 position;
        public Vector2 velocity;
        public double lastUpdateTime;

        public BombDefusalCursor(uint netId, RectTransform rectTransform, Vector2 position)
        {
            this.netId = netId;
            this.rectTransform = rectTransform;
            this.position = position;
            velocity = Vector2.zero;
            lastUpdateTime = Time.unscaledTimeAsDouble;
        }
    }
}
