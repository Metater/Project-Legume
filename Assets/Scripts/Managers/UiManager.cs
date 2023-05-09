using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : Manager
{
    private UiPanel activePanel = null;
    public bool HasActivePanel => activePanel != null;

    public bool TryUseEscapeKeyDown()
    {
        if (activePanel == null)
        {
            return false;
        }

        activePanel.Close();
        activePanel.IsOpen = false;
        activePanel = null;
        return true;
    }

    public void SetActivePanel(UiPanel panel)
    {
        if (HasActivePanel)
        {
            activePanel.Close();
            activePanel.IsOpen = false;
        }

        activePanel = panel;
        activePanel.IsOpen = true;
    }
}
