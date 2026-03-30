using UnityEngine;

public class UIPanelPulse : MonoBehaviour
{
    [Header("引用")]
    public CanvasGroup brightPanelGroup;

    [Header("效果设置")]
    public float speed = 1.5f;
    public float minAlpha = 0.1f;
    public float maxAlpha = 0.55f;

    void Update()
    {
        if (brightPanelGroup == null) return;

        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        brightPanelGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
    }
}