using UnityEngine;

public class SpiderVisionDetector : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform eyePoint;
    public Transform forwardReference;

    [Header("Vision Settings")]
    public float viewDistance = 8f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;

    [Header("Blocking")]
    public LayerMask obstacleMask;

    [Header("Player Identify")]
    public string playerTag = "Player";

    [Header("Debug")]
    public bool canSeePlayer;
    public float currentDistance;
    public float currentAngle;

    void Update()
    {
        canSeePlayer = CheckCanSeePlayer();
    }

    public bool CheckCanSeePlayer()
    {
        if (player == null) return false;

        Transform origin = eyePoint != null ? eyePoint : transform;
        Vector3 originPos = origin.position;
        Vector3 targetPos = player.position;

        Vector3 toPlayer = targetPos - originPos;
        currentDistance = toPlayer.magnitude;

        if (currentDistance > viewDistance)
            return false;

        Vector3 forward = forwardReference != null ? forwardReference.forward : transform.forward;
        currentAngle = Vector3.Angle(forward, toPlayer.normalized);

        if (currentAngle > viewAngle * 0.5f)
            return false;

        Debug.DrawLine(originPos, targetPos, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(
            originPos,
            toPlayer.normalized,
            currentDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hit.collider.transform.IsChildOf(transform))
                continue;

            if (((1 << hitObj.layer) & obstacleMask.value) != 0)
            {
                Debug.Log("[SpiderVision] Blocked by obstacle: " + hitObj.name);
                return false;
            }

            if (hitObj.CompareTag(playerTag) || hit.collider.transform.root.CompareTag(playerTag))
            {
                Debug.Log("[SpiderVision] Saw player: " + hitObj.name);
                return true;
            }
        }

        return false;
    }
}