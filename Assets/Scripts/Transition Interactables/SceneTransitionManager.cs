using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    public string PendingSpawnID { get; private set; }
    public string LastUsedSpawnID { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName, string spawnID)
    {
        PendingSpawnID = spawnID;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(PendingSpawnID))
            return;

        Debug.Log("Scene loaded: " + scene.name + " | PendingSpawnID: " + PendingSpawnID);

        SceneSpawnPoint[] spawnPoints = FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);

        foreach (SceneSpawnPoint point in spawnPoints)
        {
            Debug.Log("Found spawn point: " + point.SpawnID + " at " + point.transform.position);

            if (point.SpawnID == PendingSpawnID)
            {
                GameObject xrRig = GameObject.FindWithTag("XRRig");
                if (xrRig != null)
                {
                    Camera cam = xrRig.GetComponentInChildren<Camera>();

                    if (cam != null)
                    {
                        Vector3 cameraOffset = cam.transform.position - xrRig.transform.position;
                        Vector3 targetRigPosition = point.transform.position - new Vector3(cameraOffset.x, 0f, cameraOffset.z);
                        xrRig.transform.position = targetRigPosition;
                    }
                    else
                    {
                        xrRig.transform.position = point.transform.position;
                    }
                }
                else
                {
                    Debug.LogWarning("No object tagged XRRig was found.");
                }

                // Store before clearing so PlayerScaleController can read it
                LastUsedSpawnID = PendingSpawnID;
                PendingSpawnID = null;
                return;
            }
        }

        Debug.LogWarning("No spawn point found for ID: " + PendingSpawnID);
        LastUsedSpawnID = PendingSpawnID;
        PendingSpawnID = null;
    }
}