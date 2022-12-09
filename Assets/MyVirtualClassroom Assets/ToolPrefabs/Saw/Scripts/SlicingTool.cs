using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlicingTool : MonoBehaviour
{
    public delegate void SlicedThrough(Transform thisTransform, Vector3 aFrontExit, Vector3 aFrontEnter, Vector3 aBackEnter);
    public static SlicedThrough OnSlicedThrough;
    public delegate void CutThrough(Transform thisTransform);
    public static CutThrough OnCutThrough;
    public delegate void SlicerTouching(string aTouchedSubstanceName);
    public static SlicerTouching OnSlicerTouching;


    public delegate void HitSubstance(Vector3 aContactPoint, Vector3 aExitPoint, Vector3 aForwardVector);
    public static HitSubstance OnHitSubstance;

    [SerializeField]
    private Transform LeftHand, RightHand;

    [SerializeField]
    private Transform LeftAttach, RightAttach;

    [SerializeField]
    private InputActionProperty LeftGrab, RightGrab, LeftActiveSlice, RightActiveSlice;

    protected bool ourIsLeftHandHolding;
    protected bool ourIsRightHandHolding;
    protected bool ourToolIsHolded = false;

    private bool myIsTouchingSubstance = false;
    private bool myIsCutting = false;

    private Vector3 myHangingPos = Vector3.zero;

    private Vector3 myLastHandPosition = Vector3.zero;
    private Vector3 mySubstanceEntPos = Vector3.zero;
    private Vector3 mySubstanceExitPos = Vector3.zero;

    private Vector3 myFrontEnter, myFrontExit, myBackEnter; 
    
    private float myDotProduct = 0f;
    private float myForwardVelocity = 0f;

    private float myCenterEdgeDistance = 0f;

    private enum TouchMode { HAND, SUBSTANCE, NONE }
    private Outline myOutline;

    private void Awake()
    {
        XRGrabInteractabkeOnTwo.OnGrabbingWithHand += GrabbingSlicer;
        XRGrabInteractabkeOnTwo.OnDroppingTool += DropSlicer;
    }
    // Start is called before the first frame update
    void Start()
    {
        myCenterEdgeDistance = (transform.position.y - GetComponent<BoxCollider>().bounds.min.y);
        myHangingPos = transform.position;

        if(GetComponent<Outline>() != null)
            myOutline = GetComponent<Outline>();
        else if (GetComponentInChildren<Outline>() != null)
        {
            myOutline = GetComponentInChildren<Outline>();
        }
        DrawOutline(TouchMode.NONE);
    }

    private void OnDestroy()
    {
        XRGrabInteractabkeOnTwo.OnGrabbingWithHand -= GrabbingSlicer;
        XRGrabInteractabkeOnTwo.OnDroppingTool -= DropSlicer;
    }

    // Update is called once per frame
    void Update()
    {
        if(myIsCutting/*myIsTouchingSubstance*/)
        {
            transform.position += (transform.forward * myForwardVelocity) * Time.deltaTime;
            transform.position += -transform.up * (myDotProduct > 0 ? 1 : 0) * PosDifference().magnitude * Time.deltaTime;
            
            if (HadSlicedThroughSubstance())
            {
                /*myIsTouchingSubstance*/myIsCutting = false;
                //OnSlicedThrough?.Invoke(this.transform, myFrontExit, myFrontEnter, myBackEnter);
                OnCutThrough?.Invoke(transform);
                //GetComponent<Rigidbody>().isKinematic = false;
                HangBack();

            }
        }
    }

    private void LateUpdate()
    {
        if (myIsCutting/*myIsTouchingSubstance*/)
        {
            myDotProduct = DotProductForAxis(transform.forward);
            myForwardVelocity = (PosDifference().magnitude / Time.deltaTime) * myDotProduct;

        }
        myLastHandPosition = GetGrabHandPos();
    }

    private Vector3 PosDifference()
    {
        return GetGrabHandPos() - myLastHandPosition;
    }

    private float DotProductForAxis(Vector3 aTransformAxis)
    {
        Vector3 dir = PosDifference().normalized;
        float dot = Vector3.Dot(dir, aTransformAxis);
        if (dot < 0)
            return -1f;
        else if (dot > 0)
            return 1f;

        return 0;
    }

    private Vector3 GetBladeEdge()
    {
        return (transform.position + (-transform.up * myCenterEdgeDistance));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Sliceable") && !myIsCutting/*!myIsTouchingSubstanceGetComponent<XRGrabInteractabkeOnTwo>() != null*/)
        {
            //myIsTouchingSubstance = true;

            //Destroy(GetComponent<XRGrabInteractabkeOnTwo>());
            //Destroy(GetComponent<Rigidbody>());

            ////GetComponent<Rigidbody>().isKinematic = true;

            //Vector3 max = other.transform.GetComponent<BoxCollider>().bounds.max;
            //Vector3 min = other.transform.GetComponent<BoxCollider>().bounds.min;
            //float substanceHeight = max.y - min.y;

            //mySubstanceEntPos = other.GetComponent<BoxCollider>().bounds.ClosestPoint(transform.position);
            //transform.rotation = Quaternion.Euler(Vector3.zero);

            //float degreeAngle = Vector3.Angle(transform.up, other.transform.up);
            //float hypLenght = substanceHeight / math.cos((degreeAngle * (math.PI/180f)));

            //myFrontEnter = mySubstanceEntPos + transform.forward * 2;
            //myFrontExit = myFrontEnter + (-transform.up * (hypLenght * 2));
            //myBackEnter = mySubstanceEntPos - transform.forward * 2;

            //transform.position = mySubstanceEntPos + (transform.up * myCenterEdgeDistance);
            //mySubstanceExitPos = mySubstanceEntPos + (-transform.up * hypLenght);

            ////OnHitSubstance?.Invoke(mySubstanceEntPos, (mySubstanceEntPos + (-transform.up * hypLenght)), transform.forward);
            OnSlicerTouching?.Invoke(other.name);
            //DrawOutline(TouchMode.SUBSTANCE);

            if(ActiveCutting())
                SetCuttingPos(other.transform);

        }

        if((other.CompareTag("LeftHandTag") || other.CompareTag("RightHandTag")))
        {
            if(!ourToolIsHolded)
                DrawOutline(TouchMode.HAND);
        }
    }

    private void SetCuttingPos(Transform aTransform)
    {
        /*myIsTouchingSubstance*/myIsCutting = true;

        Destroy(GetComponent<XRGrabInteractabkeOnTwo>());
        Destroy(GetComponent<Rigidbody>());

        //GetComponent<Rigidbody>().isKinematic = true;

        Vector3 max = aTransform.GetComponent<BoxCollider>().bounds.max;
        Vector3 min = aTransform.GetComponent<BoxCollider>().bounds.min;
        float substanceHeight = max.y - min.y;

        mySubstanceEntPos = aTransform.GetComponent<BoxCollider>().bounds.ClosestPoint(transform.position);
        transform.rotation = Quaternion.Euler(Vector3.zero);

        float degreeAngle = Vector3.Angle(transform.up, aTransform.up);
        float hypLenght = substanceHeight / math.cos((degreeAngle * (math.PI / 180f)));

        myFrontEnter = mySubstanceEntPos + transform.forward * 2;
        myFrontExit = myFrontEnter + (-transform.up * (hypLenght * 2));
        myBackEnter = mySubstanceEntPos - transform.forward * 2;

        transform.position = mySubstanceEntPos + (transform.up * myCenterEdgeDistance);
        mySubstanceExitPos = mySubstanceEntPos + (-transform.up * hypLenght);

        //OnHitSubstance?.Invoke(mySubstanceEntPos, (mySubstanceEntPos + (-transform.up * hypLenght)), transform.forward);
        //OnSlicerTouching?.Invoke(aTransform.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftHandTag") || other.CompareTag("RightHandTag"))
        {
            DrawOutline(TouchMode.NONE);
        }
    }

    private Vector3 GetGrabHandPos()
    {
        Vector3 handPosition = Vector3.zero;
        if (ourIsLeftHandHolding)
            handPosition = LeftHand.position;
        else if(ourIsRightHandHolding)
            handPosition = RightHand.position;

        return handPosition;
    }

    private void GrabbingSlicer(UsingHand aGrabbingHand)
    {
        if (aGrabbingHand == UsingHand.LEFT_HAND)
        {
            ourToolIsHolded = ourIsLeftHandHolding = true;
        }
        else if (aGrabbingHand == UsingHand.RIGHT_HAND)
        {
            ourToolIsHolded = ourIsRightHandHolding = true;
        }

        DrawOutline(0);
    }

    private bool ActiveCutting()
    {
        if (ourIsLeftHandHolding)
            return LeftActiveSlice.action.IsPressed();
        if(ourIsRightHandHolding)
            return RightActiveSlice.action.IsPressed();
        return false;
    }

    private void DropSlicer()
    {
        if (myIsCutting/*myIsTouchingSubstance*/)
            return;
        ourToolIsHolded = ourIsRightHandHolding = ourIsLeftHandHolding = false;
    }

    private bool HadSlicedThroughSubstance()
    {
        Vector3 edgePos = (transform.position + (-transform.up * myCenterEdgeDistance));;
        return Vector3.Dot((mySubstanceExitPos - edgePos).normalized, -transform.up) < 0;
    }

    private void HangBack()
    {
        transform.position = myHangingPos;
        transform.eulerAngles = new Vector3(0,90,0);

        ourToolIsHolded = ourIsLeftHandHolding = ourIsRightHandHolding = false;


        transform.AddComponent<Rigidbody>().useGravity = false;
        transform.AddComponent<XRGrabInteractabkeOnTwo>().GetAttachTransform(LeftAttach, RightAttach);

        DrawOutline(TouchMode.NONE);
    }

    private void DrawOutline(TouchMode aMode)
    {
        if (aMode == TouchMode.NONE)
        {
            myOutline.enabled = false;
            return;
        }

        myOutline.enabled = true;
        switch (aMode)
        {
            case TouchMode.HAND:
                myOutline.OutlineMode = Outline.Mode.OutlineVisible; break;
            case TouchMode.SUBSTANCE:
                myOutline.OutlineMode = Outline.Mode.SilhouetteOnly; break;
        }
    }
}
