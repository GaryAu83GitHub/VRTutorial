using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class SubstanceInfo
{
    public Vector3 MaxPos { get; private set; }
    public Vector3 MinPos { get; private set; }
    public Vector3 TouchNormal { get; private set; }
    public Vector3 TouchPoint { get; set; }
    public BoxHitSide TouchSide { get; set; }

    public SubstanceInfo() { }

    public SubstanceInfo(Vector3 maxPos, Vector3 minPos)
    {
        MaxPos = maxPos;
        MinPos = minPos;
        TouchPoint = Vector3.zero;
        TouchSide = BoxHitSide.NONE;
    }

    public SubstanceInfo(SubstanceInfo anInfo)
    {
        this.MaxPos = anInfo.MaxPos;
        this.MinPos = anInfo.MinPos;
        this.TouchPoint = anInfo.TouchPoint;
        this.TouchSide = anInfo.TouchSide;
    }

    public SubstanceInfo SetTouchedSide(BoxHitSide aTouchSide)
    {
        TouchSide = aTouchSide;
        TouchNormal = GetSideNormal();

        if (aTouchSide == BoxHitSide.TOP || aTouchSide == BoxHitSide.RIGHT || aTouchSide == BoxHitSide.FORWARD)
            TouchPoint = MaxPos;
        else
            TouchPoint = MinPos;
        return this;
    }

    public SubstanceInfo SetTouchedSide(BoxHitSide aTouchSide, Vector3 aTouchingPoint)
    {
        TouchSide = aTouchSide;
        TouchNormal = GetSideNormal();
        TouchPoint = SetTouchPoint(aTouchingPoint);
        return this;
    }

    public Vector3 GetFacePositionFor(Vector3 aPosition)
    {
        if (TouchSide == BoxHitSide.RIGHT)
            aPosition.x = MaxPos.x;
        else if (TouchSide == BoxHitSide.LEFT)
            aPosition.x = MinPos.x;

        else if (TouchSide == BoxHitSide.TOP)
            aPosition.y = MaxPos.y;
        else if (TouchSide == BoxHitSide.BOTTOM)
            aPosition.y = MinPos.y;

        else if (TouchSide == BoxHitSide.FORWARD)
            aPosition.z = MaxPos.z;
        else if (TouchSide == BoxHitSide.BACK)
            aPosition.z = MinPos.z;
        return aPosition;
    }

    public Vector3 GetEdgePointFor(Vector3 aPosition)
    {
        Vector3 edgePoint = aPosition;

        if (TouchSide == BoxHitSide.RIGHT)
            edgePoint = new Vector3(MaxPos.x, aPosition.y, aPosition.z);
        else if (TouchSide == BoxHitSide.LEFT)
            edgePoint = new Vector3(MinPos.x, aPosition.y, aPosition.z);

        else if (TouchSide == BoxHitSide.TOP)
            edgePoint = new Vector3(aPosition.x, MaxPos.y, aPosition.z);
        else if (TouchSide == BoxHitSide.BOTTOM)
            edgePoint = new Vector3(aPosition.x, MinPos.y, aPosition.z);

        else if (TouchSide == BoxHitSide.FORWARD)
            edgePoint = new Vector3(aPosition.x, aPosition.y, MaxPos.z);
        else if (TouchSide == BoxHitSide.BACK)
            edgePoint = new Vector3(aPosition.x, aPosition.y, MinPos.z);

        return edgePoint;
    }

    public bool IsWithinArea(Vector3 aTouchPoint)
    {
        if(TouchSide == BoxHitSide.RIGHT || TouchSide == BoxHitSide.LEFT)
        {
            if(WithinAxisBound(aTouchPoint.y, MaxPos.y, MinPos.y) && 
                WithinAxisBound(aTouchPoint.z, MaxPos.z, MinPos.z))
                return true;
        }
        else if (TouchSide == BoxHitSide.TOP || TouchSide == BoxHitSide.BOTTOM)
        {
            if (WithinAxisBound(aTouchPoint.x, MaxPos.x, MinPos.x) &&
                WithinAxisBound(aTouchPoint.z, MaxPos.z, MinPos.z))
                return true;
        }
        else if (TouchSide == BoxHitSide.FORWARD || TouchSide == BoxHitSide.BACK)
        {
            if (WithinAxisBound(aTouchPoint.x, MaxPos.x, MinPos.x) &&
                WithinAxisBound(aTouchPoint.y, MaxPos.y, MinPos.y))
                return true;
        }
        return false;
    }

    private bool WithinAxisBound(float aCurrentValue, float aMaxValue, float aMinValue)
    {
        if (aCurrentValue < aMaxValue && aCurrentValue > aMinValue)
            return true;
        return false;
    }

    private Vector3 GetSideNormal()
    {
        if (TouchSide == BoxHitSide.RIGHT)
            return Vector3.right;
        else
            if(TouchSide == BoxHitSide.LEFT)
            return Vector3.left;
        else
            if (TouchSide == BoxHitSide.TOP)
            return Vector3.up;
        else
            if(TouchSide == BoxHitSide.BOTTOM)
            return Vector3.down;
        else
            if(TouchSide == BoxHitSide.FORWARD)
            return Vector3.forward;
        else
            if(TouchSide == BoxHitSide.BACK)
            return Vector3.back;
        return Vector3.zero;
    }

    private Vector3 SetTouchPoint(Vector3 aTouchPosition)
    {
        if (TouchSide == BoxHitSide.RIGHT)
            aTouchPosition.x = MaxPos.x;
        else if (TouchSide == BoxHitSide.LEFT)
            aTouchPosition.x = MinPos.x;

        else if (TouchSide == BoxHitSide.TOP)
            aTouchPosition.y = MaxPos.y;
        else if (TouchSide == BoxHitSide.BOTTOM)
            aTouchPosition.y = MinPos.y;

        else if (TouchSide == BoxHitSide.FORWARD)
            aTouchPosition.z = MaxPos.z;
        else if (TouchSide == BoxHitSide.BACK)
            aTouchPosition.z = MinPos.z;
        return aTouchPosition;
    }
}

