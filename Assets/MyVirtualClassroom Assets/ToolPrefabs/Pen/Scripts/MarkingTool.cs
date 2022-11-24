using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum UsingHand { LEFT_HAND, RIGHT_HAND, NONE };

public class MarkingTool : MonoBehaviour
{
    // *** Hand funciton class
    [SerializeField]
    private Transform LeftHand, RightHand;

    [SerializeField]
    private InputActionProperty LeftGrabInput, RightGrabInput;

    

    protected UsingHand myHoldingHand = UsingHand.NONE;
    protected UsingHand myTouchingHand = UsingHand.NONE;

    protected bool myIsHolding = false;
    protected bool myIsTouching = false;
    protected bool myIsHoldingWithLeftHand = false;
    protected bool myIsHoldingWithRightHand = false;
    // **************************

    // *** Tool function class
    [SerializeField]
    private Transform GrabbingPoint, DrawingTip;

    [SerializeField]
    private GameObject MarkingPrefab;

    private Vector3 myHangingPosition;
    // **************************

    // *** Marking Tool class
    private SubstanceInfo myTargetData;
    private MarkingLine myLine;
    private bool myIsDrawing;
    private float myTipToCenterDist = 0f;
    // **************************

    // Start is called before the first frame update
    void Start()
    {
        myHangingPosition = transform.position;
        myTipToCenterDist = (transform.position - DrawingTip.position).magnitude;
    }

