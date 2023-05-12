using UnityEngine;

public class BombDefusalUiPanel : UiPanel
{
    [SerializeField] private GameObject panelGameObject;
    [SerializeField] private RectTransform cursorRectTransform;
    private Canvas canvas;
    private Vector3 cursorOffset;

    protected override void UiPanelAwake()
    {
        canvas = FindObjectOfType<Canvas>(true);
        cursorOffset = cursorRectTransform.localPosition;
    }
    protected override void UiPanelUpdate()
    {
        if (!IsOpen)
        {
            return;
        }

        cursorRectTransform.position = Input.mousePosition + Vector3.Scale(cursorOffset, canvas.transform.localScale);
    }

    public void Open()
    {
        manager.Get<UiManager>().SetActivePanel(this);
        manager.Get<CursorManager>().SetVisibility(true);


        gameObject.SetActive(true);
        cursorRectTransform.gameObject.SetActive(true);
    }
    public override void Close()
    {
        manager.Get<CursorManager>().SetVisibility(false);

        gameObject.SetActive(false);
    }
}
