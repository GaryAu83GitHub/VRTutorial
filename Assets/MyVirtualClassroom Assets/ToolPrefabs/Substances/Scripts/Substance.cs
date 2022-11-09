using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubstanceInfo
{
    public Vector3 MaxPos { get; set; }
    public Vector3 MinPos { get; set; }
    public Vector3 TouchPoint { get; set; }
    public BoxHitSide TouchSide { get; set; }

    public SubstanceInfo() { }
    public SubstanceInfo(SubstanceInfo anInfo)
    {
        this.MaxPos = anInfo.MaxPos;
        this.MinPos = anInfo.MinPos;
        this.TouchPoint = anInfo.TouchPoint;
        this.TouchSide = anInfo.TouchSide;
    }
}

public class Substance : MonoBehaviour
{
    private Sliceable Sliceable;
    private MeshRenderer MeshRenderer;

    // Start is called before the first frame update

    //private Vector3 myMaxPosition = Vector3.zero;
    //private Vector3 myMinPosition = Vector3.zero;

    internal BoxHitSide HitSide = BoxHitSide.NONE;

    private SubstanceInfo mySubstanceInfo;

    void Start()
    {
        Sliceable = GetComponent<Sliceable>();
        MeshRenderer = GetComponent<MeshRenderer>();

        mySubstanceInfo = new SubstanceInfo();
        mySubstanceInfo.MaxPos = GetComponent<BoxCollider>().bounds.max;
        mySubstanceInfo.MinPos = GetComponent<BoxCollider>().bounds.min;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Drawing")
        {
            //Vector3 touchPoint = GetTouchMinMaxPosition(CollideDetectTools.ReturnBoxHitSide(this.gameObject, other.gameObject));
            //other.GetComponent<Pen>().StartDrawOnTrigger(this.transform, touchPoint, mySubstanceInfo.TouchSide);
            GetBoundaryPoint(CollideDetectTools.ReturnBoxHitSide(this.gameObject, other.gameObject));
            other.GetComponent<Pen>().StartDrawOnTrigger(this.transform, mySubstanceInfo);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Drawing")
        {
            //other.GetComponent<Pen>().StopDrawOnTrigger(mySubstanceInfo.MaxPos, mySubstanceInfo.MinPos);
            other.GetComponent<Pen>().StopDrawOnTrigger();
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
