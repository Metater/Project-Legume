using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDefusalUiPanel : UiPanel
{
    [SerializeField] private RectTransform cursorRectTransform;

    protected override void UiPanelUpdate()
    {
        if (!IsOpen)
        {
            return;
        }

        cursorRectTransform.position = Input.mousePosition;
    }

    public void Open()
    {
        manager.Get<UiManager>().SetActivePanel(this);
        manager.Get<CursorManager>().SetVisibility(true);
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        manager.Get<CursorManager>().SetVisibility(false);
        gameObject.SetActive(false);
    }
}
