using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class Substance : MonoBehaviour
{
    public Color HandTouchColor = new(96, 203, 21, 255), SlicerTouchColor = new(203, 21, 40, 255);
    public Color SubstanceTouchColor, RightAngleTouchColor;

    [SerializeField, HideInInspector] private Sliceable Sliceable;
    [SerializeField, HideInInspector] private MeshRenderer MeshRenderer;
    [SerializeField, HideInInspector] private Rigidbody myRB;
    [SerializeField, HideInInspector] private XRGrabInteractable XRGrab;

    internal BoxHitSide HitSide = BoxHitSide.NONE;

    [SerializeField, HideInInspector] public int JumpDir = 0;

    [SerializeField, HideInInspector] private SubstanceInfo mySubstanceInfo;

    [SerializeField, HideInInspector] private Outline myOutline;

    private bool mySlicerOnTouch = false;

    private enum TouchMode { HAND, SLICER, SUBSTANCE, RIGHTANGLED, NONE }

    private void Awake()
    {
        SlicingTool.OnSlicedThrough += OnSlicedThrough;
        SlicingTool.OnCutThrough += OnCutThrough;
        SlicingTool.OnSlicerTouching += OnSlicerTouching;
    }

    private void OnDestroy()
    {
        SlicingTool.OnSlicedThrough -= OnSlicedThrough;
        SlicingTool.OnCutThrough -= OnCutThrough;
        SlicingTool.OnSlicerTouching -= OnSlicerTouching;
    }

    void Start()
    {
        if (!gameObject.CompareTag("Sliceable"))
            gameObject.tag = "Sliceable";

        Sliceable = GetComponent<Sliceable>();
        MeshRenderer = GetComponent<MeshRenderer>();
        myRB = GetComponent<Rigidbody>();
        XRGrab = GetComponent<XRGrabInteractable>();

        if (GetComponent<BoxCollider>() == null)
            transform.AddComponent<BoxCollider>();

        if (GetComponent<Outline>() != null)
            myOutline = GetComponent<Outline>();
        else if (GetComponentInChildren<Outline>() != null)
            myOutline = GetComponentInChildren<Outline>();
        else
            myOutline = transform.AddComponent<Outline>();

        WhenSliced();

        myOutline.OutlineWidth = 10;

        DrawOutline(TouchMode.NONE);

        mySubstanceInfo = new SubstanceInfo(
            GetComponent<BoxCollider>().bounds.max,
            GetComponent<BoxCollider>().bounds.min);

        mySlicerOnTouch = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WhenSliced()
    {
        myRB.AddForce(1 * JumpDir * transform.right, ForceMode.Impulse);
        XRGrab.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(mySlicerOnTouch)
            return;

        if (other.CompareTag("Drawing"))
        {
            other.GetComponent<MarkingTool>().StartDraw(
                this.transform, 
                mySubstanceInfo.SetTouchedSide(
                    CollideDetectTools.GetHitSide(this.gameObject, other.gameObject),
                    other.transform.position));
        }

        if(other.CompareTag("LeftHandTag") || other.CompareTag("RightHandTag"))
        {
            DrawOutline(TouchMode.HAND);
        }

        if(other.CompareTag("Sliceable"))
            DrawOutline(TouchMode.SUBSTANCE);

        if(other.CompareTag("Slicer"))
            DrawOutline(TouchMode.SLICER);
    }

    private void OnTriggerExit(Collider other)
    {
        DrawOutline(TouchMode.NONE);
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

    private void OnSlicerTouching(string aTouchedSubstanceName)
    {
        if(aTouchedSubstanceName != transform.name)
            return;
        XRGrab.enabled = false;
        DrawOutline(TouchMode.SLICER);
        mySlicerOnTouch = true;
    }

    private Vector3 Vector3Abs(Vector3 v1, Vector3 v2)
    {
        return new Vector3(Mathf.Abs(v2.x - v1.x), Mathf.Abs(v2.y - v1.y), Mathf.Abs(v2.y - v1.y));
    }

    private void DrawOutline(TouchMode touchMode)
    {
        if (touchMode == TouchMode.NONE)
        {
            myOutline.enabled = false;
            return;
        }

        myOutline.enabled = true;
        switch(touchMode)
        {
            case TouchMode.HAND:
                myOutline.OutlineMode = Outline.Mode.OutlineVisible; 
                myOutline.OutlineColor = HandTouchColor;
                break;
            case TouchMode.SLICER:
                myOutline.OutlineMode = Outline.Mode.OutlineVisible;
                myOutline.OutlineColor = SlicerTouchColor;
                break;
            case TouchMode.SUBSTANCE:
                myOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
                myOutline.OutlineColor = SubstanceTouchColor;
                break;
            case TouchMode.RIGHTANGLED:
                myOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
                myOutline.OutlineColor = RightAngleTouchColor;
                break;
        }
    }
}