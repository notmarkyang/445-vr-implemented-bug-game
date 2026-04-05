using UnityEngine;

public class SimpleRotateX : MonoBehaviour
{
    [Header("Rotation Speed")]
    public float rotationSpeed = 90f; // 每秒旋转多少度

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}