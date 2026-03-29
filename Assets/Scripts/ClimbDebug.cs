using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ClimbDebug : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbInteractable climbInteractable;

    private void Awake()
    {
        climbInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbInteractable>();

        if (climbInteractable != null)
        {
            climbInteractable.hoverEntered.AddListener(OnHoverEntered);
            climbInteractable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    private void OnDestroy()
    {
        if (climbInteractable != null)
        {
            climbInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            climbInteractable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Climb hover entered");
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Climb select entered");
    }
}