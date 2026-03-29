using UnityEngine;

public class FlavorBugInteractable : BaseInteractable
{
    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        GameState gs = GameState.Instance;

        // can only talk to bugs while small
        if (!gs.isSmall)
        {
            StoryTextUI.Instance?.ShowLines("Ew, a bug.");
            return;
        }

        // before inspecting tank
        if (!gs.hasExaminedTank)
        {
            StoryTextUI.Instance?.ShowLines(
                "*Bug noises*",
                "*More bug noises*"
            );
            return;
        }

        // After tank inspection, before bracelet
        if (gs.hasExaminedTank && !gs.hasBracelet)
        {
            StoryTextUI.Instance?.ShowLines(
                "*Bug noises*"
            );
            return;
        }

        // After bracelet, before talking to tank bug
        if (gs.hasBracelet && !gs.talkedToTankBug)
        {
            StoryTextUI.Instance?.ShowLines(
                "That's a cute bracelet.",
                "Can I Have it?"
            );
            return;
        }

        // After train unlock, before toll is solved
        if (gs.trainUnlocked && !gs.ventUnlocked)
        {
            StoryTextUI.Instance?.ShowLines(
                "The train station...",
                "I heard there was a scuffle not too long ago."
            );
            return;
        }

        // After vent unlock, before seeing stuck bug
        if (gs.ventUnlocked && !gs.sawBugStuckInVent)
        {
            StoryTextUI.Instance?.ShowLines(
                "I think I seen someone near the vents?"
            );
            return;
        }

        // After seeing stuck bug, before screwdriver
        if (gs.sawBugStuckInVent && !gs.hasScrewdriver)
        {
            StoryTextUI.Instance?.ShowLines(
                "They're stuck?",
                "You need to find something to help get him out!"
            );
            return;
        }

        // After screwdriver, before freeing the bug
        if (gs.hasScrewdriver && !gs.freedMissingBug)
        {
            StoryTextUI.Instance?.ShowLines(
                "That should work.",
                "Go help them already!"
            );
            return;
        }

        // Final
        if (gs.bugReturnedToTank)
        {
            StoryTextUI.Instance?.ShowLines(
                "Everything turned out okay in the end.",
                "Pretty exciting day for such a tiny world."
            );
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "What a strange day..."
        );
    }
}