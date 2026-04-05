using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRRaySceneSwitchDebug : MonoBehaviour
{
    private XRRayInteractor rayInteractor;

    void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void Update()
    {
        if (rayInteractor == null) return;

        Debug.DrawRay(transform.position, transform.forward * 5f, Color.red);

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.Log(
                $"[XR DEBUG][3D] Hit: {hit.collider.name} | " +
                $"Parent: {(hit.collider.transform.parent ? hit.collider.transform.parent.name : "None")} | " +
                $"Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)} | " +
                $"Point: {hit.point}"
            );
        }
        else
        {
            Debug.Log("[XR DEBUG][3D] None");
        }

        if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult uiHit))
        {
            Debug.Log($"[XR DEBUG][UI] Hit: {uiHit.gameObject.name}");
        }
        else
        {
            Debug.Log("[XR DEBUG][UI] None");
        }
    }
}