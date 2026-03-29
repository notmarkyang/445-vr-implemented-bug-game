using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRInteractBridge : MonoBehaviour
{
    [SerializeField] private MonoBehaviour targetBehaviour;

    private BaseInteractable targetInteractable;
    private XRSimpleInteractable xrSimpleInteractable;

    private void Awake()
    {
        xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
        targetInteractable = targetBehaviour as BaseInteractable;

        if (xrSimpleInteractable != null)
            xrSimpleInteractable.selectEntered.AddListener(OnSelected);
    }

    private void OnDestroy()
    {
        if (xrSimpleInteractable != null)
            xrSimpleInteractable.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (targetInteractable != null)
            targetInteractable.Interact();
    }
}