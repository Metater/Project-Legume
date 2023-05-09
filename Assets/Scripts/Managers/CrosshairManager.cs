using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : Manager
{
    [SerializeField] private Image image;
    [SerializeField] private Color defaultColor = new(1, 1, 1, 0.5f);
    private Color? color = null;
    private bool isInit = false;
    public bool IsCrosshairVisable { get; private set; } = true;

    public override void ManagerAwake()
    {
        SetVisibility(IsCrosshairVisable);
    }
    public override void ManagerLateUpdate()
    {
        if (color == null)
        {
            image.color = defaultColor;
        }
        else
        {
            image.color = color.Value;
        }

        color = null;
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public void SetVisibility(bool isCrosshairVisible)
    {
        if (isInit && isCrosshairVisible == IsCrosshairVisable)
        {
            return;
        }

        isInit = true;
        image.gameObject.SetActive(isCrosshairVisible);
        IsCrosshairVisable = isCrosshairVisible;
    }
}
