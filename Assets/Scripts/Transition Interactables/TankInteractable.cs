using UnityEngine;

public class TankInteractable : BaseInteractable
{
    public override void Interact()
    {
        if (!GameState.Instance.hasExaminedTank)
        {
            GameState.Instance.hasExaminedTank = true;

            StoryTextUI.Instance.ShowLines(
                "One of the bugs is missing...",
                "Maybe my friend left something behind.",
                "I should check the table."
            );

            ObjectiveUI.Instance.SetObjective("Check the table.");
        }
        else
        {
            StoryTextUI.Instance.ShowLines(
                "The tank feels emptier than before."
            );
        }
    }
}