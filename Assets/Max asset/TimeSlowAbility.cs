using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TimeSlowAbility : MonoBehaviour
{
    public static TimeSlowAbility Instance;
    public TimeSlowVisualFX visualFX;

    [Header("时间减速设置")]
    [Range(0.001f, 1f)]
    public float slowTimeScale = 0.05f;
    public float activeDuration = 3f;
    public float cooldownDuration = 8f;

    [Header("缩小模式蓝色滤镜")]
    public CanvasGroup shrinkModeFilterCanvas;
    public float filterFadeDuration = 0.5f;

    [Header("UI - 可选")]
    public Button abilityButton;
    public Image cooldownOverlay;           // 可选：按钮上的遮罩
    public TMP_Text statusText;             // 可选：显示 ACTIVE / CD / READY
    public TMP_Text timerText;              // 可选：显示剩余时间数字

    [Header("按钮颜色 - 可选")]
    public Image buttonImage;
    public Color readyColor = Color.white;
    public Color activeColor = Color.cyan;
    public Color cooldownColor = Color.gray;

    private bool isActive = false;
    private bool isCoolingDown = false;

    private float activeTimer = 0f;
    private float cooldownTimer = 0f;

    private float defaultFixedDeltaTime;
    private Coroutine filterFadeRoutine;

    public bool IsActive => isActive;
    public bool IsCoolingDown => isCoolingDown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Start()
    {
        if (shrinkModeFilterCanvas != null)
        {
            shrinkModeFilterCanvas.alpha = 0f;
            shrinkModeFilterCanvas.blocksRaycasts = false;
        }

        UpdateUIReadyState();
    }

    private void Update()
    {
        // 技能生效中
        if (isActive)
        {
            activeTimer -= Time.unscaledDeltaTime;

            if (timerText != null)
                timerText.text = activeTimer.ToString("F1");

            if (activeTimer <= 0f)
            {
                EndSlowMode();
            }
        }
        // 冷却中
        else if (isCoolingDown)
        {
            cooldownTimer -= Time.unscaledDeltaTime;

            if (timerText != null)
                timerText.text = cooldownTimer.ToString("F1");

            if (cooldownOverlay != null)
            {
                float fill = cooldownTimer / cooldownDuration;
                cooldownOverlay.fillAmount = Mathf.Clamp01(fill);
            }

            if (cooldownTimer <= 0f)
            {
                FinishCooldown();
            }
        }
    }

    // 外部按钮 / 其他脚本都可以调用这个
    public void TryActivateSlowMode()
    {
        if (isActive || isCoolingDown)
            return;

        ActivateSlowMode();
    }

    private void ActivateSlowMode()
    {
        isActive = true;
        activeTimer = activeDuration;

        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

        if (visualFX != null)
            visualFX.EnableTimeSlowEffect();

        ShowShrinkFilter();

        Debug.Log("Time Slow Activated.");

        UpdateUIActiveState();
    }

    private void EndSlowMode()
    {
        isActive = false;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;

        if (visualFX != null)
            visualFX.DisableTimeSlowEffect();

        HideShrinkFilter();

        Debug.Log("Time Slow Ended. Entering cooldown.");

        StartCooldown();
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownDuration;

        UpdateUICooldownState();
    }

    private void FinishCooldown()
    {
        isCoolingDown = false;
        cooldownTimer = 0f;

        Debug.Log("Cooldown Finished.");

        UpdateUIReadyState();
    }

    // 如果你想手动强制取消，也可以给外部调用
    public void ForceStopAndReset()
    {
        isActive = false;
        isCoolingDown = false;
        activeTimer = 0f;
        cooldownTimer = 0f;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;

        if (visualFX != null)
            visualFX.DisableTimeSlowEffect();

        HideShrinkFilterImmediate();

        UpdateUIReadyState();
    }

    private void ShowShrinkFilter()
    {
        if (shrinkModeFilterCanvas == null)
            return;

        if (filterFadeRoutine != null)
            StopCoroutine(filterFadeRoutine);

        filterFadeRoutine = StartCoroutine(FadeCanvasGroup(shrinkModeFilterCanvas, 1f, filterFadeDuration));
    }

    private void HideShrinkFilter()
    {
        if (shrinkModeFilterCanvas == null)
            return;

        if (filterFadeRoutine != null)
            StopCoroutine(filterFadeRoutine);

        filterFadeRoutine = StartCoroutine(FadeCanvasGroup(shrinkModeFilterCanvas, 0f, filterFadeDuration));
    }

    private void HideShrinkFilterImmediate()
    {
        if (shrinkModeFilterCanvas == null)
            return;

        if (filterFadeRoutine != null)
        {
            StopCoroutine(filterFadeRoutine);
            filterFadeRoutine = null;
        }

        shrinkModeFilterCanvas.alpha = 0f;
        shrinkModeFilterCanvas.blocksRaycasts = false;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.blocksRaycasts = false;

        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        filterFadeRoutine = null;
    }

    private void UpdateUIReadyState()
    {
        if (statusText != null)
            statusText.text = "READY";

        if (timerText != null)
            timerText.text = "";

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;

        if (abilityButton != null)
            abilityButton.interactable = true;

        if (buttonImage != null)
            buttonImage.color = readyColor;
    }

    private void UpdateUIActiveState()
    {
        if (statusText != null)
            statusText.text = "ACTIVE";

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;

        if (abilityButton != null)
            abilityButton.interactable = false;

        if (buttonImage != null)
            buttonImage.color = activeColor;
    }

    private void UpdateUICooldownState()
    {
        if (statusText != null)
            statusText.text = "COOLDOWN";

        if (abilityButton != null)
            abilityButton.interactable = false;

        if (buttonImage != null)
            buttonImage.color = cooldownColor;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
        }
    }

    private void OnApplicationQuit()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }
}