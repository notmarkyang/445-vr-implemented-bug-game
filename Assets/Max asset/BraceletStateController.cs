using System.Collections.Generic;
using UnityEngine;

public class BraceletStateController : MonoBehaviour
{
    public enum BraceletState
    {
        Locked,             // 未解锁
        UnlockedNotWorn,    // 已解锁，但没戴上
        Equipping,          // 正在佩戴中（可选过渡态）
        IdleWorn,           // 已佩戴，待机
        MenuOpen,           // 菜单打开
        AbilityActive,      // 正在使用某个能力
        TutorialPlaying     // 正在播放教学/提示
    }

    public enum BraceletAbility
    {
        None,
        Shrink,
        TimeSlow,
        TalkToInsects,
        Inventory
    }

    [Header("Current State (read only)")]
    [SerializeField] private BraceletState currentState = BraceletState.Locked;
    [SerializeField] private BraceletAbility currentAbility = BraceletAbility.None;

    [Header("Optional Scene References")]
    [SerializeField] private GameObject braceletWorldObject;   // 场景中可抓取的手环物体
    [SerializeField] private GameObject braceletMenuObject;    // 手环菜单 UI
    [SerializeField] private AudioSource voiceAudioSource;     // 后续语音提示可用

    [Header("Shrink Landing Points")]
    [SerializeField] private List<string> landingPointNames = new List<string>();
    [SerializeField] private List<bool> landingPointUnlocked = new List<bool>();

    [Header("Time Slow")]
    [SerializeField] private float slowMotionScale = 0.5f;

    private float originalFixedDeltaTime;
    private bool isWorn = false;

    public BraceletState CurrentState => currentState;
    public BraceletAbility CurrentAbility => currentAbility;
    public bool IsWorn => isWorn;

    private void Awake()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;

        // 保证两个列表长度一致
        while (landingPointUnlocked.Count < landingPointNames.Count)
        {
            landingPointUnlocked.Add(false);
        }

        // 初始化菜单关闭
        if (braceletMenuObject != null)
        {
            braceletMenuObject.SetActive(false);
        }

