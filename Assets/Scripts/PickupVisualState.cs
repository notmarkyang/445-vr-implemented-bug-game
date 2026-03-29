using UnityEngine;

public class PickupVisualState : MonoBehaviour
{
    public enum PickupType
    {
        Bracelet,
        TrainTicket,
        Screwdriver
    }

    [SerializeField] private PickupType pickupType;
    [SerializeField] private GameObject visualRoot;

    private void Awake()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (GameState.Instance == null || visualRoot == null)
            return;

        bool shouldBeVisible = true;

        switch (pickupType)
        {
            case PickupType.Bracelet:
                shouldBeVisible = !GameState.Instance.hasBracelet;
                break;

            case PickupType.TrainTicket:
                shouldBeVisible = !GameState.Instance.hasBusPass;
                break;

            case PickupType.Screwdriver:
                shouldBeVisible = !GameState.Instance.hasScrewdriver;
                break;
        }

        visualRoot.SetActive(shouldBeVisible);
    }
}