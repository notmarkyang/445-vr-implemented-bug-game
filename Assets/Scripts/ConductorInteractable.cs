using UnityEngine;

public class ConductorInteractable : BaseInteractable
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

        if (!gs.hasMetConductor)
            gs.hasMetConductor = true;

        if (!gs.hasBusPass)
        {
            StoryTextUI.Instance?.ShowLines(
                "You want to know where [bug] went?",
                "Not 'til ya turn in the fare for the ride they hitched.",
                "No fare, no help.",
                "Use that ticket machine once you've got something to pay with."
            );

            ObjectiveUI.Instance?.SetObjective("Find something to pay the fare.");
            return;
        }

        if (!gs.paidTrainToll)
        {
            StoryTextUI.Instance?.ShowLines(
                "You'll need to use the ticket machine first.",
                "Then I can check the route log for you."
            );

            ObjectiveUI.Instance?.SetObjective("Use the ticket machine.");
            return;
        }

        if (!gs.talkedToConductor)
        {
            gs.talkedToConductor = true;
            gs.ventUnlocked = true;

            StoryTextUI.Instance?.ShowLines(
                "Alright, let's see...",
                "They were last seen heading toward the vent.",
                "You should check there next."
            );

            ObjectiveUI.Instance?.SetObjective("Check the vent.");
            return;
        }

        if (!gs.sawBugStuckInVent)
        {
            StoryTextUI.Instance?.ShowLines(
                "The vent's your best lead.",
                "That's the last place they were seen."
            );
            return;
        }

        if (!gs.freedMissingBug)
        {
            StoryTextUI.Instance?.ShowLines(
                "Still not back yet?",
                "I hope they're alright..."
            );
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "Glad to hear things worked out."
        );
    }
}