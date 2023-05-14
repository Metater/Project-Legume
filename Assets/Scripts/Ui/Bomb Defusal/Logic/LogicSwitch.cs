using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LogicSwitch : LogicOutput, IPointerDownHandler
{
    [SerializeField] private SyncedBool syncedBool;
    [SerializeField] private Image image;

    private void Update()
    {
        image.color = syncedBool.Value ? LogicConstants.PoweredColor : LogicConstants.UnpoweredColor;
    }

    protected override bool GetOutput()
    {
        return syncedBool.Value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        syncedBool.CmdSetValue(!syncedBool.Value);
    }
}
