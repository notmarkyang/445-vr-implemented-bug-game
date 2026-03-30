using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

public class HeadTurnVignetteProvider : MonoBehaviour, ITunnelingVignetteProvider
{
    [Header("References")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private TunnelingVignetteController vignetteController;

    [Header("Turn Detection")]
    [SerializeField] private bool yawOnly = true;
    [SerializeField] private float minTurnSpeed = 20f;
    [SerializeField] private float maxTurnSpeed = 120f;

    [Header("Vignette Strength")]
    [SerializeField] [Range(0f, 1f)] private float idleAperture = 1f;
    [SerializeField] [Range(0f, 1f)] private float maxVignetteAperture = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float feathering = 0.2f;
    [SerializeField] private float easeInTime = 0.05f;
    [SerializeField] private float easeOutTime = 0.1f;

    [Header("Smoothing")]
    [SerializeField] private float responseSpeed = 8f;

    private readonly VignetteParameters runtimeParameters = new VignetteParameters();

    private float currentAperture = 1f;
    private float lastYaw;
    private Quaternion lastRotation;
    private bool vignetteActive;

    public VignetteParameters vignetteParameters => runtimeParameters;

    private void Start()
    {
        if (headTransform == null || vignetteController == null)
        {
            Debug.LogWarning("HeadTurnVignetteProvider is missing references.", this);
            enabled = false;
            return;
        }

        lastRotation = headTransform.rotation;
        lastYaw = headTransform.eulerAngles.y;
        currentAperture = idleAperture;

        runtimeParameters.apertureSize = idleAperture;
        runtimeParameters.featheringEffect = feathering;
        runtimeParameters.easeInTime = easeInTime;
        runtimeParameters.easeOutTime = easeOutTime;
        runtimeParameters.easeInTimeLock = false;
        runtimeParameters.easeOutDelayTime = 0f;
        runtimeParameters.vignetteColor = Color.black;
        runtimeParameters.vignetteColorBlend = Color.black;
        runtimeParameters.apertureVerticalPosition = 0f;
    }

    private void Update()
    {
        float turnSpeed = GetTurnSpeed();
        float normalized = Mathf.InverseLerp(minTurnSpeed, maxTurnSpeed, turnSpeed);

        float targetAperture = Mathf.Lerp(idleAperture, maxVignetteAperture, normalized);
        currentAperture = Mathf.Lerp(currentAperture, targetAperture, Time.deltaTime * responseSpeed);

        runtimeParameters.apertureSize = currentAperture;
        runtimeParameters.featheringEffect = feathering;
        runtimeParameters.easeInTime = easeInTime;
        runtimeParameters.easeOutTime = easeOutTime;

        bool shouldBeActive = currentAperture < (idleAperture - 0.01f);

        if (shouldBeActive && !vignetteActive)
        {
            vignetteController.BeginTunnelingVignette(this);
            vignetteActive = true;
        }
        else if (!shouldBeActive && vignetteActive)
        {
            vignetteController.EndTunnelingVignette(this);
            vignetteActive = false;
        }
        else if (shouldBeActive && vignetteActive)
        {
            vignetteController.BeginTunnelingVignette(this);
        }
    }

    private float GetTurnSpeed()
    {
        if (yawOnly)
        {
            float currentYaw = headTransform.eulerAngles.y;
            float yawDelta = Mathf.Abs(Mathf.DeltaAngle(lastYaw, currentYaw));
            lastYaw = currentYaw;
            return yawDelta / Mathf.Max(Time.deltaTime, 0.0001f);
        }

        float angleDelta = Quaternion.Angle(lastRotation, headTransform.rotation);
        lastRotation = headTransform.rotation;
        return angleDelta / Mathf.Max(Time.deltaTime, 0.0001f);
    }

    private void OnDisable()
    {
        if (vignetteActive && vignetteController != null)
        {
            vignetteController.EndTunnelingVignette(this);
            vignetteActive = false;
        }
    }
}