using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActiveTeleportationRay : MonoBehaviour
{
    public GameObject LeftTeleportation;
    public GameObject RightTeleportation;

    public InputActionProperty LeftActive;
    public InputActionProperty RightActive;

    public InputActionProperty LeftCancel;
    public InputActionProperty RightCancel;

    public XRRayInteractor LeftRayInteractor;
    public XRRayInteractor RightRayInteractor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isLeftRayHovering = LeftRayInteractor.TryGetHitInfo(out Vector3 leftPos, out Vector3 leftNor, out int leftNumber, out bool leftValid);
        LeftTeleportation.SetActive(!isLeftRayHovering && LeftCancel.action.ReadValue<float>() == 0 && LeftActive.action.ReadValue<float>() > .1f);


        bool isRightRayHovering = LeftRayInteractor.TryGetHitInfo(out Vector3 rightPos, out Vector3 rightNor, out int rightNumber, out bool rightValid);
        RightTeleportation.SetActive(!isRightRayHovering && RightCancel.action.ReadValue<float>() == 0 && RightActive.action.ReadValue<float>() > .1f);
    }
}
