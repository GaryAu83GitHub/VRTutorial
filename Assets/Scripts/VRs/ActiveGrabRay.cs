using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ActiveGrabRay : MonoBehaviour
{
    public GameObject LeftGrabRay;
    public GameObject RightGrabRay;

    public XRDirectInteractor LeftDirectGrab;
    public XRDirectInteractor RightDirectGrab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LeftGrabRay.SetActive(LeftDirectGrab.interactablesSelected.Count == 0);
        RightGrabRay.SetActive(RightDirectGrab.interactablesSelected.Count == 0);
    }
}
