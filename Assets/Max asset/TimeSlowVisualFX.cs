using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeSlowVisualFX : MonoBehaviour
{
    [Header("Volume Profile")]
    public Volume globalVolume;

    [Header("Chromatic Aberration")]
    [Range(0f, 1f)]
    public float activeChromaticIntensity = 0.6f;
    public float inactiveChromaticIntensity = 0f;

    private ChromaticAberration chromaticAberration;

    private void Awake()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("Global Volume is not assigned.");
            return;
        }

        if (globalVolume.profile.TryGet(out chromaticAberration))
        {
            chromaticAberration.active = true;
            chromaticAberration.intensity.Override(inactiveChromaticIntensity);
        }
        else
        {
            Debug.LogWarning("Chromatic Aberration override not found in Volume Profile.");
        }
    }

    public void EnableTimeSlowEffect()
    {
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.Override(activeChromaticIntensity);
            Debug.Log("Time slow visual effect enabled.");
        }
    }

    public void DisableTimeSlowEffect()
    {
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.Override(inactiveChromaticIntensity);
            Debug.Log("Time slow visual effect disabled.");
        }
    }
}