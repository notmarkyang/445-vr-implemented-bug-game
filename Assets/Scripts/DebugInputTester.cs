using UnityEngine;

public class DebugInputTester : MonoBehaviour
{
    [SerializeField] private PlayerScaleController scaleController;
    [SerializeField] private TankInteractable tankInteractable;
    [SerializeField] private BraceletInteractable braceletInteractable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
                StoryTextUI.Instance.ShowNextLine();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && tankInteractable != null)
        {
            tankInteractable.Interact();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && braceletInteractable != null)
        {
            braceletInteractable.Interact();
        }

        if (Input.GetKeyDown(KeyCode.T) && scaleController != null)
        {
            scaleController.ToggleScale();
        }
    }
}