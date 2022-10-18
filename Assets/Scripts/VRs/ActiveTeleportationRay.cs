using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveTeleportationRay : MonoBehaviour
{
    public GameObject LeftTeleportation;
    public GameObject RightTeleportation;

    public InputActionProperty LeftActive;
    public InputActionProperty RightActive;

    public InputActionProperty LeftCancel;
    public InputActionProperty RightCancel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LeftTeleportation.SetActive(LeftCancel.action.ReadValue<float>() == 0 && LeftActive.action.ReadValue<float>() > .1f);
        RightTeleportation.SetActive(RightCancel.action.ReadValue<float>() == 0 && RightActive.action.ReadValue<float>() > .1f);
    }
}
