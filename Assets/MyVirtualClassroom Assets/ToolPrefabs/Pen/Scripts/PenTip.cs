using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenTip : MonoBehaviour
{
    internal bool IsTipTouching = false;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.name == "Plank")
        //    Debug.Log("Trigger with " + other.name + "at " + CollideDetectTools.ReturnBoxHitSide(other.gameObject, this.gameObject));
        //IsTipTouching = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.name == "Plank")
            //IsTipTouching = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.down * .0001f;
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.up * .0001f;
    }
}
