using UnityEngine;

public abstract class UiPanel : MonoBehaviour
{
    protected GameManager manager;
    public bool IsOpen { get; set; } = false;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>(true);

        UiPanelAwake();
        Close();
    }
    private void Start() => UiPanelStart();
    private void Update() => UiPanelUpdate();
    private void LateUpdate() => UiPanelLateUpdate();

    protected virtual void UiPanelAwake() { }
    protected virtual void UiPanelStart() { }
    protected virtual void UiPanelUpdate() { }
    protected virtual void UiPanelLateUpdate() { }

    public abstract void Close();
}
