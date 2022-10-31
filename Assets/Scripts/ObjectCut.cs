using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCut : MonoBehaviour
{
    /// <summary>
    /// The cutting method to destroy the cutting target and produce two seperate object
    /// This methos should be called by the cutting tools object class, ex weapon or cutting tools
    /// </summary>
    /// <param name="aTarget">The collusion object to be cut</param>
    /// <param name="aPos">The position of the cutting tool object</param>
    /// <returns></returns>
    public static bool Cut(Transform aTarget, Vector3 aPos)
    {
        Vector3 pos = new Vector3(aPos.x, aTarget.position.y, aTarget.position.z);
        Vector3 targetScale = aTarget.localScale;
        float distance = Vector3.Distance(aTarget.position, pos);
        if(distance >= targetScale.x/2)
            return false;

        Vector3 leftPoint = aTarget.position - Vector3.right * targetScale.x / 2;
        Vector3 rightPoint = aTarget.position + Vector3.right * targetScale.x / 2;
        Material mat = aTarget.GetComponent<MeshRenderer>().material;
        Destroy(aTarget.gameObject);

        GameObject rightSideObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightSideObject.transform.position = (rightPoint + pos) / 2;
        float rightWidth = Vector3.Distance(pos, rightPoint);
        rightSideObject.transform.localScale = new Vector3(rightWidth, targetScale.y, targetScale.z);
        rightSideObject.AddComponent<Rigidbody>().mass = 100f;
        rightSideObject.GetComponent<MeshRenderer>().material = mat;

        GameObject leftSideObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftSideObject.transform.position = (leftPoint + pos) / 2;
        float leftWidth = Vector3.Distance(pos, leftPoint);
        leftSideObject.transform.localScale = new Vector3(leftWidth, targetScale.y, targetScale.z);
        leftSideObject.AddComponent<Rigidbody>().mass = 100f;
        leftSideObject.GetComponent<MeshRenderer>().material = mat;

        return true;

    }
}
