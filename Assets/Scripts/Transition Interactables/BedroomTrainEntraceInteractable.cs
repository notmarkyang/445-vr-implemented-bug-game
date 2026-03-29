using UnityEngine;

public class BedroomTrainEntranceInteractable : BaseInteractable
{
    [Header("Train Scene Transition")]
    [SerializeField] private string targetSceneName = "TrainScene";
    [SerializeField] private string targetSpawnID = "TrainScene_Entry";

    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        if (!GameState.Instance.isSmall)
        {
            StoryTextUI.Instance?.ShowLines("I need to shrink first.");
            return;
        }

        if (!GameState.Instance.trainUnlocked)
        {
            StoryTextUI.Instance?.ShowLines("I don't think I should go here right now.");
            return;
        }

        SceneTransitionManager.Instance?.LoadScene(targetSceneName, targetSpawnID);
    }
}