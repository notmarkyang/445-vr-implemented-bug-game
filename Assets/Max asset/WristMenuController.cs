using UnityEngine;
using UnityEngine.UI;

public class WristMenuController : MonoBehaviour
{
    [Header("设置")]
    public GameObject uiPanel;
    public float activationTime = 1.0f;
    public string handTag = "PlayerHand";

    [Header("状态控制")]
    [SerializeField] private bool isEquipped = false;

    [Header("反馈（可选）")]
    public Image loadingBar;

    private float timer = 0f;
    private bool isHandInside = false;
    private bool isMenuOpen = false;
    private float autoCloseTimer = 0f;
    private float closeDelay = 8.0f;

    // --- 修改点 2: 提供给外部调用的公共接口 ---

    /// <summary>
    /// 设置手环的佩戴状态 (提供给 Runtime/其他脚本调用)
    /// </summary>
    /// <param name="status">是否佩戴</param>
    public void SetEquippedStatus(bool status)
    {
        isEquipped = status;

        // 如果强制脱下，立即关闭相关 UI
        if (!isEquipped)
        {
            ForceCloseAll();
        }
    }

    /// <summary>
    /// 获取当前佩戴状态
    /// </summary>
    public bool GetEquippedStatus() => isEquipped;

    // ----------------------------------------

    void Update()
    {
        if (!isEquipped) return;

        if (isHandInside && !isMenuOpen)
        {
            timer += Time.deltaTime;
            if (loadingBar != null)
            {
                loadingBar.gameObject.SetActive(true);
                loadingBar.fillAmount = timer / activationTime;
            }
            if (timer >= activationTime) OpenMenu();
        }

        if (!isHandInside && isMenuOpen)
        {
            autoCloseTimer += Time.deltaTime;
            if (autoCloseTimer >= closeDelay) CloseMenu();
        }
        else
        {
            autoCloseTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEquipped && other.CompareTag(handTag))
        {
            isHandInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(handTag))
        {
            isHandInside = false;
            ResetTimer();
        }
    }

    void OpenMenu()
    {
        isMenuOpen = true;
        autoCloseTimer = 0f;
        if (uiPanel != null) uiPanel.SetActive(true);
        if (loadingBar != null) loadingBar.gameObject.SetActive(false);
    }

    void ResetTimer()
    {
        timer = 0f;
        if (!isMenuOpen && loadingBar != null)
        {
            loadingBar.fillAmount = 0;
            loadingBar.gameObject.SetActive(false);
        }
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        autoCloseTimer = 0f;
        if (uiPanel != null) uiPanel.SetActive(false);
        if (loadingBar != null)
        {
            loadingBar.gameObject.SetActive(isHandInside);
            loadingBar.fillAmount = 0;
        }
    }

    // 强制关闭所有关联 UI（当突然脱下手环时调用）
    private void ForceCloseAll()
    {
        isMenuOpen = false;
        isHandInside = false;
        timer = 0f;
        if (uiPanel != null) uiPanel.SetActive(false);
        if (loadingBar != null) loadingBar.gameObject.SetActive(false);
    }
}