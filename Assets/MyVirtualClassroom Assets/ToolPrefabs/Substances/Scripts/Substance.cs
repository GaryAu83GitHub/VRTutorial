using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class Substance : MonoBehaviour
{
    private Sliceable Sliceable;
    private MeshRenderer MeshRenderer;

    internal BoxHitSide HitSide = BoxHitSide.NONE;

    private SubstanceInfo mySubstanceInfo;

    private void Awake()
    {
        SlicingTool.OnSlicedThrough += OnSlicedThrough;
        SlicingTool.OnCutThrough += OnCutThrough;
    }

    private void OnDestroy()
    {
        SlicingTool.OnSlicedThrough -= OnSlicedThrough;
        SlicingTool.OnCutThrough -= OnCutThrough;
    }

    void Start()
    {
        if (gameObject.tag != "Sliceable")
            gameObject.tag = "Sliceable";

        Sliceable = GetComponent<Sliceable>();
        MeshRenderer = GetComponent<MeshRenderer>();

        if(GetComponent<BoxCollider>() == null)
            transform.AddComponent<BoxCollider>();

        mySubstanceInfo = new SubstanceInfo(
            GetComponent<BoxCollider>().bounds.max,
            GetComponent<BoxCollider>().bounds.min);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WhenSliced(int aSideStepDir)
    {
        transform.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().AddForce(aSideStepDir * transform.right * 1, ForceMode.Impulse);
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

    private void OnSlicedThrough(Transform aSlicingTool, Vector3 exitPos, Vector3 enterPos1, Vector3 enterPos2)
    {
        Vector3 rigthPartSize = Vector3Abs(aSlicingTool.position, mySubstanceInfo.MaxPos);//mySubstanceInfo.MaxPos - aSlicingTool.position;
        Vector3 leftPartSize = Vector3Abs(aSlicingTool.position, mySubstanceInfo.MinPos);//mySubstanceInfo.MinPos - aSlicingTool.position;
        SlicerSupportTools.SliceTheObject(this.gameObject, exitPos, enterPos1, enterPos2);
        //GameObject[] slicedParts = SlicerSupportTools.GetSliceParts(this.gameObject, exitPos, enterPos1, enterPos2);
        Destroy(this.gameObject);
    }

    private void OnCutThrough(Transform aSlicingTool)
    {
        SlicerSupportTools.CutObject(this.transform, aSlicingTool);
    }

    private Vector3 Vector3Abs(Vector3 v1, Vector3 v2)
    {
        return new Vector3(Mathf.Abs(v2.x - v1.x), Mathf.Abs(v2.y - v1.y), Mathf.Abs(v2.y - v1.y));
    }
}