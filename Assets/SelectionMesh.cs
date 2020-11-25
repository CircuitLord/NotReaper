using NotReaper;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMesh : Graphic
{
    VertexHelper vh;
    [SerializeField] Canvas canvas;
    [SerializeField] Transform centerPoint;
    [SerializeField] Sprite sprite;
    public Mesh m;

    private void CreateQuad(VertexHelper vh, float offsetX, float offsetY)
    {
        Vector2 corner1 = Vector2.zero;
        Vector2 corner2 = Vector2.zero;

        corner1.x = 0f;
        corner1.y = 0f;
        corner2.x = 1f;
        corner2.y = 1f;

        corner1.x -= rectTransform.pivot.x;
        corner1.y -= rectTransform.pivot.y;
        corner2.x -= rectTransform.pivot.x;
        corner2.y -= rectTransform.pivot.y;

        corner1.x *= rectTransform.rect.width;
        corner1.y *= rectTransform.rect.height;
        corner2.x *= rectTransform.rect.width;
        corner2.y *= rectTransform.rect.height;



        UIVertex vert1 = UIVertex.simpleVert;
        UIVertex vert2 = UIVertex.simpleVert;
        UIVertex vert3 = UIVertex.simpleVert;
        UIVertex vert4 = UIVertex.simpleVert;

        vert1.position = new Vector2(corner1.x + offsetX, corner1.y + offsetY);
        vert1.color = color;

        //topleft
        vert2.position = new Vector2(corner1.x + offsetX, corner2.y + offsetY);
        vert2.color = color;

        //topright
        vert3.position = new Vector2(corner2.x + offsetX, corner2.y + offsetY);
        vert3.color = color;

        vert4.position = new Vector2(corner2.x + offsetX, corner1.y + offsetY);
        vert4.color = color;

        UIVertex[] verts = { vert1, vert2, vert3, vert4 };
        vh.AddUIVertexQuad(verts);
    }

    public void GenerateMeshForTimeline()
    {

        m = new Mesh();
        var timeline = Timeline.instance;
        using (var vh = new VertexHelper(m))
        {
            vh.Clear();
            for (int i = 0; i < timeline.selectedNotes.Count; i++)
            {
                var selectedTarget = timeline.selectedNotes[i].data;
                float canvasScale = (1 / canvas.transform.localScale.x);
                CreateQuad(vh, (selectedTarget.x * canvasScale), (selectedTarget.y * canvasScale));
            }

            //CreateQuad(vh, 10);
            vh.FillMesh(m);
            var cr = GetComponent<CanvasRenderer>();
            cr.SetMesh(m);
        }

        transform.position = Vector3.zero;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GenerateMeshForTimeline();
        }
    }
    protected override void Awake()
    {

    }


}