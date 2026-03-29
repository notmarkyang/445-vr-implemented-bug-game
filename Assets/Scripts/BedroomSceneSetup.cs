using UnityEngine;

public class BedroomSceneSetup : MonoBehaviour
{
    private void Start()
    {
        if (ObjectiveUI.Instance != null)
            ObjectiveUI.Instance.SetObjective("Check on your friend’s bug.");
    }
}