using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScaleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform worldRoot;
    [SerializeField] private Transform xrRigRoot;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private MonoBehaviour moveProviderBehaviour;

    [Header("World Scale States")]
    [SerializeField] private Vector3 normalWorldScale = Vector3.one;
    [SerializeField] private Vector3 giantWorldScale = new Vector3(5f, 5f, 5f);

    [Header("Move Speeds")]
    [SerializeField] private float normalMoveSpeed = 2.5f;
    [SerializeField] private float smallMoveSpeed = 8f;

    private ResizeSpot currentResizeSpot;

    public void SetCurrentResizeSpot(ResizeSpot spot)
    {
        currentResizeSpot = spot;
    }

    public void ClearCurrentResizeSpot(ResizeSpot spot)
    {
        if (currentResizeSpot == spot)
            currentResizeSpot = null;
    }

    public bool IsInsideResizeSpot()
    {
        return currentResizeSpot != null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ApplyCurrentStateVisuals();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wait a frame so SceneTransitionManager places the rig first,
        // then apply scale and re-snap to the spawn point.
        StartCoroutine(ApplyVisualsNextFrame());
    }

    private IEnumerator ApplyVisualsNextFrame()
    {
        yield return null; // let SceneTransitionManager's OnSceneLoaded run first

        ApplyCurrentStateVisuals();

        // If we're small, the world just scaled to 5x which shifts everything.
        // Re-snap the rig to wherever SceneTransitionManager placed us.
        if (GameState.Instance != null && GameState.Instance.isSmall)
        {
            string spawnID = SceneTransitionManager.Instance?.LastUsedSpawnID;
            if (!string.IsNullOrEmpty(spawnID))
            {
                SceneSpawnPoint[] points = FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);
                foreach (SceneSpawnPoint point in points)
                {
                    if (point.SpawnID == spawnID)
                    {
                        Camera cam = xrRigRoot.GetComponentInChildren<Camera>();
                        if (cam != null)
                        {
                            Vector3 cameraOffset = cam.transform.position - xrRigRoot.position;
                            xrRigRoot.position = point.transform.position - new Vector3(cameraOffset.x, 0f, cameraOffset.z);
                        }
                        else
                        {
                            xrRigRoot.position = point.transform.position;
                        }

                        Debug.Log("[PlayerScaleController] Re-snapped rig to spawn point: " + spawnID + " at " + point.transform.position);
                        break;
                    }
                }
            }
        }
    }

    public void ToggleScale()
    {
        if (GameState.Instance == null)
        {
            Debug.LogWarning("GameState instance is missing.");
            return;
        }

        if (StoryTextUI.Instance != null && StoryTextUI.Instance.IsShowing)
            return;

        if (SceneManager.GetActiveScene().name != "BedroomScene")
        {
            StoryTextUI.Instance?.ShowLines("Doesn't look like we have the space for that.");
            return;
        }

        if (!GameState.Instance.canShrink)
        {
            StoryTextUI.Instance?.ShowLines("I can't do that yet.");
            return;
        }

        if (!IsInsideResizeSpot())
        {
            StoryTextUI.Instance?.ShowLines("I should do this somewhere safe.");
            return;
        }

        if (GameState.Instance.isSmall)
            SetNormalMode();
        else
            SetSmallMode();
    }

    private void SetNormalMode()
    {
        if (currentResizeSpot == null || currentResizeSpot.SnapPointBig == null)
        {
            Debug.LogWarning("Missing big snap point on current resize spot.");
            return;
        }

        ApplyScaleWithSnap(normalWorldScale, currentResizeSpot.SnapPointBig);
        SetMoveSpeed(normalMoveSpeed);

        if (GameState.Instance != null)
            GameState.Instance.SetSmallState(false);
    }

    private void SetSmallMode()
    {
        if (currentResizeSpot == null || currentResizeSpot.SnapPointSmall == null)
        {
            Debug.LogWarning("Missing small snap point on current resize spot.");
            return;
        }

        ApplyScaleWithSnap(giantWorldScale, currentResizeSpot.SnapPointSmall);
        SetMoveSpeed(smallMoveSpeed);

        if (GameState.Instance != null)
            GameState.Instance.SetSmallState(true);
    }

    private void ApplyCurrentStateVisuals()
    {
        if (GameState.Instance == null || worldRoot == null)
            return;

        if (GameState.Instance.isSmall)
        {
            worldRoot.localScale = giantWorldScale;
            SetMoveSpeed(smallMoveSpeed);
        }
        else
        {
            worldRoot.localScale = normalWorldScale;
            SetMoveSpeed(normalMoveSpeed);
        }
    }

    private void SetMoveSpeed(float speed)
    {
        if (moveProviderBehaviour == null)
            return;

        var type = moveProviderBehaviour.GetType();
        var field = type.GetField("moveSpeed");
        if (field != null)
        {
            field.SetValue(moveProviderBehaviour, speed);
            return;
        }

        var property = type.GetProperty("moveSpeed");
        if (property != null && property.CanWrite)
        {
            property.SetValue(moveProviderBehaviour, speed);
            return;
        }

        Debug.LogWarning("Could not set moveSpeed on move provider.");
    }

    private void ApplyScaleWithSnap(Vector3 newScale, Transform targetSnapPoint)
    {
        if (worldRoot == null || xrRigRoot == null || playerCamera == null || targetSnapPoint == null)
        {
            Debug.LogWarning("PlayerScaleController is missing required references.");
            return;
        }

        worldRoot.localScale = newScale;

        Vector3 cameraOffset = playerCamera.position - xrRigRoot.position;
        Vector3 targetRigPosition = targetSnapPoint.position - new Vector3(cameraOffset.x, 0f, cameraOffset.z);

        xrRigRoot.position = targetRigPosition;
    }
}