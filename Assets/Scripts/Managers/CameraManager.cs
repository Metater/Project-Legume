using UnityEngine;

public class CameraManager : Manager
{
    [SerializeField] private Transform generalTransform;
    [SerializeField] private Vector3 defaultPosition = new(0, 1, -10);

    public override void ManagerAwake()
    {
        Camera.main.transform.localPosition = defaultPosition;

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
            Camera.main.transform.localPosition = defaultPosition;
            Camera.main.transform.localRotation = Quaternion.identity;
        };
    }
}
