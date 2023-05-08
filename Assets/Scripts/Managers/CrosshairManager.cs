using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : Manager
{
    [SerializeField] private Image image;
    [SerializeField] private Color defaultColor = new(1, 1, 1, 0.5f);
    private Color? color = null;

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

    public void SetVisibility(bool visible)
    {
        image.gameObject.SetActive(visible);
    }
}
