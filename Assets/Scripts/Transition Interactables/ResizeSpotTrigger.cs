using UnityEngine;

public class ResizeSpotTrigger : MonoBehaviour
{
    [SerializeField] private ResizeSpot resizeSpot;

    private void OnTriggerEnter(Collider other)
    {
        PlayerScaleController scaler = other.GetComponentInParent<PlayerScaleController>();
        if (scaler != null)
        {
            scaler.SetCurrentResizeSpot(resizeSpot);
            Debug.Log("Entered resize spot: " + resizeSpot.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerScaleController scaler = other.GetComponentInParent<PlayerScaleController>();
        if (scaler != null)
        {
            scaler.ClearCurrentResizeSpot(resizeSpot);
            Debug.Log("Exited resize spot: " + resizeSpot.gameObject.name);
        }
    }
}