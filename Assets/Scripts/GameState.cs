using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [Header("Core Progress")]
    public bool hasExaminedTank = false;
    public bool hasBracelet = false;
    public bool canShrink = false;
    public bool isSmall = false;

    [Header("Tank Progress")]
    public bool talkedToTankBug = false;
    public bool trainUnlocked = false;

    [Header("Train Progress")]
    public bool hasMetConductor = false;
    public bool hasReachedTrainArea = false;
    public bool hasBusPass = false;
    public bool paidTrainToll = false;
    public bool talkedToConductor = false;
    public bool ventUnlocked = false;

    [Header("Vent Progress")]
    public bool sawBugStuckInVent = false;
    public bool hasScrewdriver = false;
    public bool freedMissingBug = false;
    public bool talkedToFreedBug = false;

    [Header("Final Progress")]
    public bool bugReturnedToTank = false;
    public bool finalTankConversationDone = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Core helers
    public void ResetProgress()
    {
        hasExaminedTank = false;
        hasBracelet = false;
        canShrink = false;
        isSmall = false;

        talkedToTankBug = false;
        trainUnlocked = false;

        hasBusPass = false;
        paidTrainToll = false;
        talkedToConductor = false;
        ventUnlocked = false;

        sawBugStuckInVent = false;
        hasScrewdriver = false;
        freedMissingBug = false;

        bugReturnedToTank = false;
        finalTankConversationDone = false;
    }

    public void SetSmallState(bool small)
    {
        isSmall = small;
    }

    public void MarkTankExamined()
    {
        hasExaminedTank = true;
    }

    public void CollectBracelet()
    {
        hasBracelet = true;
        canShrink = true;
    }

    public void TalkToTankBug()
    {
        talkedToTankBug = true;
        trainUnlocked = true;
    }

    public void CollectBusPass()
    {
        hasBusPass = true;
    }

    public void PayTrainToll()
    {
        paidTrainToll = true;
        talkedToConductor = true;
        ventUnlocked = true;
    }

    public void DiscoverBugInVent()
    {
        sawBugStuckInVent = true;
    }

    public void CollectScrewdriver()
    {
        hasScrewdriver = true;
    }

    public void FreeMissingBug()
    {
        freedMissingBug = true;
    }
}