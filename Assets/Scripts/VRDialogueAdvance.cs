using UnityEngine;
using UnityEngine.InputSystem;

public class VRDialogueAdvance : MonoBehaviour
{
    [SerializeField] private InputActionReference advanceAction;

    private void OnEnable()
    {
        if (advanceAction != null)
            advanceAction.action.Enable();
    }

    private void OnDisable()
    {
        if (advanceAction != null)
            advanceAction.action.Disable();
    }

    private void Update()
    {
        if (advanceAction == null || StoryTextUI.Instance == null)
            return;

        if (advanceAction.action.WasPressedThisFrame() && StoryTextUI.Instance.IsShowing)
        {
            StoryTextUI.Instance.ShowNextLine();
        }
    }
}