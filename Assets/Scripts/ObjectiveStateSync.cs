using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ObjectiveStateSync : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        RefreshObjective();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RefreshNextFrame());
    }

    private IEnumerator RefreshNextFrame()
    {
        yield return null;
        RefreshObjective();
    }

    public void RefreshObjective()
    {
        if (GameState.Instance == null || ObjectiveUI.Instance == null)
            return;

        string objective = GetObjectiveFromState(GameState.Instance);

        if (!string.IsNullOrEmpty(objective))
            ObjectiveUI.Instance.SetObjective(objective);
    }

    private string GetObjectiveFromState(GameState gs)
    {
        if (!gs.hasExaminedTank)
            return "Check the tank.";

        if (!gs.hasBracelet)
            return "Check the table.";

        if (!gs.talkedToTankBug && !gs.isSmall)
            return "Shrink down and get to the tank.";

        if (!gs.talkedToTankBug && gs.isSmall)
            return "Explore the tank.";

        if (gs.talkedToTankBug && !gs.hasReachedTrainArea)
            return "Go to the train station.";

        if (gs.hasReachedTrainArea && !gs.hasMetConductor)
            return "Talk to the conductor.";

        if (gs.hasMetConductor && !gs.hasBusPass && !gs.paidTrainToll)
            return "Find something to pay the fare.";

        if (gs.hasBusPass && !gs.paidTrainToll)
            return "Use the ticket machine.";

        if (gs.paidTrainToll && !gs.ventUnlocked)
            return "Talk to the conductor.";

        if (gs.ventUnlocked && !gs.sawBugStuckInVent)
            return "Check the vent.";

        if (gs.sawBugStuckInVent && !gs.hasScrewdriver)
            return "Find something to loosen the vent.";

        if (gs.hasScrewdriver && !gs.freedMissingBug)
            return "Return to the vent.";

        if (gs.freedMissingBug && !gs.talkedToFreedBug)
            return "Talk to the bug.";

        if (gs.bugReturnedToTank && !gs.finalTankConversationDone)
            return "Go back to the tank.";

        if (gs.finalTankConversationDone)
            return "Yipeeee.";

        return "";
    }
}