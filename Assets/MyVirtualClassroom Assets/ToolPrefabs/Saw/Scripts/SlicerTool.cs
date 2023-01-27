using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SlicerTool : XRGrabInteractableObject
{
    [SerializeField]
    private InputActionProperty LeftActiveSlice, RightActiveSlice;

    private Vector3 myLastFramePosition;
    private Vector3 myCuttingExitPosition;

    protected bool ourTooIsHolding = false;
    protected bool ourIsCutting = false;

    private float myDotProduct = 0f;
    private float myForwardVelocity = 0f;
    private float myCenterEdgeDistance = 0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        myCenterEdgeDistance = (transform.position.y - GetComponent<BoxCollider>().bounds.min.y);
        DisplayOutline(TouchBy.OFF);
    }

    // Update is called once per frame
    void Update()
    {
        if(ourIsCutting)
        {
            transform.position += (transform.forward * myForwardVelocity) * Time.deltaTime;
            transform.position -= transform.up * ((myDotProduct > 0 ? 1 : 0) * (PosFrameDifference().magnitude * Time.deltaTime));

            if(HasCutThrought())
            {
                CuttingSwitch(false);
            }
        }

        if(ourIsCutting && !IsCuttingActived())
        {
            CuttingSwitch(false);
        }
    }

    private void LateUpdate()
    {
        if(ourIsCutting)
        {
            myDotProduct = DotProductForAxis(transform.forward);
            myForwardVelocity = myDotProduct * (PosFrameDifference().magnitude / Time.deltaTime);
        }
        myLastFramePosition = GetHoldingHandPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHandTag") || other.CompareTag("RightHandTag"))
        {
            DisplayOutline(TouchBy.HAND);
        }

        if(other.CompareTag("Sliceable"))
        {
            if(IsCuttingActived() && !ourIsCutting)
            {
                ActiveCutting(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftHandTag") || other.CompareTag("RightHandTag"))
        {
            DisplayOutline(TouchBy.OFF);
        }
    }

    private Vector3 PosFrameDifference()
    {
        return GetHoldingHandPosition() - myLastFramePosition;
    }

    private float DotProductForAxis(Vector3 aTransformAxis)
    {
        Vector3 dir = PosFrameDifference().normalized;

        float dot = Vector3.Dot(dir, aTransformAxis);
        if (dot < 0f)
            return -1;
        else if (dot > 0f)
            return 1;
        return 0;
    }

    private void ActiveCutting(Transform aSubstanceTransform)
    {
        CuttingSwitch(true);

        float substanceHeight = 
            aSubstanceTransform.GetComponent<BoxCollider>().bounds.max.y - 
            aSubstanceTransform.GetComponent<BoxCollider>().bounds.min.y;

        Vector3 hitPoint = aSubstanceTransform.GetComponent<BoxCollider>().bounds.ClosestPoint(transform.position);
        transform.rotation = Quaternion.Euler(Vector3.zero);

        float degreeAngle = Vector3.Angle(transform.up, aSubstanceTransform.up);
        float thickness = substanceHeight / math.cos((degreeAngle * (math.PI / 180f)));

        transform.position = hitPoint + (transform.up * myCenterEdgeDistance);
        myCuttingExitPosition = hitPoint + (-transform.up * thickness);
    }

    private void CuttingSwitch(bool isOn)
    {
        ourRigidbody.isKinematic = ourIsCutting = isOn;
        if (isOn)
            Drop();
        else
            Grab();
    }

    private bool IsCuttingActived()
    {
        if (ourIsHoldingByLeftHand)
            return LeftActiveSlice.action.IsPressed();
        if(ourIsHoldingByRightHand)
            return RightActiveSlice.action.IsPressed();
        return false;
    }

    protected override bool DisplayOutline(TouchBy aMode)
    {
        if(!base.DisplayOutline(aMode))
            return false;

        switch(aMode)
        {
            case TouchBy.HAND:
                ourOutline.OutlineMode = Outline.Mode.OutlineVisible; break;
            case TouchBy.SUBSTANCE:
                ourOutline.OutlineMode = Outline.Mode.SilhouetteOnly; break;
        }

        return true;
    }

    private bool HasCutThrought()
    {
        Vector3 edgeCurPos = (transform.position + (-transform.up * myCenterEdgeDistance));
        return Vector3.Dot((myCuttingExitPosition - edgeCurPos).normalized, -transform.up) < 0;
    }
}
