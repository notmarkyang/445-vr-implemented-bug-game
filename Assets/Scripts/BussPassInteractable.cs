using UnityEngine;

public class BusPassInteractable : BaseInteractable
{
    [SerializeField] private GameObject passVisual;
    [SerializeField] private PickupVisualState pickupVisualState;

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

        if (!gs.trainUnlocked)
        {
            StoryTextUI.Instance?.ShowLines("We don't need that right now.");
            return;
        }

        if (gs.hasBusPass)
        {
            StoryTextUI.Instance?.ShowLines("I already picked that up.");
            return;
        }

        gs.CollectBusPass();

        StoryTextUI.Instance?.ShowLines(
            "This should work as a ticket.",
            "I should bring it to the train station."
        );

        ObjectiveUI.Instance?.SetObjective("Bring the ticket to the train station.");

        if (passVisual != null)
            passVisual.SetActive(false);

        if (pickupVisualState != null)
            pickupVisualState.Refresh();
    }
}