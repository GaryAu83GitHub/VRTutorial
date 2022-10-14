using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatedHandInput : MonoBehaviour
{
    public InputActionProperty PinchAnimationAction;
    public InputActionProperty GripAnimationAction;

    public Animator HandAnimator;

    //private float myTrigValue = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float trigValue = PinchAnimationAction.action.ReadValue<float>();
        HandAnimator.SetFloat("Trigger", trigValue);
        
        float gripValue = GripAnimationAction.action.ReadValue<float>();
        HandAnimator.SetFloat("Grip", gripValue);
    }
}
