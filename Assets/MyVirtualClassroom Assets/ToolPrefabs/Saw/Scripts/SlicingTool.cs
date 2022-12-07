using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static SlicingTool;

public class SlicingTool : MonoBehaviour
{
    public delegate void SlicedThrough(Transform thisTransform, Vector3 aFrontExit, Vector3 aFrontEnter, Vector3 aBackEnter);
    public static SlicedThrough OnSlicedThrough;
    public delegate void CutThrough(Transform thisTransform);
    public static CutThrough OnCutThrough;


    public delegate void HitSubstance(Vector3 aContactPoint, Vector3 aExitPoint, Vector3 aForwardVector);
    public static HitSubstance OnHitSubstance;

    [SerializeField]
    private Transform LeftHand, RightHand;

    [SerializeField]
    private Transform LeftAttach, RightAttach;

    [SerializeField]
    private InputActionProperty LeftGrab, RightGrab;

    protected bool ourIsLeftHandHolding;
    protected bool ourIsRightHandHolding;

    private bool myIsTouchingSubstance = false;

    private Vector3 myHangingPos = Vector3.zero;

    private Vector3 myLastHandPosition = Vector3.zero;
    private Vector3 mySubstanceEntPos = Vector3.zero;
    private Vector3 mySubstanceExitPos = Vector3.zero;

    private Vector3 myFrontEnter, myFrontExit, myBackEnter; 
    
    private float myDotProduct = 0f;
    private float myForwardVelocity = 0f;

    private float myCenterEdgeDistance = 0f;

    private void Awake()
    {
        XRGrabInteractabkeOnTwo.grabbingWithHand += GrabbingSlicer;
        XRGrabInteractabkeOnTwo.droppingTool += DropSlicer;
    }
    // Start is called before the first frame update
    void Start()
    {
        myCenterEdgeDistance = (transform.position.y - GetComponent<BoxCollider>().bounds.min.y);
        myHangingPos = transform.position;
    }

    private void OnDestroy()
    {
        XRGrabInteractabkeOnTwo.grabbingWithHand -= GrabbingSlicer;
        XRGrabInteractabkeOnTwo.droppingTool -= DropSlicer;
    }

    // Update is called once per frame
    void Update()
    {
        if(myIsTouchingSubstance)
        {
            transform.position += (transform.forward * myForwardVelocity) * Time.deltaTime;
            transform.position += -transform.up * (myDotProduct > 0 ? 1 : 0) * PosDifference().magnitude * Time.deltaTime;
            
            if (HadSlicedThroughSubstance())
            {
                myIsTouchingSubstance = false;
                //OnSlicedThrough?.Invoke(this.transform, myFrontExit, myFrontEnter, myBackEnter);
                OnCutThrough?.Invoke(transform);
                
                HangBack();

            }
        }
    }

    private void LateUpdate()
    {
        if (myIsTouchingSubstance)
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
        if(other.CompareTag("Sliceable") && GetComponent<XRGrabInteractabkeOnTwo>() != null)
        {
            //Debug.Log("The slice is touching " + other.transform.name);
            myIsTouchingSubstance = true;

            Destroy(GetComponent<XRGrabInteractabkeOnTwo>());
            Destroy(GetComponent<Rigidbody>());

            Vector3 max = other.transform.GetComponent<BoxCollider>().bounds.max;
            Vector3 min = other.transform.GetComponent<BoxCollider>().bounds.min;
            float substanceHeight = max.y - min.y;

            Vector3 contactPoint = mySubstanceEntPos = other.GetComponent<BoxCollider>().bounds.ClosestPoint(transform.position);
            transform.rotation = Quaternion.Euler(Vector3.zero);
            
            float degreeAngle = Vector3.Angle(transform.up, other.transform.up);
            float hypLenght = substanceHeight / math.cos((degreeAngle * (math.PI/180f)));

            myFrontEnter = mySubstanceEntPos + transform.forward * 2;
            myFrontExit = myFrontEnter + (-transform.up * (hypLenght * 2));
            myBackEnter = mySubstanceEntPos - transform.forward * 2;

            //OnHitSubstance?.Invoke(mySubstanceEntPos, (mySubstanceEntPos + (-transform.up * hypLenght)), transform.forward);

            //transform.position = new Vector3(GetGrabHandPos().x, substanceSufaceTop, GetGrabHandPos().z) + (transform.up * myCenterEdgeDistance);
            transform.position = mySubstanceEntPos + (transform.up * myCenterEdgeDistance);
            mySubstanceExitPos = mySubstanceEntPos + (-transform.up * hypLenght);
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
            ourIsLeftHandHolding = true;
        }
        else if (aGrabbingHand == UsingHand.RIGHT_HAND)
        { 
            ourIsRightHandHolding = true;
        }
    }

    private void DropSlicer()
    {
        if (myIsTouchingSubstance)
            return;
        ourIsRightHandHolding = ourIsLeftHandHolding = false;
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

        ourIsLeftHandHolding = ourIsRightHandHolding = false;

        transform.AddComponent<Rigidbody>().useGravity = false;
        transform.AddComponent<XRGrabInteractabkeOnTwo>().GetAttachTransform(LeftAttach, RightAttach);
    }
}
