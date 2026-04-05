using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class ResetCurveVisualOnSceneLoad : MonoBehaviour
{
    private CurveVisualController curveVisual;
    private LineRenderer lr;

    void Awake()
    {
        curveVisual = GetComponent<CurveVisualController>();
        lr = GetComponentInChildren<LineRenderer>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ResetVisualNextFrame());
    }

    System.Collections.IEnumerator ResetVisualNextFrame()
    {
        if (curveVisual != null)
            curveVisual.enabled = false;

        if (lr != null)
        {
            lr.positionCount = 0;
            lr.enabled = false;
        }

        yield return null; // 等一帧，让新场景和XR状态先稳定

        if (lr != null)
            lr.enabled = true;

        if (curveVisual != null)
            curveVisual.enabled = true;
    }
}