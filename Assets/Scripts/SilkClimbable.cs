using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SilkClimbable : XRBaseInteractable
{
    private Transform _xrRig;
    private Dictionary<IXRSelectInteractor, Vector3> _grabPositions = new();

    protected override void Awake()
    {
        base.Awake();

        var rigObj = GameObject.FindWithTag("XRRig");
        if (rigObj != null)
            _xrRig = rigObj.transform;
        else
            Debug.LogError("SilkClimbable: Could not find GameObject tagged 'XRRig'!");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _grabPositions[args.interactorObject] = 
            args.interactorObject.GetAttachTransform(this).position;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _grabPositions.Remove(args.interactorObject);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic) return;
        if (_xrRig == null || _grabPositions.Count == 0) return;

        var interactors = new List<IXRSelectInteractor>(_grabPositions.Keys);

        foreach (var interactor in interactors)
        {
            var previousPos = _grabPositions[interactor];
            var currentPos = interactor.GetAttachTransform(this).position;

            Vector3 delta = previousPos - currentPos;
            _xrRig.position += delta;

            _grabPositions[interactor] = currentPos;
        }
    }
}