using System.Diagnostics;
using UnityEngine;

public class CursorManager : Manager
{
    public bool IsCursorVisable { get; private set; } = true;

    public override void ManagerAwake()
    {
        UpdateCursorVisibility();
    }
    public override void ManagerUpdate()
    {
        if (!manager.Get<PlayerManager>().IsLocalPlayerNull)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IsCursorVisable = !IsCursorVisable;
                UpdateCursorVisibility();
            }
        }
        else
        {
            IsCursorVisable = true;
            UpdateCursorVisibility();
        }

        manager.Get<CrosshairManager>().SetVisibility(!IsCursorVisable);
    }

    private void UpdateCursorVisibility()
    {
        Cursor.lockState = IsCursorVisable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsCursorVisable;
    }
}
