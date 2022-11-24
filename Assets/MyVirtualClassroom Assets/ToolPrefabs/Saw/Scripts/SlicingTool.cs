using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlicingTool : MonoBehaviour
{
    [SerializeField]
    private Transform LeftHand, RightHand;

    [SerializeField]
    private Transform LeftAttach, RightAttach;

    [SerializeField]
    private InputActionProperty LeftGrab, RightGrab;

    protected bool ourIsLeftHandHolding;
    protected bool ourIsRightHandHolding;

    private Rigidbody myRB;
    private XRGrabInteractabkeOnTwo myXRGrab;

    private bool myIsTouchingSubstance = false;

    private float myLockingPosY = 0f;
    private float myLockingPosX = 0f;

    private Vector3 myLastHandPositio= Vector3.zero;
    private float myDotProduct = 0f;

    private void Awake()
    {
        XRGrabInteractabkeOnTwo.grabbingWithHand += GrabbingSlicer;
        XRGrabInteractabkeOnTwo.droppingTool += DropSlicer;
    }
    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myXRGrab = GetComponent<XRGrabInteractabkeOnTwo>();
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
            //transform.position += transform.forward * velocity
            //transform.position = new Vector3(myLockingPosX, myLockingPosY, GetGrabHandPos().z);
        }
    }

    private void LateUpdate()
    {
        // distance = vector3.Distance(hand.position, lastHandPosition)
        // velocity = distance / deltatime

        /*
            var heading = (lookObj.position - transform.position).normalized;
            var dot = Vector3.Dot(heading, transform.forward); // both are unit length
        */
        if (myIsTouchingSubstance)
        {
            Vector3 dir = (GetGrabHandPos() - myLastHandPositio).normalized;
            myDotProduct = Vector3.Dot(dir, transform.forward);
            //if (myDotProduct < 0f)
                Debug.Log(myDotProduct);

            myLastHandPositio = GetGrabHandPos();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Sliceable"))
        {
            Debug.Log("The slice is touching " + other.transform.name);
            myIsTouchingSubstance = true;

            Destroy(GetComponent<XRGrabInteractabkeOnTwo>());
            Destroy(GetComponent<Rigidbody>());

            transform.rotation = Quaternion.Euler(Vector3.zero);
            float halvHeight = (transform.position.y - GetComponent<BoxCollider>().bounds.min.y);
            float substanceSufaceTop = other.GetComponent<BoxCollider>().bounds.max.y;
            myLockingPosY = substanceSufaceTop + halvHeight;
            myLockingPosX = transform.position.x;

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
}
