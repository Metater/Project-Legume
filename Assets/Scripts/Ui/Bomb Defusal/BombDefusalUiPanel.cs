using System;
using System.Collections;
using UnityEngine;

public class BombDefusalUiPanel : UiPanel
{
    [SerializeField] private GameObject panelGameObject;
    [SerializeField] private BombDefusalCursors bombDefusalCursors;
    [SerializeField] private float cursorUpdateFrequency;

    protected override void UiPanelStart()
    {
        StartCoroutine(StartPeriodicCursorUpdates());
    }

    private IEnumerator StartPeriodicCursorUpdates()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f / cursorUpdateFrequency);

            if (IsOpen)
            {
                bombDefusalCursors.CmdUpdateCursor(Camera.main.ScreenToViewportPoint(Input.mousePosition));
            }
        }
    }

    public void Open()
    {
        manager.Get<UiManager>().SetActivePanel(this);
        manager.Get<CursorManager>().SetVisibility(true);


        panelGameObject.SetActive(true);
    }
    public override void Close()
    {
        manager.Get<CursorManager>().SetVisibility(false);

        panelGameObject.SetActive(false);
    }
}
