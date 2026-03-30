using UnityEngine;

public class VentPanelInteractable : BaseInteractable
{
    [SerializeField] private GameObject ventClosedVisual;
    [SerializeField] private GameObject ventOpenVisual;
    [SerializeField] private GameObject trappedBugVisual;
    [SerializeField] private GameObject rescuedBugVisual;

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
            StoryTextUI.Instance?.ShowLines("This is too small to work on like this.");
            return;
        }

        if (!gs.sawBugStuckInVent)
        {
            gs.DiscoverBugInVent();

            StoryTextUI.Instance?.ShowLines(
                "Hey! I'm stuck back here!",
                "The vent cover won't budge.",
                "Can you find something to help me get out?"
            );

            ObjectiveUI.Instance?.SetObjective("Find something to loosen the vent.");
            return;
        }

        if (!gs.hasScrewdriver)
        {
            StoryTextUI.Instance?.ShowLines(
                "I'm still stuck...",
                "Maybe something from the bedroom could loosen this."
            );
            return;
        }

        if (!gs.freedMissingBug)
        {
            gs.FreeMissingBug();

            StoryTextUI.Instance?.ShowLines(
                "That did it!",
                "The vent's open now."
            );

            ObjectiveUI.Instance?.SetObjective("Talk to the bug.");

            RefreshVisualState();
            return;
        }

        if (gs.freedMissingBug && !gs.talkedToFreedBug)
        {
            StoryTextUI.Instance?.ShowLines(
                "The vent's open now."
            );
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "Umm.. I'd rather not."
        );
    }

    public void RefreshVisualState()
    {
        if (GameState.Instance == null)
            return;

        GameState gs = GameState.Instance;

        bool isFreed = gs.freedMissingBug;
        bool bugHasLeftVent = gs.talkedToFreedBug;

        if (ventClosedVisual != null)
            ventClosedVisual.SetActive(!isFreed);

        if (ventOpenVisual != null)
            ventOpenVisual.SetActive(isFreed);

        if (trappedBugVisual != null)
            trappedBugVisual.SetActive(!isFreed);

        if (rescuedBugVisual != null)
            rescuedBugVisual.SetActive(isFreed && !bugHasLeftVent);
    }
}