public class Substance : MonoBehaviour
{
    private Sliceable Sliceable;
    private MeshRenderer MeshRenderer;

    internal BoxHitSide HitSide = BoxHitSide.NONE;

    private SubstanceInfo mySubstanceInfo;

    void Start()
    {
        Sliceable = GetComponent<Sliceable>();
        MeshRenderer = GetComponent<MeshRenderer>();

        mySubstanceInfo = new SubstanceInfo(
            GetComponent<BoxCollider>().bounds.max,
            GetComponent<BoxCollider>().bounds.min);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Drawing")
        {
            other.GetComponent<MarkingTool>().StartDraw(
                this.transform, 
                mySubstanceInfo.SetTouchedSide(
                    CollideDetectTools.GetHitSide(this.gameObject, other.gameObject),
                    other.transform.position));
        }
    }

    private Vector3 GetTouchMinMaxPosition(BoxHitSide aTouchSide)
    {
        mySubstanceInfo.TouchSide = aTouchSide;

        if(HitSide == BoxHitSide.TOP || HitSide == BoxHitSide.RIGHT || HitSide == BoxHitSide.FORWARD)
            return mySubstanceInfo.MaxPos;

        return mySubstanceInfo.MinPos;
    }

    private void GetBoundaryPoint(BoxHitSide aTouchSide)
    {
        mySubstanceInfo.TouchSide = aTouchSide;

        if (aTouchSide == BoxHitSide.TOP || aTouchSide == BoxHitSide.RIGHT || aTouchSide == BoxHitSide.FORWARD)
            mySubstanceInfo.TouchPoint = mySubstanceInfo.MaxPos;
        else
            mySubstanceInfo.TouchPoint = mySubstanceInfo.MinPos;
    }
}
