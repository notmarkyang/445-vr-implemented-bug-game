using UnityEngine;
using UnityEngine.InputSystem;

public class VRScaleInput : MonoBehaviour
{
    [SerializeField] private PlayerScaleController scaler;
    [SerializeField] private InputActionReference toggleAction;

    private void OnEnable()
    {
        if (toggleAction != null && toggleAction.action != null)
            toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        if (toggleAction != null && toggleAction.action != null)
            toggleAction.action.Disable();
    }

    private void Update()
    {
        if (toggleAction == null || toggleAction.action == null || scaler == null)
            return;

        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (toggleAction.action.WasPressedThisFrame())
        {
            scaler.ToggleScale();
        }
    }
}