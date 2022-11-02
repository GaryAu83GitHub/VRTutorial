using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarkingLine : MonoBehaviour
{
    [SerializeField]
    private Color DrawColor, WithinColor, LockedColor;

    [SerializeField]
    private Material LineMaterial;

    private LineRenderer myLine;

    private bool myIsDrawing = false;
    
    private const float LINE_MINIMUM_WIDTH = .001f;

    /// <summary>
    /// Set the start point of the line and toggle the line drawing
    /// </summary>
    /// <param name="aPos">The position the line start is set</param>
    public void StartDrawing(Vector3 aPos)
    {
        myIsDrawing = !myIsDrawing;
        CreateLine();
        myLine.SetPosition(0, aPos);
    }

    /// <summary>
    /// Set the end point of the line and toggle the line drawing
    /// </summary>
    /// <param name="aStopPos">The position the line end is set</param>
    public void StopDrawing(Vector3 aStopPos)
    {
        myIsDrawing = !myIsDrawing;
        myLine.SetPosition(1, aStopPos);
    }

    /// <summary>
    /// Navigate the line's end point by following the position that was
    /// sent in
    /// </summary>
    /// <param name="aCurrentPos">The current position the end point is locateing</param>
    public void DrawingLine(Vector3 aCurrentPos)
    {
        if (!myIsDrawing)
            return;
        myLine.SetPosition(1, aCurrentPos);
    }

    private void CreateLine()
    {
        transform.AddComponent<LineRenderer>();
        myLine = GetComponent<LineRenderer>();

        myLine.startColor = myLine.endColor = DrawColor;
        myLine.material = LineMaterial;

        myLine.startWidth = myLine.endWidth = LINE_MINIMUM_WIDTH;
    }
}
