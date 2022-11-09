using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.MyVirtualClassroom_Assets.ToolPrefabs.Saw.Scripts;
using UnityEngine.Rendering;

public class Saw : MonoBehaviour
{
    [SerializeField]
    private Transform SawBladeTopTip = null;

    [SerializeField]
    private Transform SawBladeBottonTip = null;

    private Vector3 mySawTopTipEnterPosition = Vector3.zero;
    private Vector3 mySawTopTipExitPosition = Vector3.zero;

    private Vector3 mySawBottonTipEnterPosition = Vector3.zero;
    private Vector3 mySawBottonTipExitPosition = Vector3.zero;

    private const float FORCE_APPLIED_TO_CUT = 3f;

    private bool myBladeTouchObject = false;
    private float myStopHeigh = 0f;
    private void Update()
    {
        if (myBladeTouchObject) 
        {
            Vector3 parentPosition = transform.parent.transform.localPosition;
            transform.parent.transform.localPosition = new Vector3(parentPosition.x, myStopHeigh, parentPosition.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Sliceable"))
            return;

        //transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
        other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        myBladeTouchObject = true;
        myStopHeigh = transform.parent.transform.localPosition.y;

        mySawTopTipEnterPosition = SawBladeTopTip.position;
        mySawBottonTipEnterPosition = SawBladeBottonTip.position;
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Sliceable"))
            return;

        myBladeTouchObject = false;
        mySawTopTipExitPosition = SawBladeTopTip.position;
        //mySawBottonTipExitPosition = SawBladeBottonTip.position;

        Slice(other.gameObject);
    }

    private void Slice(GameObject aSlicingObject)
    {
        Vector3 cuttingPlaneTopSide = mySawTopTipExitPosition - mySawTopTipEnterPosition;
        Vector3 cuttingPlaneBottomSide = mySawTopTipExitPosition - mySawBottonTipEnterPosition;

        Vector3 normal = Vector3.Cross(cuttingPlaneTopSide, cuttingPlaneBottomSide).normalized;

        Vector3 transformedNormal = ((Vector3)(aSlicingObject.transform.localToWorldMatrix.transpose * normal)).normalized;
        Vector3 transformedStartingPoint = aSlicingObject.transform.InverseTransformPoint(mySawTopTipEnterPosition);

        Plane cuttingPlane = new Plane();
        cuttingPlane.SetNormalAndPosition(transformedNormal, transformedStartingPoint);

        float direction = Vector3.Dot(Vector3.up, transformedNormal);
        if (direction < 0)
            cuttingPlane = cuttingPlane.flipped;

        GameObject[] slices = Slicer.Slice(cuttingPlane, aSlicingObject);
        Destroy(aSlicingObject);

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal + Vector3.up * FORCE_APPLIED_TO_CUT;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);
    }
}
