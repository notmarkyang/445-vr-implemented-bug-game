using UnityEngine;

public class ScrewdriverInteractable : BaseInteractable
{
    [SerializeField] private GameObject screwdriverVisual;

    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        GameState gs = GameState.Instance;

        if (gs.isSmall)
        {
            StoryTextUI.Instance?.ShowLines("This looks a little too big for us.");
            return;
        }

        if (!gs.sawBugStuckInVent)
        {
            StoryTextUI.Instance?.ShowLines("We don't need that right now.");
            return;
        }

        if (gs.hasScrewdriver)
        {
            StoryTextUI.Instance?.ShowLines("I already picked that up.");
            return;
        }

        gs.CollectScrewdriver();

        StoryTextUI.Instance?.ShowLines(
            "This should help loosen the vent."
        );

        ObjectiveUI.Instance?.SetObjective("Return to the vent.");

        if (screwdriverVisual != null)
            screwdriverVisual.SetActive(false);
    }
}