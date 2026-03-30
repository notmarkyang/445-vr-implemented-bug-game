using UnityEngine;

public class SpiderAlertIndicator : MonoBehaviour
{
    public SpiderAlertSystem alertSystem;
    public Renderer targetRenderer;

    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

    void Update()
    {
        if (alertSystem == null || targetRenderer == null) return;

        switch (alertSystem.currentState)
        {
            case SpiderAlertSystem.AlertState.Green:
                targetRenderer.material.color = greenColor;
                break;
            case SpiderAlertSystem.AlertState.Yellow:
                targetRenderer.material.color = yellowColor;
                break;
            case SpiderAlertSystem.AlertState.Red:
                targetRenderer.material.color = redColor;
                break;
        }
    }
}