using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This class is for creating STRAIGHT line object for drawing line tools that will be a child objects
/// in substance objects.
/// 
/// Object with this class bounded will only be created when a substance object being touch by
/// a tool object that is deemed to have drawing capability 
/// </summary>
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
        myLine.SetPosition(1, aPos);

        //Debug.Log("Line start at " + aPos.ToString());
    }

    /// <summary>
    /// Set the end point of the line and toggle the line drawing
    /// </summary>
    /// <param name="aStopPos">The position the line end is set</param>
    public void StopDrawing(Vector3 aStopPos)
    {
        myIsDrawing = !myIsDrawing;
        myLine.SetPosition(1, aStopPos);
        //Debug.Log("Line end at " + aStopPos.ToString());
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
    
    /// <summary>
    /// When the substance objects start to draw, the object will first be adding the LineRender
    /// Component to the object and pass the new added component to the LineRender instance in the
    /// class.
    /// 
    /// If the LineRender component failed to be added, the function will be ceased before setting
    /// the selected drawing colour, the defaul material and the minimal width of the line
    /// </summary>
    private void CreateLine()
    {
        transform.AddComponent<LineRenderer>();
        myLine = GetComponent<LineRenderer>();

        if (myLine == null)
            return;

        myLine.startColor = myLine.endColor = DrawColor;
        myLine.material = LineMaterial;

        myLine.startWidth = myLine.endWidth = LINE_MINIMUM_WIDTH;
    }
}
