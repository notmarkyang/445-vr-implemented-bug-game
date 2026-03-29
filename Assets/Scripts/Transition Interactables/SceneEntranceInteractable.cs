using UnityEngine;

public class SceneEntranceInteractable : BaseInteractable
{
    [Header("Scene Transition")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetSpawnID;

    [Header("Rules")]
    [SerializeField] private bool requireSmallMode = true;
    [SerializeField] private string tooBigMessage = "I need to shrink first.";

    [Header("Progression Gates")]
    [SerializeField] private bool requireTrainUnlocked = false;
    [SerializeField] private string trainLockedMessage = "Friend's voice echoed...There's a time an place for everything.";

    [SerializeField] private bool requireVentUnlocked = false;
    [SerializeField] private string ventLockedMessage = "Oh...Maybe not right now...";

    public override void Interact()
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (GameState.Instance == null)
            return;

        if (requireSmallMode && !GameState.Instance.isSmall)
        {
            StoryTextUI.Instance?.ShowLines(tooBigMessage);
            return;
        }

        if (requireTrainUnlocked && !GameState.Instance.trainUnlocked)
        {
            StoryTextUI.Instance?.ShowLines(trainLockedMessage);
            return;
        }

        if (requireVentUnlocked && !GameState.Instance.ventUnlocked)
        {
            StoryTextUI.Instance?.ShowLines(ventLockedMessage);
            return;
        }

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogWarning("SceneTransitionManager is missing.");
            return;
        }

        SceneTransitionManager.Instance.LoadScene(targetSceneName, targetSpawnID);
    }
}