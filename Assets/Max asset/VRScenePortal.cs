using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VRScenePortal : MonoBehaviour
{
    public static VRScenePortal Instance;

    [System.Serializable]
    public class FixedDestination
    {
        [Header("基本信息")]
        public string id;
        public string displayName;
        public string sceneName;
        public bool unlocked;

        [Header("场景中的可视目标")]
        public GameObject visualMarker;
        public Transform targetPoint;
        public LineRenderer lineRenderer;
    }

    [Header("黑屏淡入淡出")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1.0f;

    [Header("缩小模式滤镜")]
    public CanvasGroup modeCanvas;
    public float modeFadeDuration = 0.5f;

    [Header("玩家根物体")]
    public Transform playerRoot; // XR Origin 根物体

    [Header("线条起点")]
    public Transform braceletOrigin; // 手环/手表位置

    [Header("弧线设置")]
    public float arcHeight = 1.5f;
    public int arcSegments = 20;

    [Header("主场景设置")]
    public string mainSceneName = "MainScene";

    [Header("主功能UI")]
    public GameObject shrinkModeButtonPanel; // 原本进入缩小模式的面板
    public GameObject returnButtonPanel;     // 返回主场景的面板

    [Header("目的地UI按钮")]
    public GameObject tankButton;
    public GameObject trainButton;
    public GameObject ventButton;

    [Header("固定目的地")]
    public FixedDestination tankDestination;
    public FixedDestination trainStationDestination;
    public FixedDestination ventDestination;

    private bool isTravelling = false;
    private bool isShrinkModeActive = false;
    private FixedDestination currentSelectedDestination;

    private void Update()
    {
        if (!isShrinkModeActive || isTravelling)
            return;

        UpdateVisibleArcLines();
    }

    private void UpdateVisibleArcLines()
    {
        if (tankDestination != null && tankDestination.unlocked && tankDestination.lineRenderer != null && tankDestination.lineRenderer.enabled)
            UpdateArcLine(tankDestination);

        if (trainStationDestination != null && trainStationDestination.unlocked && trainStationDestination.lineRenderer != null && trainStationDestination.lineRenderer.enabled)
            UpdateArcLine(trainStationDestination);

        if (ventDestination != null && ventDestination.unlocked && ventDestination.lineRenderer != null && ventDestination.lineRenderer.enabled)
            UpdateArcLine(ventDestination);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
        }

        if (modeCanvas != null)
        {
            modeCanvas.alpha = 0f;
            modeCanvas.blocksRaycasts = false;
        }

        // 初始状态：只解锁 tank
        tankDestination.id = "tank";
        if (string.IsNullOrEmpty(tankDestination.displayName))
            tankDestination.displayName = "Tank";
        if (string.IsNullOrEmpty(tankDestination.sceneName))
            tankDestination.sceneName = "tank";
        tankDestination.unlocked = true;

        trainStationDestination.id = "train";
        if (string.IsNullOrEmpty(trainStationDestination.displayName))
            trainStationDestination.displayName = "Train Station";
        if (string.IsNullOrEmpty(trainStationDestination.sceneName))
            trainStationDestination.sceneName = "train station";

        ventDestination.id = "vent";
        if (string.IsNullOrEmpty(ventDestination.displayName))
            ventDestination.displayName = "Vent";
        if (string.IsNullOrEmpty(ventDestination.sceneName))
            ventDestination.sceneName = "vent";

        HideAllDestinationVisuals();
        HideAllDestinationButtons();

        SetNormalModeUI();
    }

    // =========================
    // UI 状态
    // =========================
    private void SetNormalModeUI()
    {
        if (shrinkModeButtonPanel != null)
            shrinkModeButtonPanel.SetActive(true);

        if (returnButtonPanel != null)
            returnButtonPanel.SetActive(false);
    }

    private void SetShrinkModeUI()
    {
        if (shrinkModeButtonPanel != null)
            shrinkModeButtonPanel.SetActive(false);

        if (returnButtonPanel != null)
            returnButtonPanel.SetActive(true);
    }

    // =========================
    // 进入缩小模式
    // =========================
    public void EnterShrinkMode()
    {
        if (isTravelling || isShrinkModeActive)
            return;

        isShrinkModeActive = true;
        currentSelectedDestination = null;

        Debug.Log("Entered shrink mode.");

        SetShrinkModeUI();
        StartCoroutine(FadeCanvas(modeCanvas, 1f, modeFadeDuration));
        RefreshDestinationVisuals();
        RefreshDestinationButtons();
    }

    // =========================
    // 退出缩小模式（只退出模式，不切场景）
    // =========================
    public void CancelShrinkMode()
    {
        if (!isShrinkModeActive || isTravelling)
            return;

        isShrinkModeActive = false;
        currentSelectedDestination = null;

        Debug.Log("Shrink mode cancelled.");

        StartCoroutine(FadeCanvas(modeCanvas, 0f, modeFadeDuration));
        HideAllDestinationVisuals();
        HideAllDestinationButtons();
        SetNormalModeUI();
    }

    // =========================
    // 返回主场景
    // 你可以绑定到新的返回UI
    // =========================
    public void ReturnToMainScene()
    {
        if (isTravelling)
            return;

        StartCoroutine(ReturnToMainSceneRoutine());
    }

    private IEnumerator ReturnToMainSceneRoutine()
    {
        isTravelling = true;
        isShrinkModeActive = false;
        currentSelectedDestination = null;

        HideAllDestinationVisuals();
        HideAllDestinationButtons();

        yield return StartCoroutine(FadeCanvas(modeCanvas, 0f, modeFadeDuration));
        yield return StartCoroutine(FadeCanvas(fadeCanvas, 1f, fadeDuration));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject spawnPoint = GameObject.FindWithTag("Respawn");
        if (spawnPoint != null)
        {
            if (playerRoot != null)
            {
                playerRoot.position = spawnPoint.transform.position;
                playerRoot.rotation = spawnPoint.transform.rotation;
            }
            else
            {
                Debug.LogWarning("playerRoot is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("No object with tag 'Respawn' found in main scene.");
        }

        yield return StartCoroutine(FadeCanvas(fadeCanvas, 0f, fadeDuration));

        SetNormalModeUI();
        isTravelling = false;
    }

    // =========================
    // 三个固定选择函数
    // =========================
    public void SelectTank()
    {
        SelectDestination(tankDestination);
    }

    public void SelectTrainStation()
    {
        SelectDestination(trainStationDestination);
    }

    public void SelectVent()
    {
        SelectDestination(ventDestination);
    }

    // =========================
    // 通用选择逻辑
    // =========================
    private void SelectDestination(FixedDestination destination)
    {
        if (!isShrinkModeActive || isTravelling)
            return;

        if (destination == null)
            return;

        if (!destination.unlocked)
        {
            Debug.Log("Destination is locked: " + destination.displayName);
            return;
        }

        currentSelectedDestination = destination;

        Debug.Log("Selected destination: " + destination.displayName);

        StartCoroutine(ConfirmAndTravelRoutine(destination.sceneName));
    }

    // =========================
    // 最终切场景
    // =========================
    private IEnumerator ConfirmAndTravelRoutine(string sceneName)
    {
        isTravelling = true;
        isShrinkModeActive = false;

        yield return StartCoroutine(FadeCanvas(modeCanvas, 0f, modeFadeDuration));
        HideAllDestinationVisuals();
        HideAllDestinationButtons();

        yield return StartCoroutine(FadeCanvas(fadeCanvas, 1f, fadeDuration));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject spawnPoint = GameObject.FindWithTag("Respawn");
        if (spawnPoint != null)
        {
            if (playerRoot != null)
            {
                playerRoot.position = spawnPoint.transform.position;
                playerRoot.rotation = spawnPoint.transform.rotation;
            }
            else
            {
                Debug.LogWarning("playerRoot is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("No object with tag 'Respawn' found in new scene.");
        }

        yield return StartCoroutine(FadeCanvas(fadeCanvas, 0f, fadeDuration));

        // 到达缩小场景后，不恢复原按钮，而是保留返回按钮
        SetShrinkModeUI();

        currentSelectedDestination = null;
        isTravelling = false;
    }

    // =========================
    // 远程解锁接口
    // =========================
    public void UnlockTank()
    {
        tankDestination.unlocked = true;
        Debug.Log("Unlocked: Tank");
    }

    public void UnlockTrainStation()
    {
        trainStationDestination.unlocked = true;
        Debug.Log("Unlocked: Train Station");
    }

    public void UnlockVent()
    {
        ventDestination.unlocked = true;
        Debug.Log("Unlocked: Vent");
    }

    public void UnlockDestinationById(string id)
    {
        switch (id.ToLower())
        {
            case "tank":
                UnlockTank();
                break;
            case "train":
            case "train station":
            case "trainstation":
                UnlockTrainStation();
                break;
            case "vent":
                UnlockVent();
                break;
            default:
                Debug.LogWarning("Unknown destination id: " + id);
                break;
        }
    }

    public bool IsDestinationUnlocked(string id)
    {
        switch (id.ToLower())
        {
            case "tank":
                return tankDestination.unlocked;
            case "train":
            case "train station":
            case "trainstation":
                return trainStationDestination.unlocked;
            case "vent":
                return ventDestination.unlocked;
            default:
                return false;
        }
    }

    // =========================
    // 更新显示
    // =========================
    private void RefreshDestinationVisuals()
    {
        RefreshSingleDestination(tankDestination);
        RefreshSingleDestination(trainStationDestination);
        RefreshSingleDestination(ventDestination);
    }

    private void RefreshSingleDestination(FixedDestination destination)
    {
        if (destination == null)
            return;

        if (destination.unlocked)
        {
            ShowDestinationVisuals(destination, true);
            UpdateArcLine(destination);
        }
        else
        {
            ShowDestinationVisuals(destination, false);
        }
    }

    private void HideAllDestinationVisuals()
    {
        ShowDestinationVisuals(tankDestination, false);
        ShowDestinationVisuals(trainStationDestination, false);
        ShowDestinationVisuals(ventDestination, false);
    }

    private void ShowDestinationVisuals(FixedDestination destination, bool show)
    {
        if (destination.visualMarker != null)
            destination.visualMarker.SetActive(show);

        if (destination.lineRenderer != null)
            destination.lineRenderer.enabled = show;
    }

    // =========================
    // 弧线绘制
    // =========================
    private void UpdateArcLine(FixedDestination destination)
    {
        if (braceletOrigin == null || destination.lineRenderer == null || destination.targetPoint == null)
            return;

        LineRenderer lr = destination.lineRenderer;
        lr.positionCount = arcSegments + 1;

        Vector3 start = braceletOrigin.position;
        Vector3 end = destination.targetPoint.position;

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            Vector3 point = Vector3.Lerp(start, end, t);
            point.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            lr.SetPosition(i, point);
        }
    }

    // =========================
    // 淡入淡出
    // =========================
    private IEnumerator FadeCanvas(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.blocksRaycasts = true;

        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (Mathf.Approximately(targetAlpha, 0f))
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void RefreshDestinationButtons()
    {
        if (tankButton != null)
            tankButton.SetActive(tankDestination != null && tankDestination.unlocked);

        if (trainButton != null)
            trainButton.SetActive(trainStationDestination != null && trainStationDestination.unlocked);

        if (ventButton != null)
            ventButton.SetActive(ventDestination != null && ventDestination.unlocked);
    }

    private void HideAllDestinationButtons()
    {
        if (tankButton != null) tankButton.SetActive(false);
        if (trainButton != null) trainButton.SetActive(false);
        if (ventButton != null) ventButton.SetActive(false);
    }
}