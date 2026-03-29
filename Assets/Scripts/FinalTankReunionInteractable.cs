using UnityEngine;

public class FinalTankReunionInteractable : BaseInteractable
{
    [SerializeField] private TankReturnBugVisualState visualState;

    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        GameState gs = GameState.Instance;

        if (!gs.isSmall)
        {
            StoryTextUI.Instance?.ShowLines("I need to shrink first.");
            return;
        }

        if (!gs.bugReturnedToTank)
        {
            StoryTextUI.Instance?.ShowLines("I should keep looking.");
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "Thanks again for helping me get back.",
            "I think I'll stay close to the tank from now on...",
            "I'm not built for the outside world..."
        );

        ObjectiveUI.Instance?.SetObjective("Everything is back to normal.");

        if (visualState != null)
            visualState.Refresh();
    }
}