using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NearFarDebug : MonoBehaviour
{
    private NearFarInteractor interactor;

    void Awake()
    {
        interactor = GetComponent<NearFarInteractor>();
    }

    void Update()
    {
        if (interactor == null)
        {
            Debug.Log("No NearFarInteractor found on " + gameObject.name);
            return;
        }

        Vector3 endPoint;
        var endType = interactor.TryGetCurveEndPoint(out endPoint);

        Debug.Log(
            $"[NearFarDebug] Object: {gameObject.name} | " +
            $"EndType: {endType} | EndPoint: {endPoint}"
        );
    }
}