using Assets.MyVirtualClassroom_Assets.ToolPrefabs.Saw.Scripts;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SlicerSupportTools
{
    private static float FORCE_APPLIED_TO_CUT = 3f;
    public static GameObject[] Slice(GameObject aSlicingObject, Vector3 exitPos, Vector3 enterPos1, Vector3 enterPos2)
    {
        Vector3 cuttingPlaneTopSide = exitPos - enterPos1;
        Vector3 cuttingPlaneBottomSide = exitPos - enterPos2;

        Vector3 normal = Vector3.Cross(cuttingPlaneTopSide, cuttingPlaneBottomSide).normalized;

        Vector3 transformedNormal = ((Vector3)(aSlicingObject.transform.localToWorldMatrix.transpose * normal)).normalized;
        Vector3 transformedStartingPoint = aSlicingObject.transform.InverseTransformPoint(enterPos1);

        Plane cuttingPlane = new Plane();
        cuttingPlane.SetNormalAndPosition(transformedNormal, transformedStartingPoint);

        float direction = Vector3.Dot(Vector3.up, transformedNormal);
        if (direction < 0)
            cuttingPlane = cuttingPlane.flipped;

        GameObject[] slices = Slicer.Slice(cuttingPlane, aSlicingObject);
        //Destroy(aSlicingObject);

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal + Vector3.up * FORCE_APPLIED_TO_CUT;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);

        return slices;
    }
}
