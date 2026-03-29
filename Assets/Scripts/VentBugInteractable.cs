using UnityEngine;

public class VentBugInteractable : BaseInteractable
{
    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        GameState gs = GameState.Instance;

        if (!gs.isSmall)
        {
            StoryTextUI.Instance?.ShowLines("Ew, a bug.");
            return;
        }

        if (!gs.sawBugStuckInVent)
        {
            gs.DiscoverBugInVent();

            StoryTextUI.Instance?.ShowLines(
                "I'm stuck back here!",
                "I need help getting out."
            );

            ObjectiveUI.Instance?.SetObjective("Find something to loosen the vent.");
            return;
        }

        if (!gs.hasScrewdriver)
        {
            StoryTextUI.Instance?.ShowLines(
                "I can't get out like this.",
                "Maybe there's a tool nearby?"
            );
            return;
        }

        if (!gs.freedMissingBug)
        {
            gs.FreeMissingBug();

            StoryTextUI.Instance?.ShowLines(
                "You got it open!",
                "Thank you. I'll head back to the tank."
            );

            ObjectiveUI.Instance?.SetObjective("Go back to the tank.");
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "I'll meet you back at the tank."
        );
    }
}