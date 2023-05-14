using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LogicSwitch : LogicOutput, IPointerDownHandler
{
    [SerializeField] private Image image;
    private bool isPowered = false;

    private void Start()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        image.color = isPowered ? LogicConstants.PoweredColor : LogicConstants.UnpoweredColor;
    }

    protected override bool GetOutput()
    {
        return isPowered;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPowered = !isPowered;

        UpdateVisual();
    }
}
