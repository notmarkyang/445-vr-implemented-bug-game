using UnityEngine;
using System.Collections.Generic;

public class BugNPCInteractable : BaseInteractable
{
    [Header("When Player Is Big")]
    [SerializeField] private List<string> bigSizeLines = new List<string> { "Ew, a bug." };

    [Header("Before Tank Inspection")]
    [SerializeField] private List<string> beforeTankInspectionLines = new List<string>();

    [Header("After Tank Inspection, Before Bracelet")]
    [SerializeField] private List<string> afterTankBeforeBraceletLines = new List<string>();

    [Header("After Bracelet, Before Talking to Tank Bug")]
    [SerializeField] private List<string> afterBraceletBeforeTankBugLines = new List<string>();

    [Header("Train Phase")]
    [SerializeField] private List<string> trainPhaseLines = new List<string>();

    [Header("Vent Unlocked, Before Seeing Stuck Bug")]
    [SerializeField] private List<string> ventUnlockedLines = new List<string>();

    [Header("Stuck Bug Found, Before Screwdriver")]
    [SerializeField] private List<string> stuckBugFoundLines = new List<string>();

    [Header("Has Screwdriver, Before Rescue")]
    [SerializeField] private List<string> hasScrewdriverLines = new List<string>();

    [Header("Final Phase (Late Game / After Return State)")]
    [SerializeField] private List<string> finalPhaseLines = new List<string>();

    [Header("Fallback")]
    [SerializeField] private List<string> fallbackLines = new List<string> { "*Bug noises*" };

    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        GameState gs = GameState.Instance;

        if (!gs.isSmall)
        {
            ShowLinesOrFallback(bigSizeLines);
            return;
        }

        if (!gs.hasExaminedTank)
        {
            ShowLinesOrFallback(beforeTankInspectionLines);
            return;
        }

        if (gs.hasExaminedTank && !gs.hasBracelet)
        {
            ShowLinesOrFallback(afterTankBeforeBraceletLines);
            return;
        }

        if (gs.hasBracelet && !gs.talkedToTankBug)
        {
            ShowLinesOrFallback(afterBraceletBeforeTankBugLines);
            return;
        }

        if (gs.trainUnlocked && !gs.ventUnlocked)
        {
            ShowLinesOrFallback(trainPhaseLines);
            return;
        }

        if (gs.ventUnlocked && !gs.sawBugStuckInVent)
        {
            ShowLinesOrFallback(ventUnlockedLines);
            return;
        }

        if (gs.sawBugStuckInVent && !gs.hasScrewdriver)
        {
            ShowLinesOrFallback(stuckBugFoundLines);
            return;
        }

        if (gs.hasScrewdriver && !gs.freedMissingBug)
        {
            ShowLinesOrFallback(hasScrewdriverLines);
            return;
        }

        if (gs.bugReturnedToTank || gs.finalTankConversationDone)
        {
            ShowLinesOrFallback(finalPhaseLines);
            return;
        }

        ShowLinesOrFallback(fallbackLines);
    }

    private void ShowLinesOrFallback(List<string> preferredLines)
    {
        if (preferredLines != null && preferredLines.Count > 0)
        {
            StoryTextUI.Instance?.ShowLines(preferredLines.ToArray());
            return;
        }

        if (fallbackLines != null && fallbackLines.Count > 0)
        {
            StoryTextUI.Instance?.ShowLines(fallbackLines.ToArray());
        }
    }
}