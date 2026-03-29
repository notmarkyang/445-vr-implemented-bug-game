using UnityEngine;

public class TicketMachineInteractable : BaseInteractable
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
            StoryTextUI.Instance?.ShowLines("This is too tiny to use like this.");
            return;
        }

        if (!gs.hasBusPass)
        {
            StoryTextUI.Instance?.ShowLines(
                "Looks like I need some kind of ticket.",
                "Maybe there's something in the bedroom I can use."
            );

            ObjectiveUI.Instance?.SetObjective("Find something to pay the fare.");
            return;
        }

        if (gs.paidTrainToll)
        {
            StoryTextUI.Instance?.ShowLines(
                "The fare's already been paid."
            );
            return;
        }

        gs.paidTrainToll = true;

        StoryTextUI.Instance?.ShowLines(
            "That should do it.",
            "I should ask the conductor now."
        );

        ObjectiveUI.Instance?.SetObjective("Talk to the conductor.");
    }
}