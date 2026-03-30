using UnityEngine;

public class FacePlayerOnInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;

    public void FaceOnce()
    {
        if (playerCamera == null)
            return;

        Vector3 direction = playerCamera.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}