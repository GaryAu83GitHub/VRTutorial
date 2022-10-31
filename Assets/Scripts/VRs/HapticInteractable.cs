using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Assets.Scripts;

public class HapticInteractable : MonoBehaviour
{
    public Haptic HapticOnActivated;
    public Haptic HapticHoverEnter;
    public Haptic HapticHoverExit;
    public Haptic HapticSelectEnter;
    public Haptic HapticSelectExit;

    // Start is called before the first frame update
    void Start()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        interactable.activated.AddListener(HapticOnActivated.TriggerHaptic);

        interactable.hoverEntered.AddListener(HapticHoverEnter.TriggerHaptic);
        interactable.hoverExited.AddListener(HapticHoverExit.TriggerHaptic);
        interactable.selectEntered.AddListener(HapticSelectEnter.TriggerHaptic);
        interactable.selectExited.AddListener(HapticSelectExit.TriggerHaptic);
    }

}
