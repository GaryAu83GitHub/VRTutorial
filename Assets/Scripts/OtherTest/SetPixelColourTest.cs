using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPixelColourTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Vector3 max = GetComponent<Renderer>().bounds.max;
        //Vector3 min = GetComponent<Renderer>().bounds.min;

        Debug.Log("Render Max: " + GetComponent<Renderer>().bounds.max);
        Debug.Log("Render Min: " + GetComponent<Renderer>().bounds.min);
        Debug.Log("Render Size: " + GetComponent<Renderer>().bounds.size);

        Debug.Log("Collider Max: " + GetComponent<BoxCollider>().bounds.max);
        Debug.Log("Collider Min: " + GetComponent<BoxCollider>().bounds.min);
        Debug.Log("Collider Size: " + GetComponent<BoxCollider>().bounds.size);

        Texture2D tex = new Texture2D(100, 100);
        GetComponent<Renderer>().material.mainTexture = tex;
        tex.SetPixel(3, 5, Color.red);
        tex.Apply();

        return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
