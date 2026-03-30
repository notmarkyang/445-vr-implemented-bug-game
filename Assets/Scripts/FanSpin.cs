using UnityEngine;

public class FanSpin : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 180f;

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }
}