using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class Pen : MonoBehaviour
{
    [SerializeField]
    private Transform PenTip;

    [SerializeField]
    private GameObject MarkingLineObjct;

    [SerializeField]
    public InputActionProperty DrawingButton;

    private MarkingLine myLine;

    private bool myIsDrawing = false;

    private BoxHitSide myDrawSide = BoxHitSide.NONE;
    private Vector3 myTouchingPosition = Vector3.zero;
    private Vector3 myDrawingObjectMaxPos = Vector3.zero;
    private Vector3 myDrawingObjectMinPos = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        DrawLine();
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
        myTouchingPosition = anInfo.TouchPoint;
        myDrawSide = anInfo.TouchSide;
        myDrawingObjectMaxPos = anInfo.MaxPos;
        myDrawingObjectMinPos = anInfo.MinPos;

        myLine = Instantiate(MarkingLineObjct, GetEdgePos(), Quaternion.identity, aParentObject).GetComponent<MarkingLine>();
        myLine.StartDrawing(GetEdgePos());
        myIsDrawing = true;
    }

    //public void StartDrawOnTrigger(Transform aParentObject, Vector3 aTouchingPosition, BoxHitSide aFace)
    //{
    //    myTouchingPosition = aTouchingPosition;
    //    myDrawSide = aFace;

    //    Vector3 startPos = GetEdgePos();
    //    myLine = Instantiate(MarkingLineObjct, startPos, Quaternion.identity, aParentObject).GetComponent<MarkingLine>();
    //    myLine.StartDrawing(startPos);
    //    myIsDrawing = true;

    //}

    //public void StopDrawOnTrigger(Vector3 anObjectMaxPoint, Vector3 anObjectMinPoint)
    //{
    //    Vector3 endpos = GetEdgePos();

    //    myLine.StopDrawing(endpos);
    //    myIsDrawing = false;
    //}

    /// <summary>
    /// Put the line on the last position when this function is called and trigger of the line drawing
    /// </summary>
    public void StopDrawOnTrigger()
    {
        myLine.StopDrawing(GetEdgePos());
        myIsDrawing = false;
    }

    /// <summary>
    /// Get the current position the tool is thouching and lock the axel depending on which side of the
    /// substance bounding box the tool is touching
    /// </summary>
    /// <returns>The position with the locked axel on the touching face</returns>
    private Vector3 TipOnFacePosition()
    {
        if (myDrawSide == BoxHitSide.RIGHT || myDrawSide == BoxHitSide.LEFT)
            return new Vector3(myTouchingPosition.x, PenTip.position.y, PenTip.position.z);
        else if (myDrawSide == BoxHitSide.TOP || myDrawSide == BoxHitSide.BOTTOM)
            return new Vector3(PenTip.position.x, myTouchingPosition.y, PenTip.position.z);
        else if (myDrawSide == BoxHitSide.FORWARD || myDrawSide == BoxHitSide.BACK)
            return new Vector3(PenTip.position.x, PenTip.position.y, myTouchingPosition.z);

        return Vector3.zero;
    }

    /// <summary>
    /// Get the edge position if the line start or end outside of the substance bounding box
    /// </summary>
    /// <returns>The edge position the line either start or stop</returns>
    private Vector3 GetEdgePos()
    {
        Vector3 point = TipOnFacePosition();
        if (myDrawSide == BoxHitSide.RIGHT || myDrawSide == BoxHitSide.LEFT)
        {
            point.y = GetMaxMinValue(point.y, myDrawingObjectMaxPos.y, myDrawingObjectMinPos.y);
            point.z = GetMaxMinValue(point.z, myDrawingObjectMaxPos.z, myDrawingObjectMinPos.z);
        }
        else if (myDrawSide == BoxHitSide.TOP || myDrawSide == BoxHitSide.BOTTOM)
        {
            point.x = GetMaxMinValue(point.x, myDrawingObjectMaxPos.x, myDrawingObjectMinPos.x);
            point.z = GetMaxMinValue(point.z, myDrawingObjectMaxPos.z, myDrawingObjectMinPos.z);
        }
        else if (myDrawSide == BoxHitSide.FORWARD || myDrawSide == BoxHitSide.BACK) 
        {
            point.x = GetMaxMinValue(point.x, myDrawingObjectMaxPos.x, myDrawingObjectMinPos.x);
            point.y = GetMaxMinValue(point.y, myDrawingObjectMaxPos.y, myDrawingObjectMinPos.y);
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
}