        ApplyVisualState();
    }

    // ----------------------------
    // 基础流程
    // ----------------------------

    public bool TryUnlockBracelet()
    {
        if (currentState != BraceletState.Locked)
            return false;

        currentState = BraceletState.UnlockedNotWorn;
        Debug.Log("Bracelet unlocked.");
        ApplyVisualState();
        return true;
    }

    public bool TryBeginEquip()
    {
        if (currentState != BraceletState.UnlockedNotWorn)
            return false;

        currentState = BraceletState.Equipping;
        Debug.Log("Bracelet equipping...");
        ApplyVisualState();
        return true;
    }

    public void OnBraceletWorn()
    {
        isWorn = true;
        currentState = BraceletState.IdleWorn;
        Debug.Log("Bracelet worn successfully.");
        ApplyVisualState();
    }

    public void RemoveBracelet()
    {
        isWorn = false;
        currentAbility = BraceletAbility.None;
        StopTimeSlow();

        currentState = BraceletState.UnlockedNotWorn;

        if (braceletMenuObject != null)
            braceletMenuObject.SetActive(false);

        Debug.Log("Bracelet removed.");
        ApplyVisualState();
    }

    // ----------------------------
    // 菜单
    // ----------------------------

    public bool OpenMenu()
    {
        if (currentState != BraceletState.IdleWorn)
            return false;

        currentState = BraceletState.MenuOpen;

        if (braceletMenuObject != null)
            braceletMenuObject.SetActive(true);

        Debug.Log("Bracelet menu opened.");
        return true;
    }

    public bool CloseMenu()
    {
        if (currentState != BraceletState.MenuOpen)
            return false;

        currentState = BraceletState.IdleWorn;

        if (braceletMenuObject != null)
            braceletMenuObject.SetActive(false);

        Debug.Log("Bracelet menu closed.");
        return true;
    }

    // ----------------------------
    // 能力选择
    // ----------------------------

    public bool SelectShrinkAbility()
    {
        if (currentState != BraceletState.MenuOpen)
            return false;

        currentAbility = BraceletAbility.Shrink;
        currentState = BraceletState.AbilityActive;

        if (braceletMenuObject != null)
            braceletMenuObject.SetActive(false);

        Debug.Log("Shrink ability selected.");
        return true;
    }

    public bool SelectTimeSlowAbility()
    {
        if (currentState != BraceletState.MenuOpen)
            return false;

        currentAbility = BraceletAbility.TimeSlow;
        currentState = BraceletState.AbilityActive;

        if (braceletMenuObject != null)
            braceletMenuObject.SetActive(false);

        StartTimeSlow();
        Debug.Log("Time slow ability selected.");
        return true;
    }

    public bool SelectTalkToInsectsAbility()
    {
        if (currentState != BraceletState.MenuOpen)
            return false;

        currentAbility = BraceletAbility.TalkToInsects;
        currentState = BraceletState.AbilityActive;

        if (braceletMenuObject != null)
            braceletMenuObject.SetActive(false);

        Debug.Log("Talk to insects ability selected.");
        return true;
    }

    public bool SelectInventoryAbility()
    {
        if (currentState != BraceletState.MenuOpen)
            return false;

        currentAbility = BraceletAbility.Inventory;
        currentState = BraceletState.AbilityActive;

        if (braceletMenuObject != null)
            braceletMenuObject.SetActive(false);

        Debug.Log("Inventory ability selected.");
        return true;
    }

    public void FinishCurrentAbility()
    {
        if (currentState != BraceletState.AbilityActive)
            return;

        if (currentAbility == BraceletAbility.TimeSlow)
        {
            StopTimeSlow();
        }

        currentAbility = BraceletAbility.None;
        currentState = BraceletState.IdleWorn;

        Debug.Log("Ability finished. Back to idle.");
    }

    // ----------------------------
    // 缩小落点
    // ----------------------------

    public void UnlockLandingPoint(int index)
    {
        if (!IsValidLandingIndex(index))
            return;

        landingPointUnlocked[index] = true;
        Debug.Log($"Landing point unlocked: {landingPointNames[index]}");
    }

    public bool IsLandingPointUnlocked(int index)
    {
        if (!IsValidLandingIndex(index))
            return false;

        return landingPointUnlocked[index];
    }

    public bool TryUseShrinkLandingPoint(int index)
    {
        if (currentState != BraceletState.AbilityActive || currentAbility != BraceletAbility.Shrink)
            return false;

        if (!IsValidLandingIndex(index))
            return false;

        if (!landingPointUnlocked[index])
        {
            Debug.Log($"Landing point locked: {landingPointNames[index]}");
            return false;
        }

        // 这里只做状态逻辑；真正传送你后面再接 Teleportation Anchor
        Debug.Log($"Using shrink landing point: {landingPointNames[index]}");

        currentAbility = BraceletAbility.None;
        currentState = BraceletState.IdleWorn;
        return true;
    }

    // ----------------------------
    // 教学/语音
    // ----------------------------

    public bool BeginTutorial()
    {
        if (currentState == BraceletState.Locked)
            return false;

        currentState = BraceletState.TutorialPlaying;
        Debug.Log("Tutorial started.");
        return true;
    }

    public void EndTutorial()
    {
        if (!isWorn)
            currentState = BraceletState.UnlockedNotWorn;
        else
            currentState = BraceletState.IdleWorn;

        Debug.Log("Tutorial ended.");
    }

    // ----------------------------
    // 时间减慢
    // ----------------------------

    private void StartTimeSlow()
    {
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
    }

    private void StopTimeSlow()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }

    // ----------------------------
    // 工具
    // ----------------------------

    private bool IsValidLandingIndex(int index)
    {
        return index >= 0 && index < landingPointNames.Count;
    }

    private void ApplyVisualState()
    {
        // 这里只先保留接口，你后面可以在这里控制模型显示、材质、音效等
        if (braceletWorldObject != null)
        {
            braceletWorldObject.SetActive(currentState != BraceletState.Locked);
        }
    }

    // 方便你在 Inspector 里测试
    [ContextMenu("DEBUG Unlock Bracelet")]
    private void DebugUnlock()
    {
        TryUnlockBracelet();
    }

    [ContextMenu("DEBUG Open Menu")]
    private void DebugOpenMenu()
    {
        OpenMenu();
    }

    [ContextMenu("DEBUG Finish Ability")]
    private void DebugFinishAbility()
    {
        FinishCurrentAbility();
    }
}