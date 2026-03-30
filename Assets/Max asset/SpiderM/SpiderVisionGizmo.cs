using UnityEngine;

public class SpiderVisionGizmo : MonoBehaviour
{
    [Header("Vision")]
    public float viewDistance = 8f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;

    [Header("Gizmo")]
    public int segments = 30;
    public Color fillColor = new Color(1f, 1f, 0f, 0.15f);
    public Color lineColor = Color.yellow;
    public Transform visionOrigin; 
    public Transform forwardReference;

    private void OnDrawGizmosSelected()
    {
        Transform origin = visionOrigin != null ? visionOrigin : transform;
        Vector3 pos = origin.position;
        Vector3 forward = forwardReference != null ? forwardReference.forward : transform.forward;
        Vector3 leftDir = DirFromAngle(-viewAngle * 0.5f, forward);
        Vector3 rightDir = DirFromAngle(viewAngle * 0.5f, forward);

        Gizmos.color = lineColor;
        Gizmos.DrawLine(pos, pos + leftDir * viewDistance);
        Gizmos.DrawLine(pos, pos + rightDir * viewDistance);
        Vector3 prevPoint = pos + leftDir * viewDistance;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float currentAngle = Mathf.Lerp(-viewAngle * 0.5f, viewAngle * 0.5f, t);
            Vector3 dir = DirFromAngle(currentAngle, forward);
            Vector3 nextPoint = pos + dir * viewDistance;

            Gizmos.DrawLine(prevPoint, nextPoint);
            Gizmos.DrawLine(pos, nextPoint); 
            prevPoint = nextPoint;
        }
    }

    private Vector3 DirFromAngle(float angleOffset, Vector3 baseForward)
    {
        Quaternion rot = Quaternion.AngleAxis(angleOffset, Vector3.up);
        return rot * baseForward.normalized;
    }
}