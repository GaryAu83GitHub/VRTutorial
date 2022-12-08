using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractabkeOnTwo : XRGrabInteractable
{
    public Transform LeftAttachTransform;
    public Transform RightAttachTransform;

    public bool IsRightHandGraping { get; private set; }
    public bool IsLeftHandGraping { get; private set; }

    public delegate void GrabbingToolWithHand(UsingHand grabbingHandIndex);
    public static GrabbingToolWithHand OnGrabbingWithHand;

    public delegate void DroppingTool();
    public static DroppingTool OnDroppingTool;


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHandTag"))
        {
            attachTransform = LeftAttachTransform;
            IsLeftHandGraping = true;
            OnGrabbingWithHand?.Invoke(UsingHand.LEFT_HAND);
        }
        else if (args.interactorObject.transform.CompareTag("RightHandTag"))
        {
            attachTransform = RightAttachTransform;
            IsRightHandGraping = true;
            OnGrabbingWithHand?.Invoke(UsingHand.RIGHT_HAND);
        }
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (IsLeftHandGraping && args.interactorObject.transform.CompareTag("LeftHandTag"))
            IsLeftHandGraping = false;
        else if (IsRightHandGraping && args.interactorObject.transform.CompareTag("RightHandTag"))
            IsRightHandGraping = false;

        OnDroppingTool?.Invoke();

        base.OnSelectExited(args);
    }

    public void GetAttachTransform(Transform aLeftAttach, Transform aRightAttach)
    {
        LeftAttachTransform = aLeftAttach;
        RightAttachTransform = aRightAttach;


        interactionLayers = InteractionLayerMask.GetMask("DirectInteraction");
        movementType = MovementType.Kinematic;
    }
}
