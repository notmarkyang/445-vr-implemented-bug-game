using UnityEngine;

public class SpiderAlertSystem : MonoBehaviour
{
    public enum AlertState
    {
        Green,
        Yellow,
        Red
    }

    [Header("References")]
    public SpiderVisionDetector visionDetector;

    [Header("Alert Values")]
    public float alertValue = 0f;
    public float maxAlertValue = 100f;
    public float yellowThreshold = 30f;
    public float redThreshold = 100f;

    [Header("Alert Speed")]
    public float minAlertGain = 10f;   // 远距离时每秒增长
    public float maxAlertGain = 40f;   // 近距离时每秒增长
    public float alertDecayRate = 20f; // 看不到时每秒下降

    [Header("Debug")]
    public AlertState currentState = AlertState.Green;

    void Update()
    {
        UpdateAlertValue();
        UpdateAlertState();
    }

    void UpdateAlertValue()
    {
        if (visionDetector == null) return;

        if (visionDetector.canSeePlayer)
        {
            float dist = visionDetector.currentDistance;
            float maxDist = visionDetector.viewDistance;

            float distancePercent = 1f - (dist / maxDist);
            distancePercent = Mathf.Clamp01(distancePercent);

            float gain = Mathf.Lerp(minAlertGain, maxAlertGain, distancePercent);
            alertValue += gain * Time.deltaTime;
        }
        else
        {
            alertValue -= alertDecayRate * Time.deltaTime;
        }

        alertValue = Mathf.Clamp(alertValue, 0f, maxAlertValue);
    }

    void UpdateAlertState()
    {
        if (alertValue >= redThreshold)
        {
            currentState = AlertState.Red;
        }
        else if (alertValue >= yellowThreshold)
        {
            currentState = AlertState.Yellow;
        }
        else
        {
            currentState = AlertState.Green;
        }
    }
}