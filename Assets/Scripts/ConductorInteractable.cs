using UnityEngine;
using System.Collections.Generic;

public class ConductorInteractable : BaseInteractable
{
    [Header("Repeat One-Liners")]
    [SerializeField] private List<string> noFareBarks = new List<string>()
    {
        "Well... get on with it."
    };

    [SerializeField] private List<string> useMachineBarks = new List<string>()
    {
        "Well that ticket aint gonna use itself."
    };

    [SerializeField] private List<string> postUnlockBarks = new List<string>()
    {
        "Go check the vent."
    };

    private bool saidNoFareDialogue = false;
    private bool saidUseMachineDialogue = false;

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

        if (!gs.hasMetConductor)
            gs.hasMetConductor = true;

        // no ticket
        if (!gs.hasBusPass)
        {
            if (!saidNoFareDialogue)
            {
                saidNoFareDialogue = true;

                StoryTextUI.Instance?.ShowLines(
                    "You want to know where Benni went?",
                    "Not 'til ya pay me back what y'all owe.",
                    "That rascal stole a ride from my establishment",
                    "No fare, no help.",
                    "Use that ticket machine once you've got somethin' to pay with."
                );

                ObjectiveUI.Instance?.SetObjective("Find something to pay the fare.");
            }
            else
            {
                SayRandom(noFareBarks);
            }

            return;
        }

        // has ticket but not used
        if (!gs.paidTrainToll)
        {
            if (!saidUseMachineDialogue)
            {
                saidUseMachineDialogue = true;

                StoryTextUI.Instance?.ShowLines(
                    "C'mon now."
                );

                ObjectiveUI.Instance?.SetObjective("Use the ticket machine.");
            }
            else
            {
                SayRandom(useMachineBarks);
            }

            return;
        }

        // vent unlocked
        if (!gs.talkedToConductor)
        {
            gs.talkedToConductor = true;
            gs.ventUnlocked = true;

            StoryTextUI.Instance?.ShowLines(
                "Alrighty.",
                "Not so hard now, was it.",
                "I reckon they were last seen heading toward the vents.",
                "You give that spot a lil gander."
            );

            ObjectiveUI.Instance?.SetObjective("Check the vent.");
            return;
        }

        // After area complete
        if (!gs.sawBugStuckInVent)
        {
            SayRandom(postUnlockBarks);
            return;
        }

        if (!gs.freedMissingBug)
        {
            StoryTextUI.Instance?.ShowLines(
                "Still not back yet?",
                "Kids these days."
            );
            return;
        }

        StoryTextUI.Instance?.ShowLines(
            "Glad to hear things worked out."
        );
    }

    private void SayRandom(List<string> lines)
    {
        if (lines == null || lines.Count == 0)
            return;

        int index = Random.Range(0, lines.Count);
        StoryTextUI.Instance?.ShowLines(lines[index]);
    }
}