    private void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Holding();
        Moving();
    }

    void LateUpdate()
    {
        if (myIsDrawing)
        {
            if (!myTargetData.IsWithinArea(transform.position))
            {
                StopDraw();
                myIsDrawing = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!myIsTouching)
        {
            if (other.transform.tag == "LeftHandTag")
                DetermineHandTouch(UsingHand.LEFT_HAND);
            if (other.transform.tag == "RightHandTag")
                DetermineHandTouch(UsingHand.RIGHT_HAND);
        }

        if(myIsHolding)
        {
            if(other.transform.tag == "Sliceable")
                return;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "LeftHandTag" || other.transform.tag == "RightHandTag")
        {
            if (myIsTouching)
            {
                myHoldingHand = UsingHand.NONE;
                myIsTouching = false;
            }
        }
    }

    public void StartDraw(Transform aDrawingObject, SubstanceInfo anInfo)
    {
        Debug.Log(anInfo.TouchSide + " : " + transform.rotation.z.ToString());
        if (myIsDrawing)
            return;

        myTargetData = anInfo;
        FreezeAxis();

        Vector3 startPos = myTargetData.GetEdgePointFor(DrawingTip.position);
        myLine = Instantiate(MarkingPrefab, startPos, Quaternion.identity, aDrawingObject).GetComponent<MarkingLine>();
        myLine.StartDrawing(startPos);
        myIsDrawing = true;
    }

    private void OnDrawing()
    {
        if (!myIsDrawing)
            return;
        //myLine.DrawingLine(GetDrawingPosition());
        //this.transform.position = OnGetDrawingHandMotion();
    }

    public void StopDraw()
    {
        myLine.StopDrawing(DrawingTip.position);
        myIsDrawing = false;
    }

    private void Moving()
    {
        if (!myIsHolding)
            return;

        if (myIsHoldingWithLeftHand)
            Transformation(LeftHand);
        else if (myIsHoldingWithRightHand)
            Transformation(RightHand);
    }

    private void Holding()
    {
        if (myIsTouching && !myIsHolding)
        {
            if (myTouchingHand == UsingHand.LEFT_HAND && LeftGrabInput.action.IsPressed())
                ToolIsHoldingWith(UsingHand.LEFT_HAND);
            if (myTouchingHand == UsingHand.RIGHT_HAND && RightGrabInput.action.IsPressed())
                ToolIsHoldingWith(UsingHand.RIGHT_HAND);
        }

        if(myIsHolding)
        {
            if ((myIsHoldingWithLeftHand && !LeftGrabInput.action.IsPressed()) ||
                (myIsHoldingWithRightHand && !RightGrabInput.action.IsPressed()))
                HangBack();
        }
    }

    private void ToolIsHoldingWith(UsingHand anUsingHandIndex)
    {
        myHoldingHand = anUsingHandIndex;

        if (anUsingHandIndex == UsingHand.LEFT_HAND)
            myIsHolding = myIsHoldingWithLeftHand = true;
        else if (anUsingHandIndex == UsingHand.RIGHT_HAND)
            myIsHolding = myIsHoldingWithRightHand = true;

        myIsTouching = false;
    }

    private void DetermineHandTouch(UsingHand aTouchingHand)
    {
        myIsTouching = true;
        myTouchingHand = aTouchingHand;
    }

    private void HangBack()
    {
        transform.position = myHangingPosition;
        transform.eulerAngles = Vector3.zero;

        myIsHolding = myIsHoldingWithLeftHand = myIsHoldingWithRightHand = false;
        myHoldingHand = UsingHand.NONE;
    }

    private void Transformation(Transform aTransform)
    {
        if (!myIsDrawing)
        {
            transform.position = Vector3.MoveTowards(transform.position, aTransform.position, 1.05f);
            transform.rotation = Quaternion.Euler(aTransform.rotation.eulerAngles);
        }
        else
        {
            transform.position = GetDrawingPosition(aTransform);
            myLine.DrawingLine(DrawingTip.position);
        }
    }

    private Vector3 GetDrawingPosition(Transform aHoldingHandTransform)
    {
        if (myTargetData.TouchSide == BoxHitSide.RIGHT ||
            myTargetData.TouchSide == BoxHitSide.LEFT)
            return GetPositionOnAxisX(aHoldingHandTransform.position);
        else
        if (myTargetData.TouchSide == BoxHitSide.TOP ||
            myTargetData.TouchSide == BoxHitSide.BOTTOM)
            return GetPerpendicularOnAxisZ(aHoldingHandTransform.position.z);//GetPositionOnAxisY(aHoldingHandTransform.position);
        else
        if (myTargetData.TouchSide == BoxHitSide.FORWARD ||
            myTargetData.TouchSide == BoxHitSide.BACK)
            return GetPositionOnAxisZ(aHoldingHandTransform.position);

        return aHoldingHandTransform.position;
    }

    private bool IsHandOnRightSide(Vector3 aHoldingHandTransform)
    {
        return true;
    }

    private Vector3 GetPositionOnAxisX(Vector3 aHoldingHandPosition)
    {
        return new Vector3(myTargetData.TouchPoint.x, aHoldingHandPosition.y, aHoldingHandPosition.z) + PenTipOffset();
    }

    private Vector3 GetPositionOnAxisY(Vector3 aHoldingHandPosition)
    {
        return new Vector3(aHoldingHandPosition.x, myTargetData.TouchPoint.y, aHoldingHandPosition.z) + PenTipOffset();
    }

    private Vector3 GetPositionOnAxisZ(Vector3 aHoldingHandPosition)
    {
        return new Vector3(aHoldingHandPosition.x, aHoldingHandPosition.y, myTargetData.TouchPoint.z) + PenTipOffset();
    }

    private Vector3 GetPerpendicularOnAxisX(float aHand_X_Value)
    {
        return new Vector3(aHand_X_Value, myTargetData.TouchPoint.y, myTargetData.TouchPoint.z) + PenTipOffset();
    }

    private Vector3 GetPerpendicularOnAxisY(float aHand_Y_Value)
    {
        return new Vector3(myTargetData.TouchPoint.x, aHand_Y_Value, myTargetData.TouchPoint.z) + PenTipOffset();
    }

    private Vector3 GetPerpendicularOnAxisZ(float aHand_Z_Value)
    {
        return new Vector3(myTargetData.TouchPoint.x, myTargetData.TouchPoint.y, aHand_Z_Value) + PenTipOffset();
    }

    private Vector3 PenTipOffset()
    {
        return myTargetData.TouchNormal * myTipToCenterDist;
    }

    private void FreezeAxis()
    {
        if (myTargetData.TouchSide == BoxHitSide.RIGHT)
            FreezeAxisX(1, Vector3.right);
        else if (myTargetData.TouchSide == BoxHitSide.LEFT)
            FreezeAxisX(-1, Vector3.left);
        else if (myTargetData.TouchSide == BoxHitSide.TOP)
            FreezeAxisY(0, Vector3.up);
        else if (myTargetData.TouchSide == BoxHitSide.BOTTOM)
            FreezeAxisY(1, Vector3.down);
        else if (myTargetData.TouchSide == BoxHitSide.FORWARD)
            FreezeAxisZ(1, Vector3.forward);
        else if (myTargetData.TouchSide == BoxHitSide.BACK)
            FreezeAxisZ(-1, Vector3.back);
    }

    private void FreezeAxisX(int aDir, Vector3 aSideDir)
    {   
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, aDir * 90));
    }

    private void FreezeAxisY(int aDir, Vector3 aSideDir)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, aDir * 180));
    }

    private void FreezeAxisZ(int aDir, Vector3 aSideDir)
    {
        transform.rotation = Quaternion.Euler(new Vector3(aDir * 90, 0, 0));
    }
}
