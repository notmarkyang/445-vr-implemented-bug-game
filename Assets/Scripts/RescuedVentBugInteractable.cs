using UnityEngine;

public class RescuedVentBugInteractable : BaseInteractable
{
    [SerializeField] private GameObject rescuedBugVisual;
    [SerializeField] private VentPanelInteractable ventPanelInteractable;

    private void Awake()
    {
        RefreshVisualState();
    }

    private void OnEnable()
    {
        RefreshVisualState();
    }

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

        if (!gs.freedMissingBug)
        {
            StoryTextUI.Instance?.ShowLines("They're still stuck in the vent.");
            return;
        }

        if (!gs.talkedToFreedBug)
        {
            gs.talkedToFreedBug = true;
            gs.bugReturnedToTank = true;

            StoryTextUI.Instance?.ShowLines(
                "I'm never going exploring again...",
                "I'll meet you back at the tank..."
            );

            ObjectiveUI.Instance?.SetObjective("Go back to the tank.");

            RefreshVisualState();

            if (ventPanelInteractable != null)
                ventPanelInteractable.RefreshVisualState();

            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "I'll meet you back at the tank."
        );
    }

    public void RefreshVisualState()
    {
        if (GameState.Instance == null || rescuedBugVisual == null)
            return;

        rescuedBugVisual.SetActive(GameState.Instance.freedMissingBug && !GameState.Instance.talkedToFreedBug);
    }
}