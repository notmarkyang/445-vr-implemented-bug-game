using UnityEngine;

public class ResizeSpot : MonoBehaviour
{
    [SerializeField] private Transform snapPointBig;
    [SerializeField] private Transform snapPointSmall;

    public Transform SnapPointBig => snapPointBig;
    public Transform SnapPointSmall => snapPointSmall;
}