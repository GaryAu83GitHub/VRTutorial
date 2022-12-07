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

    public delegate void OnGrabbingToolWithHand(UsingHand grabbingHandIndex);
    public static OnGrabbingToolWithHand grabbingWithHand;

    public delegate void OnDroppingTool();
    public static OnDroppingTool droppingTool;


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHandTag"))
        {
            attachTransform = LeftAttachTransform;
            IsLeftHandGraping = true;
            grabbingWithHand?.Invoke(UsingHand.LEFT_HAND);
        }
        else if (args.interactorObject.transform.CompareTag("RightHandTag"))
        {
            attachTransform = RightAttachTransform;
            IsRightHandGraping = true;
            grabbingWithHand?.Invoke(UsingHand.RIGHT_HAND);
        }
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (IsLeftHandGraping && args.interactorObject.transform.CompareTag("LeftHandTag"))
            IsLeftHandGraping = false;
        else if (IsRightHandGraping && args.interactorObject.transform.CompareTag("RightHandTag"))
            IsRightHandGraping = false;

        droppingTool?.Invoke();

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
