using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PhaseManager : Manager
{
    public bool IsReady { get; private set; } = true;
    public bool IsPlayerReady => !manager.Get<PlayerManager>().IsLocalPlayerNull && IsReady;
    public bool ShouldPlayerMove => IsPlayerReady && !manager.Get<CursorManager>().IsCursorVisable && !manager.Get<UiManager>().HasActivePanel;

    public override void ManagerUpdate()
    {
        if (IsPlayerReady)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (manager.Get<CursorManager>().IsCursorVisable || !manager.Get<UiManager>().TryUseEscapeKeyDown())
                {
                    // Toggle cursor
                    manager.Get<CursorManager>().SetVisibility(!manager.Get<CursorManager>().IsCursorVisable);
                }
            }
        }
        else
        {
            manager.Get<CursorManager>().SetVisibility(true);
        }

        manager.Get<CrosshairManager>().SetVisibility(!manager.Get<CursorManager>().IsCursorVisable);
    }
}
