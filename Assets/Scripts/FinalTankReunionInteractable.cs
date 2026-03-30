using UnityEngine;

public class FinalTankReunionInteractable : BaseInteractable
{
    [SerializeField] private TankReturnBugVisualState visualState;
    [Header("Face Player")]
    [SerializeField] private FacePlayerOnInteract facePlayer;

    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;
        facePlayer?.FaceOnce();

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

        if (!gs.finalTankConversationDone)
        {
            gs.finalTankConversationDone = true;

            StoryTextUI.Instance?.ShowLines(
                "Thanks again for helping me get back.",
                "I think I'll stay close to the tank from now on...",
                "I'm not built for the outside world..."
            );

            ObjectiveUI.Instance?.SetObjective("Everything is back to normal.");
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "I'm just glad to be home."
        );

        if (visualState != null)
            visualState.Refresh();
    }
}