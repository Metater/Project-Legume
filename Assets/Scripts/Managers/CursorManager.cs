using UnityEngine;

public class CursorManager : Manager
{
    private bool isInit = false;
    public bool IsCursorVisable { get; private set; } = true;

    public override void ManagerAwake()
    {
        SetVisibility(IsCursorVisable);
    }

    public void SetVisibility(bool isCursorVisible)
    {
        if (isInit && isCursorVisible == IsCursorVisable)
        {
            return;
        }

        isInit = true;
        Cursor.lockState = isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isCursorVisible;
        IsCursorVisable = isCursorVisible;
    }
}
