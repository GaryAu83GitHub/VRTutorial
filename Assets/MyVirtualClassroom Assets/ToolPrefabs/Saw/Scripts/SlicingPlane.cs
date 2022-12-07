using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicingPlane
{
    public Plane Plane { get; private set; }
    public Vector3 Normal { get; private set; }
    public Vector3 StartPoint { get; private set; }

    public SlicingPlane(Plane aPlane, Vector3 aNormal, Vector3 aStartPoint)
    {
        this.Plane = aPlane;
        this.Normal = aNormal;
        this.StartPoint = aStartPoint;
    }

    public SlicingPlane(SlicingPlane aPlane)
    {
        this.Plane = aPlane.Plane;
        this.Normal = aPlane.Normal;
        this.StartPoint = aPlane.StartPoint;
    }

    public static SlicingPlane CreatePlane(Transform aToolTransform, Vector3 exitPos, Vector3 enterPos1, Vector3 enterPos2)
    {
        Vector3 cuttingPlaneTopSide = exitPos - enterPos1;
        Vector3 cuttingPlaneBottomSide = exitPos - enterPos2;

        Vector3 normal = Vector3.Cross(cuttingPlaneTopSide, cuttingPlaneBottomSide).normalized;

        Vector3 transformedNormal = ((Vector3)(aToolTransform.localToWorldMatrix.transpose * normal)).normalized;
        Vector3 transformedStartingPoint = aToolTransform.InverseTransformPoint(enterPos1);

        Plane cuttingPlane = new Plane();
        cuttingPlane.SetNormalAndPosition(transformedNormal, transformedStartingPoint);

        float direciton = Vector3.Dot(Vector3.up, transformedNormal);
        if(direciton < 0)
        {
            cuttingPlane = cuttingPlane.flipped;
        }

        return new SlicingPlane(cuttingPlane, transformedNormal, transformedStartingPoint);
    }
}
