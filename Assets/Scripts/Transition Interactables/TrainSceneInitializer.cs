using UnityEngine;

public class TrainSceneInitializer : MonoBehaviour
{
    private void Start()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.hasReachedTrainArea = true;
        }
    }
}