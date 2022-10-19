using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractabkeOnTwo : XRGrabInteractable
{
    public Transform LeftAttachTransform;
    public Transform RightAttachTransform;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("LeftHandTag"))
            attachTransform = LeftAttachTransform;
        else if(args.interactorObject.transform.CompareTag("RightHandTag"))
            attachTransform = RightAttachTransform;

        base.OnSelectEntered(args);
    }
}
