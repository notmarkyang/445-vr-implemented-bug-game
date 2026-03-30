using UnityEngine;

public class BraceletInteractable : BaseInteractable
{
    [SerializeField] private GameObject braceletVisual;
    [SerializeField] private PickupVisualState pickupVisualState;

    public override void Interact()
    {
        if (GameState.Instance == null)
            return;

        if (!GameState.Instance.hasExaminedTank)
        {
            StoryTextUI.Instance?.ShowLines("I should check the tank first.");
            return;
        }

        if (GameState.Instance.hasBracelet)
        {
            StoryTextUI.Instance?.ShowLines("I already picked up the bracelet.");
            return;
        }

        GameState.Instance.CollectBracelet();

        StoryTextUI.Instance?.ShowLines(
            "Ah right, here it is.",
            "I think this lets me shrink.",
            "Let's see...",
            @"""[Left trigger] to shrink in size.""",
            "Okay... Let's give this a try."
        );

        ObjectiveUI.Instance?.SetObjective("Shrink down and get to the tank.");

        if (braceletVisual != null)
            braceletVisual.SetActive(false);

        if (pickupVisualState != null)
            pickupVisualState.Refresh();
    }
}