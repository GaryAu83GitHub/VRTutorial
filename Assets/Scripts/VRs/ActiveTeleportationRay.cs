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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LeftTeleportation.SetActive(LeftActive.action.ReadValue<float>() > .1f);
        RightTeleportation.SetActive(RightActive.action.ReadValue<float>() > .1f);
    }
}
