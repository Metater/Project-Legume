using UnityEngine;
using UnityEngine.UI;

public class WireVisual : Graphic
{
    [SerializeField] private Transform start, end;
    [SerializeField] private float thickness;
    private bool isPowered = false;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (start == null || end == null || thickness == 0) return;

        // Vertex order: BL, TL, TR, BR

        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = isPowered ? LogicConstants.PoweredColor : LogicConstants.UnpoweredColor;

        vertex.position = start.localPosition - new Vector3(0, thickness / 2);
        vh.AddVert(vertex);

        vertex.position = start.localPosition + new Vector3(0, thickness / 2);
        vh.AddVert(vertex);


        vertex.position = end.localPosition - rectTransform.localPosition + new Vector3(0, thickness / 2);
        vh.AddVert(vertex);

        vertex.position = end.localPosition - rectTransform.localPosition - new Vector3(0, thickness / 2);
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            SetVerticesDirty();
        }
    }

    public void SetIsPowered(bool isPowered)
    {
        if (isPowered == this.isPowered)
        {
            return;
        }

        this.isPowered = isPowered;
        SetVerticesDirty();
    }
}
