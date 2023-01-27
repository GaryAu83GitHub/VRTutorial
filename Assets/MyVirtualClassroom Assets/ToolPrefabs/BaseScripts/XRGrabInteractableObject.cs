using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableObject : XRGrabInteractable
{
    [SerializeField]
    public Transform LeftHand, RightHand;

    [SerializeField]
    public Transform LeftAttach, RightAttach;

    [SerializeField]
    public InputActionProperty LeftGrab, RightGrab;

    [SerializeField, HideInInspector]
    protected Outline ourOutline;

    protected Rigidbody ourRigidbody;

    protected bool ourIsHoldingByLeftHand, ourIsHoldingByRightHand;

    protected enum TouchBy { OFF, HAND, TOOL, SUBSTANCE };

    protected virtual void Start()
    {
        ourRigidbody = GetComponent<Rigidbody>();

        if (GetComponent<Outline>() != null)
            ourOutline = GetComponent<Outline>();
        else if (GetComponentInChildren<Outline>() != null)
            ourOutline = GetComponentInChildren<Outline>();

        DisplayOutline(TouchBy.OFF);
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if(args.interactorObject.transform.CompareTag("LeftHandTag"))
        {
            attachTransform = LeftAttach;
            ourIsHoldingByLeftHand = true;
        }    
        else if(args.interactorObject.transform.CompareTag("RightHandTag"))
        {
            attachTransform = RightAttach;
            ourIsHoldingByRightHand = true;
        }
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        ourIsHoldingByLeftHand = ourIsHoldingByRightHand = false;
        base.OnSelectExited(args);
    }

    protected virtual bool DisplayOutline(TouchBy aMode)
    {
        if(ourOutline != null)
        {
            ourOutline.enabled = (aMode != TouchBy.OFF);
            return true;
        }
        return false;
    }

    protected Vector3 GetHoldingHandPosition()
    {
        //if (firstInteractorSelecting == null)
        //    return Vector3.zero;

        //return firstInteractorSelecting.transform.position;
        if (ourIsHoldingByLeftHand)
            return LeftHand.position;
        else if (ourIsHoldingByRightHand)
            return RightHand.position;
        return Vector3.zero;
    }
}
