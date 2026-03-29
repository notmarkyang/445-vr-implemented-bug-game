using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LadybugLift : MonoBehaviour
{
    public enum LiftState
    {
        AtBottom,
        GoingUp,
        AtTopHover,
        AtTopPerch,
        GoingDown,
        ReturningToBottom,
        ReturningToTopHover,
        MovingToTopPerch
    }

    [Header("References")]
    [SerializeField] private Transform movingBugRoot;
    [SerializeField] private Transform carryAnchor;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform bottomLandingPoint;
    [SerializeField] private Transform topHoverPoint;
    [SerializeField] private Transform topLandingPoint;
    [SerializeField] private Transform topPerchPoint;
    [SerializeField] private XRSimpleInteractable grabInteractable;
    [SerializeField] private Transform xrRigRoot;
    [SerializeField] private CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float flySpeed = 2.5f;
    [SerializeField] private float arriveDistance = 0.02f;

    [Header("Rules")]
    [SerializeField] private bool requireSmallMode = true;

    private LiftState currentState = LiftState.AtBottom;
    private bool playerAttached = false;
    private Transform currentTarget;

    private void Awake()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void Start()
    {
        if (movingBugRoot != null && bottomPoint != null)
            movingBugRoot.position = bottomPoint.position;

        currentState = LiftState.AtBottom;
        currentTarget = null;
    }

    private void Update()
    {
        UpdateMovement();

        if (playerAttached && xrRigRoot != null && carryAnchor != null)
        {
            xrRigRoot.position = carryAnchor.position;
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (requireSmallMode && GameState.Instance != null && !GameState.Instance.isSmall)
        {
            if (StoryTextUI.Instance != null)
                StoryTextUI.Instance.ShowLines("Ew, a bug.");
            return;
        }

        if (xrRigRoot == null || carryAnchor == null)
            return;

        playerAttached = true;

        if (characterController != null)
            characterController.enabled = false;

        if (currentState == LiftState.AtBottom)
        {
            currentState = LiftState.GoingUp;
            currentTarget = topHoverPoint;
        }
        else if (currentState == LiftState.AtTopHover || currentState == LiftState.AtTopPerch)
        {
            currentState = LiftState.GoingDown;
            currentTarget = bottomPoint;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (!playerAttached)
            return;

        playerAttached = false;

        if (currentState == LiftState.GoingUp)
        {
            currentState = LiftState.ReturningToBottom;
            currentTarget = bottomPoint;

            if (characterController != null)
                characterController.enabled = true;

            if (StoryTextUI.Instance != null)
                StoryTextUI.Instance.ShowLines("You need to hold on tight.");
        }
        else if (currentState == LiftState.GoingDown)
        {
            currentState = LiftState.ReturningToBottom;
            currentTarget = bottomPoint;

            if (characterController != null)
                characterController.enabled = true;

            if (StoryTextUI.Instance != null)
                StoryTextUI.Instance.ShowLines("You need to hold on tight.");
        }
        else if (currentState == LiftState.AtTopHover)
        {
            PlaceRigAt(topLandingPoint);

            if (characterController != null)
                characterController.enabled = true;

            currentState = LiftState.MovingToTopPerch;
            currentTarget = topPerchPoint;
        }
        else if (currentState == LiftState.AtBottom)
        {
            PlaceRigAt(bottomLandingPoint != null ? bottomLandingPoint : bottomPoint);

            if (characterController != null)
                characterController.enabled = true;
        }
    }

    private void UpdateMovement()
    {
        if (movingBugRoot == null || currentTarget == null)
            return;

        movingBugRoot.position = Vector3.MoveTowards(
            movingBugRoot.position,
            currentTarget.position,
            flySpeed * Time.deltaTime
        );

        if (Vector3.Distance(movingBugRoot.position, currentTarget.position) <= arriveDistance)
        {
            movingBugRoot.position = currentTarget.position;

            switch (currentState)
            {
                case LiftState.GoingUp:
                    currentState = LiftState.AtTopHover;
                    currentTarget = null;
                    break;

                case LiftState.GoingDown:
                    currentState = LiftState.AtBottom;
                    currentTarget = null;

                    playerAttached = false;
                    PlaceRigAt(bottomLandingPoint != null ? bottomLandingPoint : bottomPoint);

                    if (characterController != null)
                        characterController.enabled = true;
                    break;

                case LiftState.ReturningToBottom:
                    currentState = LiftState.AtBottom;
                    currentTarget = null;
                    break;

                case LiftState.ReturningToTopHover:
                    currentState = LiftState.AtTopHover;
                    currentTarget = null;
                    break;

                case LiftState.MovingToTopPerch:
                    currentState = LiftState.AtTopPerch;
                    currentTarget = null;
                    break;
            }
        }
    }

    private void PlaceRigAt(Transform targetPoint)
    {
        if (xrRigRoot == null || targetPoint == null)
            return;

        xrRigRoot.position = targetPoint.position;
    }
}