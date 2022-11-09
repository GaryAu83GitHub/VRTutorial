using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is to be attach to object that avaiable to be sliced
/// The SlicingProgress represent like a "health point" value for the object
/// It'll be reducing when a slicing object where doing a slicing progress
/// and when the value fall below 0, the static fuction will be called and divid
/// this object into two indivudual objects and destroy this object
/// </summary>
public class SliceableObject : MonoBehaviour
{
    private float mySlicingProgress = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.transform.CompareTag("Slicer"))
            return;
    }
}
