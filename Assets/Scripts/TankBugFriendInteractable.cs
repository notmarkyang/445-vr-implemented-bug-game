using UnityEngine;

public class TankBugFriendInteractable : BaseInteractable
{
    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        GameState gs = GameState.Instance;

        // Can only talk to bugs while small
        if (!gs.isSmall)
        {
            StoryTextUI.Instance?.ShowLines("Ew, a bug.");
            return;
        }

        // first important story conversation
        if (!gs.talkedToTankBug)
        {
            gs.TalkToTankBug();

            StoryTextUI.Instance?.ShowLines(
                "Oh, are you looking for Benni",
                "They got curious and wandered off.",
                "I think they headed toward the train station."
            );

            ObjectiveUI.Instance?.SetObjective("Go to the train station.");
            return;
        }

        // after train is unlocked - before vent is unlocked
        if (gs.trainUnlocked && !gs.ventUnlocked)
        {
            StoryTextUI.Instance?.ShowLines(
                "If you're looking for them, the train station is your best bet."
            );
            return;
        }

        // after vent gets unlocked; before player discovers the stuck bug
        if (gs.ventUnlocked && !gs.sawBugStuckInVent)
        {
            StoryTextUI.Instance?.ShowLines(
                "So they were seen near the vent?",
                "That doesn't sound good..."
            );
            return;
        }

        // after player finds the bug stuck, but before rescue
        if (gs.sawBugStuckInVent && !gs.freedMissingBug)
        {
            StoryTextUI.Instance?.ShowLines(
                "They're really stuck in there?",
                "Lol sucks to suck.",
                "...Don't tell them I said that..."
            );
            return;
        }

        // after rescue, before final return state is fully resolved
        if (gs.freedMissingBug && gs.bugReturnedToTank)
        {
            StoryTextUI.Instance?.ShowLines(
                "You found them?",
                "Ugh.",
                "*Coughs*",
                "I-I mean yippeee!"
            );
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "Thanks for helping everyone."
        );
    }
}