using UnityEngine;

public class CircleOutline : MonoBehaviour
{
    LineRenderer lineRenderer;
    public Color color = Color.yellow;
    public float radius = 7.5f;
    public float lineWidth = 3f;
    public int segments = 100;
    public bool showInEditor = true;
    public bool showInGame = true;

    void Awake()
    {
        if (showInGame) {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = segments + 1;
            lineRenderer.useWorldSpace = false;
            lineRenderer.material.color = color;
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.generateLightingData = true;

            float deltaTheta = (2.0f * Mathf.PI) / segments;
            float theta = 0.0f;

            for (int i = 0; i < segments + 1; i++)
            {
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);
                Vector3 pos = new Vector3(x, y, 0);
                lineRenderer.SetPosition(i, pos);
                theta += deltaTheta;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showInEditor) {
            Gizmos.color = color;

            Vector3 center = transform.position;
            float angle = 360.0f / segments;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 direction = Vector3.right * radius;
            Vector3 lastPoint = center + direction;

            for (int i = 0; i < segments; i++)
            {
                direction = rotation * direction;
                Vector3 newPoint = center + direction;
                Gizmos.DrawLine(lastPoint, newPoint);
                lastPoint = newPoint;
            }
        }
    }

}