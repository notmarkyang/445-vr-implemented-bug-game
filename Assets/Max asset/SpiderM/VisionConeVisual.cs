using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionConeVisual : MonoBehaviour
{
    [Header("Source")]
    public SpiderVisionDetector visionDetector;

    [Header("Shape")]
    public int segments = 40;
    public float yOffset = 0.02f;

    [Header("Colors")]
    public Color greenColor = new Color(0f, 1f, 0f, 0.18f);
    public Color yellowColor = new Color(1f, 0.85f, 0f, 0.22f);
    public Color redColor = new Color(1f, 0f, 0f, 0.25f);

    [Header("Optional State Link")]
    public SpiderAlertSystem alertSystem;

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Vision Cone Mesh";

        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();

        CreateMesh();
        UpdateColor();
    }

    void LateUpdate()
    {
        CreateMesh();
        UpdateColor();
    }

    void CreateMesh()
    {
        if (visionDetector == null) return;

        float viewDistance = visionDetector.viewDistance;
        float viewAngle = visionDetector.viewAngle;

        int vertexCount = segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 3];

        vertices[0] = new Vector3(0f, yOffset, 0f);

        float step = viewAngle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -viewAngle * 0.5f + step * i;
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * viewDistance;
            float z = Mathf.Cos(rad) * viewDistance;

            vertices[i + 1] = new Vector3(x, yOffset, z);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void UpdateColor()
    {
        if (meshRenderer == null || meshRenderer.material == null) return;

        Color targetColor = greenColor;

        if (alertSystem != null)
        {
            switch (alertSystem.currentState)
            {
                case SpiderAlertSystem.AlertState.Green:
                    targetColor = greenColor;
                    break;
                case SpiderAlertSystem.AlertState.Yellow:
                    targetColor = yellowColor;
                    break;
                case SpiderAlertSystem.AlertState.Red:
                    targetColor = redColor;
                    break;
            }
        }

        meshRenderer.material.color = targetColor;
    }
}