using UnityEngine;

public class TankReturnBugVisualState : MonoBehaviour
{
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

        visualRoot.SetActive(GameState.Instance.bugReturnedToTank);
    }
}