using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex
{
    public Vector3 Position;
    
    public List<Edge> Edges = new List<Edge>();

    public int RingIndex;

    public bool IsBoundary;

    public Vertex(Vector3 position, int ringIndex)
    {
        this.Position = position;
        this.RingIndex = ringIndex;
    }

}

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