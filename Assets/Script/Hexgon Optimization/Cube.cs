using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube
{
    public float Height;
    public int Floor;

    public List<Vertex3> Vertices;
    public List<Edge> Edges;
    public SubQuad SubQuad;
    public int Bit;
    public int PreBit;
    public Vector3 CenterPoint;
    public Cube(SubQuad subQuad, int floor, float height)
    {
        this.SubQuad = subQuad;
        Height = height;
        Floor = floor;
        var vertices = subQuad.Vertices;

        CenterPoint = subQuad.CenterPoint + Vector3.up * ((floor + 0.5f) * height);

        Edges = new List<Edge>();
        List<Edge> edgeLower = new List<Edge>();
        List<Edge> edgeUpper = new List<Edge>();
        List<Edge> edgeMiddle = new List<Edge>();

        Vertices = new List<Vertex3>();
        List<Vertex3> vertexLower = new List<Vertex3>();
        List<Vertex3> vertexUpper = new List<Vertex3>();

        foreach (var vertex in vertices)
        {
            vertexLower.Add(new Vertex3(vertex, Floor * Height));
            vertexUpper.Add(new Vertex3(vertex, (Floor + 1) * Height));
        }

        for (int i = 0; i < 4; i++)
        {
            edgeLower.Add(new Edge(vertexLower[i], vertexLower[(i + 1) % 4]));
            edgeUpper.Add(new Edge(vertexUpper[i], vertexUpper[(i + 1) % 4]));
            edgeMiddle.Add(new Edge(vertexLower[i], vertexUpper[i]));
        }

        Vertices.AddRange(vertexLower);
        Vertices.AddRange(vertexUpper);

        Edges.AddRange(edgeLower);
        Edges.AddRange(edgeUpper);
        Edges.AddRange(edgeMiddle);

    }


    public void UpdateBit()
    {
        this.PreBit = Bit;
        this.Bit = bitMask.GetBitMask(this);
    }
}

