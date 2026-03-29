using UnityEngine;

public class BedroomTankInteractable : BaseInteractable
{
    [Header("Tank Scene Transition")]
    [SerializeField] private string targetSceneName = "TankScene";
    [SerializeField] private string targetSpawnID = "TankScene_Entry";

    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        if (!GameState.Instance.hasExaminedTank)
        {
            GameState.Instance.MarkTankExamined();

            StoryTextUI.Instance?.ShowLines(
                "One of the bugs is missing...",
                "I think my friend left something behind.",
                "It should be on the nightstand."
            );

            ObjectiveUI.Instance?.SetObjective("Check the table.");
            return;
        }

        if (!GameState.Instance.isSmall)
        {
            StoryTextUI.Instance?.ShowLines("I need to shrink first.");
            return;
        }

        SceneTransitionManager.Instance?.LoadScene(targetSceneName, targetSpawnID);
    }
}