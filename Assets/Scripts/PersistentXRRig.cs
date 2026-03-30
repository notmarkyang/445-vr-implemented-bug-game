using UnityEngine;

public class PersistentXRRig : MonoBehaviour
{
    private static bool exists;

    private void Awake()
    {
        if (exists)
        {
            Destroy(gameObject);
            return;
        }

        exists = true;
        DontDestroyOnLoad(gameObject);
    }
}