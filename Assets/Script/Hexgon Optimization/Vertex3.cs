using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex3
{
    public float Y;
    public Vertex Vertex;
    public Vector3 worldPosition;
    public bool IsActive;
    public bool IsBoundary;

    public SubQuad SubQuad;
    public Vertex3(Vertex vertex, float y)
    {
        Y = y;
        Vertex = vertex;
        IsBoundary = vertex.IsBoundary;
        worldPosition = vertex.Position + Vector3.up * y;
    }



}
