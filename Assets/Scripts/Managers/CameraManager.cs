using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Manager
{
    [SerializeField] private Transform generalTransform;
    [SerializeField] private float yOffset = 1.6f;

    public override void ManagerAwake()
    {
        manager.Get<PlayerManager>().OnStartLocalPlayer += (player) =>
        {
            // Position own camera
            Camera.main.transform.SetParent(player.transform);
            Camera.main.transform.localPosition = new Vector3(0, yOffset, 0);
        };

        manager.Get<PlayerManager>().OnStopLocalPlayer += (player) =>
        {
            // Reset transform of own camera
            Camera.main.transform.SetParent(generalTransform);
        };
    }
}
