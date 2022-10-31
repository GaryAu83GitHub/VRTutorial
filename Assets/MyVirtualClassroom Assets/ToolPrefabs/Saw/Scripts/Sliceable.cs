using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private bool _isSolid = true;

    [SerializeField]
    private bool _reverseWindTriangles = false;

    [SerializeField]
    private bool _useGravity = false;

    [SerializeField]
    private bool _shareVerticies = false;

    [SerializeField]
    private bool _smoothVerticices = false;

    public bool IsSolid 
    { 
        get { return _isSolid; } 
        set { _isSolid = value; } 
    }
    
    public bool ReverseWindTriangles 
    { 
        get { return _reverseWindTriangles; } 
        set { _reverseWindTriangles = value; }
    }

    public bool UseGravity
    {
        get { return _useGravity; }
        set { _useGravity = value; }
    }

    public bool ShareVerticies
    {
        get { return _shareVerticies; }
        set { _shareVerticies = value; }
    }

    public bool SmoothVerticices
    {
        get { return _smoothVerticices; }
        set { _smoothVerticices = value; }
    }
}
