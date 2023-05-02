using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : Manager
{
    [SerializeField] private Image image;
    [SerializeField] private Color defaultColor = new(1, 1, 1, 0.5f);
    private Color? color = null;

    // [SerializeField] private Color defaultItemColor = new(0.2039216f, 0.5647059f, 0.8117647f, 0.4980392f);

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
