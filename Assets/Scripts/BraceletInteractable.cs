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
            "Ah right, our friend said something about this bracelet...",
            "I think this lets me shrink.",
            "I should try it somewhere safe."
        );

        ObjectiveUI.Instance?.SetObjective("Shrink down and get to the tank.");

        if (braceletVisual != null)
            braceletVisual.SetActive(false);

        if (pickupVisualState != null)
            pickupVisualState.Refresh();
    }
}