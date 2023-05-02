using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Manager
{
    [SerializeField] private Transform generalTransform;

    public override void ManagerAwake()
    {
        manager.Get<PlayerManager>().OnStartLocalPlayer += (player) =>
        {
            // Position own camera
            Camera.main.transform.SetParent(player.transform);
            float yOffset = player.Get<PlayerMovement>().HandsTransform.localPosition.y;
            Camera.main.transform.localPosition = new Vector3(0, yOffset, 0);
        };

        manager.Get<PlayerManager>().OnStopLocalPlayer += (player) =>
        {
            // Reset transform of own camera
            Camera.main.transform.SetParent(generalTransform);
        };
    }
}
