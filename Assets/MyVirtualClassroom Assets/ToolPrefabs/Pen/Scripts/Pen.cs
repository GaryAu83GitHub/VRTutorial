using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class Pen : MonoBehaviour
{
    [SerializeField]
    private Transform PenTip, LeftHand, RightHand;

    [SerializeField]
    private GameObject MarkingLineObjct;

    [SerializeField]
    public InputActionProperty DrawingButton;

    private MarkingLine myLine;
    private Rigidbody myRigidbody;
    private Transform myGrabHandTransform;

    private bool myIsDrawing = false;
    private bool myTriggerGrabInteraction = false;

    private Vector3 tempPosition;
    private bool tempEnableXRGrab = false;

    private Vector3 tempHangingPos = Vector3.zero;

    private float myLenghtBetweenCenterAndTip = 0f;

    private SubstanceInfo myDrawObjectInfo;

    private void Start()
    {
        tempHangingPos = transform.position;
        tempEnableXRGrab = GetComponent<XRGrabInteractable>().enabled;

        myLenghtBetweenCenterAndTip = (transform.position - PenTip.position).magnitude;
        myRigidbody = GetComponent<Rigidbody>();

    }

    private void OnTriggerEnter(Collider other)
    {
      

    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
        //TriggerXRGrab(DrawingButton.action.IsPressed());
    }

    /// <summary>
    /// The basic draw funciton that us input to trigger it.
    /// When the button for line drawing is pressed it'll toggle the boolian for is drawing.
    /// The line will be instantiated and set the start position for the line.
    /// As long the button is pressed when the line is instantiated, the end of the line will
    /// keep tailing after the tip of the pen until the button is released.
    /// </summary>
    /// <param name="aDrawButtonPressed">Boolian for the button for drawing is pressed</param>
    /// <param name="aTipPosition">The position of the tip transformation</param>
    private void DrawLineWithInput(bool aDrawButtonPressed, Vector3 aTipPosition)
    {
        if(aDrawButtonPressed)
        {
            if (!myIsDrawing)
            {
                myLine = Instantiate(MarkingLineObjct, aTipPosition, Quaternion.identity).GetComponent<MarkingLine>();
                myLine.StartDrawing(aTipPosition);
                myIsDrawing = true;
            }
            else
                myLine.DrawingLine(aTipPosition);
        }
        else
        {
            if(myIsDrawing)
            {
                myLine.StopDrawing(aTipPosition);
                myIsDrawing = false;
            }
        }
    }

    /// <summary>
    /// Navigation to the lines end point as long the trigger is still true
    /// </summary>
    private void DrawLine()
    {
        if (!myIsDrawing)
            return;

        transform.position = RightHand.position;
        Debug.Log("Pen Position: " + transform.position.ToString());
        Debug.Log("Hand Position: " + RightHand.position.ToString());

        myLine.DrawingLine(TipOnFacePosition());
    }

    /// <summary>
    /// Called to start drawing a line with the given info from the collide triggered substance and trigger true to
    /// the drawing drawing trigger
    /// The new created line will be put as a child object under the substance
    /// </summary>
    /// <param name="aParentObject">The substance transform as the parent transform for the new created line</param>
    /// <param name="anInfo">The info of the collided substance</param>
    public void StartDrawOnTrigger(Transform aParentObject, SubstanceInfo anInfo)
    {
        myDrawObjectInfo = new SubstanceInfo(anInfo);


        FreezeAxis(myDrawObjectInfo.TouchSide);

        myLine = Instantiate(MarkingLineObjct, GetEdgePos(), Quaternion.identity, aParentObject).GetComponent<MarkingLine>();
        myLine.StartDrawing(GetEdgePos());
        myIsDrawing = true;
    }

    /// <summary>
    /// Put the line on the last position when this function is called and trigger of the line drawing
    /// </summary>
    public void StopDrawOnTrigger()
    {
        myLine.StopDrawing(GetEdgePos());
        myRigidbody.isKinematic = false;
        myIsDrawing = false;
    }

    /// <summary>
    /// Get the current position the tool is thouching and lock the axel depending on which side of the
    /// substance bounding box the tool is touching
    /// </summary>
    /// <returns>The position with the locked axel on the touching face</returns>
    private Vector3 TipOnFacePosition()
    {
        if (myDrawObjectInfo.TouchSide == BoxHitSide.RIGHT || myDrawObjectInfo.TouchSide == BoxHitSide.LEFT)
            return new Vector3(myDrawObjectInfo.TouchPoint.x, PenTip.position.y, PenTip.position.z);
        else if (myDrawObjectInfo.TouchSide == BoxHitSide.TOP || myDrawObjectInfo.TouchSide == BoxHitSide.BOTTOM)
            return new Vector3(PenTip.position.x, myDrawObjectInfo.TouchPoint.y, PenTip.position.z);
        else if (myDrawObjectInfo.TouchSide == BoxHitSide.FORWARD || myDrawObjectInfo.TouchSide == BoxHitSide.BACK)
            return new Vector3(PenTip.position.x, PenTip.position.y, myDrawObjectInfo.TouchPoint.z);

        return Vector3.zero;
    }

    /// <summary>
    /// Get the edge position if the line start or end outside of the substance bounding box
    /// </summary>
    /// <returns>The edge position the line either start or stop</returns>
    private Vector3 GetEdgePos()
    {
        Vector3 point = TipOnFacePosition();

        if (myDrawObjectInfo.TouchSide == BoxHitSide.RIGHT || myDrawObjectInfo.TouchSide == BoxHitSide.LEFT)
        {
            point.y = GetMaxMinValue(point.y, myDrawObjectInfo.MaxPos.y, myDrawObjectInfo.MinPos.y);
            point.z = GetMaxMinValue(point.z, myDrawObjectInfo.MaxPos.z, myDrawObjectInfo.MinPos.z);
        }
        else if (myDrawObjectInfo.TouchSide == BoxHitSide.TOP || myDrawObjectInfo.TouchSide == BoxHitSide.BOTTOM)
        {
            point.x = GetMaxMinValue(point.x, myDrawObjectInfo.MaxPos.x, myDrawObjectInfo.MinPos.x);
            point.z = GetMaxMinValue(point.z, myDrawObjectInfo.MaxPos.z, myDrawObjectInfo.MinPos.z);
        }
        else if (myDrawObjectInfo.TouchSide == BoxHitSide.FORWARD || myDrawObjectInfo.TouchSide == BoxHitSide.BACK) 
        {
            point.x = GetMaxMinValue(point.x, myDrawObjectInfo.MaxPos.x, myDrawObjectInfo.MinPos.x);
            point.y = GetMaxMinValue(point.y, myDrawObjectInfo.MaxPos.y, myDrawObjectInfo.MinPos.y);
        }
        return point;
    }

    /// <summary>
    /// Get the value of which edge axel value for the line's points. This is to determine if the
    /// line's point is within the boundare of the touching side of the substance
    /// </summary>
    /// <param name="aCompareValue">The value of the original points to be compared with teh axel values</param>
    /// <param name="aMaxValue">The substance bounding box max axel values</param>
    /// <param name="aMinValue">The substance bounding box min axel values</param>
    /// <returns>If the return value is the original, the point is within the boundare of the touching side
    /// and if not, mean that the point was outside the boundare and will be given the edge value</returns>
    private float GetMaxMinValue(float aCompareValue, float aMaxValue, float aMinValue)
    {
        if(aCompareValue > aMaxValue)
            return aMaxValue;
        else if(aCompareValue < aMinValue)
            return aMinValue;
        return aCompareValue;
    }

    private bool WithinSubstanceBoundary(Vector3 aTipPosition)
    {
        if (myDrawObjectInfo.TouchSide == BoxHitSide.RIGHT || myDrawObjectInfo.TouchSide == BoxHitSide.LEFT)
        {
            if(WithinBoundaryValue(aTipPosition.y, myDrawObjectInfo.MaxPos.y, myDrawObjectInfo.MinPos.y) ||
                WithinBoundaryValue(aTipPosition.z, myDrawObjectInfo.MaxPos.z, myDrawObjectInfo.MinPos.z))
                return true;
        }
        else if (myDrawObjectInfo.TouchSide == BoxHitSide.TOP || myDrawObjectInfo.TouchSide == BoxHitSide.BOTTOM)
        {
            if (WithinBoundaryValue(aTipPosition.x, myDrawObjectInfo.MaxPos.x, myDrawObjectInfo.MinPos.x) ||
                    WithinBoundaryValue(aTipPosition.z, myDrawObjectInfo.MaxPos.z, myDrawObjectInfo.MinPos.z))
                return true;
        }
        else if (myDrawObjectInfo.TouchSide == BoxHitSide.FORWARD || myDrawObjectInfo.TouchSide == BoxHitSide.BACK)
        {
            if (WithinBoundaryValue(aTipPosition.x, myDrawObjectInfo.MaxPos.x, myDrawObjectInfo.MinPos.x) ||
                    WithinBoundaryValue(aTipPosition.y, myDrawObjectInfo.MaxPos.y, myDrawObjectInfo.MinPos.y))
                return true;
        }
        return false;
    }

    private bool WithinBoundaryValue(float aCurrentValue, float aMaxValue, float aMinValue)
    {
        if(aCurrentValue > aMaxValue || aCurrentValue < aMinValue)
            return true;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isPressed"></param>
    private void TriggerXRGrab(bool isPressed)
    {
        if(isPressed && !myTriggerGrabInteraction)
        {
            //Debug.Log(GetComponent<XRGrabInteractable>().GetOldestInteractorSelecting().transform.name);
            myGrabHandTransform = GetComponent<XRGrabInteractable>().GetOldestInteractorSelecting().transform;

            tempEnableXRGrab = GetComponent<XRGrabInteractable>().enabled = !GetComponent<XRGrabInteractable>().enabled;
            //Debug.Log("XR Grap enable " + (GetComponent<XRGrabInteractable>().enabled ? "On" : "Off").ToString());
            myTriggerGrabInteraction = isPressed;
        }
        else if(myTriggerGrabInteraction && !isPressed)
            myTriggerGrabInteraction = false;

        if (!tempEnableXRGrab)
        {
            tempPosition = transform.position;
        }
    }

    private void FreezeAxis(BoxHitSide aHitSide)
    {
        if (aHitSide == BoxHitSide.RIGHT || aHitSide == BoxHitSide.LEFT)
        {
            if (aHitSide == BoxHitSide.RIGHT)
                this.transform.eulerAngles = new Vector3(0, 0, 90);
            else if (aHitSide == BoxHitSide.LEFT)
                this.transform.eulerAngles = new Vector3(0, 0, -90);
            FreezeAxisX();
        }
        else if (aHitSide == BoxHitSide.TOP || aHitSide == BoxHitSide.BOTTOM)
        {
            if (aHitSide == BoxHitSide.TOP)
                this.transform.eulerAngles = new Vector3(0, 0, 0);
            else if (aHitSide == BoxHitSide.BOTTOM)
                this.transform.eulerAngles = new Vector3(0, 0, 180);
            FreezeAxisY();
        }
        else if (aHitSide == BoxHitSide.FORWARD || aHitSide == BoxHitSide.BACK)
        {
            if (aHitSide == BoxHitSide.FORWARD)
                this.transform.eulerAngles = new Vector3(90, 0, 0);
            else if (aHitSide == BoxHitSide.BACK)
                this.transform.eulerAngles = new Vector3(-90, 0, 0);
            FreezeAxisZ();
        }
        TriggerXRGrab(true);
    }

    private void FreezeAxisX()
    {
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
    }

    private void FreezeAxisY()
    {
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    private void FreezeAxisZ()
    {
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    private void UnFreezeAxis()
    {
        myRigidbody.constraints = RigidbodyConstraints.None;
    }

    private void PlaceDrawingPose(Vector3 aRotation, Vector3 aLocation)
    {
        this.transform.eulerAngles = aRotation;
        this.transform.position = myDrawObjectInfo.TouchPoint + (aLocation * myLenghtBetweenCenterAndTip);
    }
}
