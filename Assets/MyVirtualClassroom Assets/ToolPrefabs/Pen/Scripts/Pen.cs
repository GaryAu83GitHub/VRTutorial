using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Pen : MonoBehaviour
{
    [SerializeField]
    private Transform PenTip;

    [SerializeField]
    private GameObject MarkingLineObjct;

    [SerializeField]
    public InputActionProperty DrawingButton;

    private MarkingLine myLine;

    private bool myIsDrawing = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine(DrawingButton.action.IsPressed(), PenTip.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Rigidbody>().useGravity = false;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Rigidbody>().useGravity = true;
    }

    /// <summary>
    /// The main funciton for the line drawing.
    /// When the button for line drawing is pressed it'll toggle the boolian for is drawing.
    /// The line will be instantiated and set the start position for the line.
    /// As long the button is pressed when the line is instantiated, the end of the line will
    /// keep tailing after the tip of the pen until the button is released.
    /// </summary>
    /// <param name="aDrawButtonPressed">Boolian for the button for drawing is pressed</param>
    /// <param name="aTipPosition">THe position of the tip transformation</param>
    private void DrawLine(bool aDrawButtonPressed, Vector3 aTipPosition)
    {
        if(aDrawButtonPressed)
        {
            if (!myIsDrawing)
            {
                myLine = Instantiate(MarkingLineObjct, aTipPosition, Quaternion.identity).GetComponent<MarkingLine>();
                myLine.StartDrawing(aTipPosition);
                myIsDrawing = true;
            }
            else
                myLine.DrawingLine(aTipPosition);
        }
        else
        {
            if(myIsDrawing)
            {
                myLine.StopDrawing(aTipPosition);
                myIsDrawing = false;
            }
        }
    }
}
