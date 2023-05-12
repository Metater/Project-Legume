using UnityEngine;

public class DebugPointIndicator : MonoBehaviour
{
    [SerializeField] private GameObject modelGameObject;

    public void ShowAtPosition(Vector3 position)
    {
        modelGameObject.SetActive(true);
        modelGameObject.transform.position = position;
    }
    public void Hide()
    {
        modelGameObject.SetActive(false);
    }
}
