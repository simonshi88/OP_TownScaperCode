using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Edge
{
    public List<Vertex> Vertices = new List<Vertex>(2);
    public List<Vertex3> Vertices3 = new List<Vertex3>(2);

    public List<Triangle> Triangles = new List<Triangle>();

    public bool IsLastRing = false;

    public Edge(Vertex firstVertex, Vertex secondVertex)
    {
        Vertices.Add(firstVertex);
        Vertices.Add(secondVertex);
    }

    public Edge(Vertex3 firstVertex3, Vertex3 secondVertex3)
    {
        Vertices3.Add(firstVertex3);
        Vertices3.Add(secondVertex3);
    }

    public static bool IsEdgesSame(Edge firstEdge, Edge secondEdge)
    {
        var firstEdgeVertices = firstEdge.Vertices.ToArray();
        var secondEdgeVertices = secondEdge.Vertices.ToArray();

        var isEdgesSame = (firstEdgeVertices[0].Position == secondEdgeVertices[0].Position || firstEdgeVertices[0].Position == secondEdgeVertices[1].Position)
            && (firstEdgeVertices[1].Position == secondEdgeVertices[0].Position || firstEdgeVertices[1].Position == secondEdgeVertices[1].Position);

        return isEdgesSame;
    }
}


public class EdgeComparer : IComparer<Edge>
{
    public int Compare(Edge x, Edge y)
    {
        if (x.Vertices[1].Position == y.Vertices[0].Position &&
    x.Vertices[0].Position != y.Vertices[1].Position)
            return 1;
        else if (x.Vertices[0].Position == y.Vertices[1].Position &&
            x.Vertices[1].Position != y.Vertices[0].Position)
            return -1;
        else
            return 0;
    }
}