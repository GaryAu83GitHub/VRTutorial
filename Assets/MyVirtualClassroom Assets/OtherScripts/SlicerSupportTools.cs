using Assets.MyVirtualClassroom_Assets.ToolPrefabs.Saw.Scripts;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SlicerSupportTools : MonoBehaviour
{
    private static float FORCE_APPLIED_TO_CUT = 1f;
    public static int SLICED_PART_COUNT = 0;
    public static GameObject[] GetSliceParts(GameObject aSlicingObject, Vector3 exitPos, Vector3 enterPos1, Vector3 enterPos2)
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

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal + Vector3.up * FORCE_APPLIED_TO_CUT;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);

        return slices;
    }

    public static GameObject[] GetSliceParts(GameObject aSlicingObject, SlicingPlane aCuttingPlane)
    {
        GameObject[] slices = Slicer.Slice(aCuttingPlane.Plane, aSlicingObject);

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = aCuttingPlane.Normal - Vector3.right * FORCE_APPLIED_TO_CUT;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);

        return slices;
    }

    public static void SliceTheObject(GameObject aSlicingObject, Vector3 exitPos, Vector3 enterPos1, Vector3 enterPos2)
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

        for(int i = 0; i < slices.Length; i++)
        {
            slices[i].AddComponent<Substance>();
        }
        
        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal - Vector3.right * FORCE_APPLIED_TO_CUT;
        rigidbody.AddForce(newNormal, ForceMode.VelocityChange);
    }

    public static void CutObject(Transform substance, Transform slicer)
    {
        Vector3 hitPos = new Vector3(slicer.position.x, substance.position.y, substance.position.z);
        Vector3 substanceScale = substance.localScale;
        float distance = Vector3.Distance(substance.position, hitPos);
        if(distance >= substanceScale.x * .5f)
            return;

        Vector3 rightPoint = substance.position + Vector3.right * substanceScale.x * .5f;
        float rightWidth = Vector3.Distance(hitPos, rightPoint);
        Vector3 leftPoint = substance.position - Vector3.right * substanceScale.x * .5f;
        float leftWidth = Vector3.Distance(hitPos, leftPoint);

        Material mat = substance.GetComponent<MeshRenderer>().material;
        
        GameObject rightObj = Instantiate(substance.gameObject);
        rightObj.transform.name = "SlicedPart_" + (SlicerSupportTools.SLICED_PART_COUNT);
        rightObj.transform.position = (rightPoint + hitPos) * .5f;
        rightObj.transform.localScale = new Vector3(rightWidth, substanceScale.y, substanceScale.z);
        rightObj.GetComponent<Substance>().JumpDir = 1;

        GameObject leftObj = Instantiate(substance.gameObject);
        leftObj.transform.name = "SlicedPart_" + (SlicerSupportTools.SLICED_PART_COUNT + 1);
        leftObj.transform.position = (leftPoint + hitPos) * .5f;
        leftObj.transform.localScale = new Vector3(leftWidth, substanceScale.y, substanceScale.z);
        leftObj.GetComponent<Substance>().JumpDir = -1;

        Destroy(substance.gameObject);

        //GameObject rightObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //rightObj.transform.name = "SlicedPart_" + (SlicerSupportTools.SLICED_PART_COUNT);
        //rightObj.transform.position = (rightPoint + hitPos) * .5f;
        //rightObj.transform.localScale = new Vector3(rightWidth, substanceScale.y, substanceScale.z);
        //if(rightObj.GetComponent<MeshRenderer>() == null)
        //    rightObj.AddComponent<MeshRenderer>().material = mat;
        //else
        //    rightObj.GetComponent<MeshRenderer>().material = mat;
        //rightObj.AddComponent<Substance>();
        //rightObj.GetComponent<Substance>().WhenSliced(1);
        ////rightObj.AddComponent<myRB>();

        //GameObject leftObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //leftObj.transform.name = "SlicedPart_" + (SlicerSupportTools.SLICED_PART_COUNT + 1);
        //leftObj.transform.position = (leftPoint + hitPos) * .5f;
        //leftObj.transform.localScale = new Vector3(leftWidth, substanceScale.y, substanceScale.z);
        //if (leftObj.GetComponent<MeshRenderer>() == null)
        //    leftObj.AddComponent<MeshRenderer>().material = mat;
        //else
        //    leftObj.GetComponent<MeshRenderer>().material = mat;
        //leftObj.AddComponent<Substance>();
        //leftObj.GetComponent<Substance>().WhenSliced(-1);
        ////leftObj.AddComponent<myRB>();

        SlicerSupportTools.SLICED_PART_COUNT += 2;
    }
}
