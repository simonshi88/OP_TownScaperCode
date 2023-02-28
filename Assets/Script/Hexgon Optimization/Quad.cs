using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Quad : Polygon
{
    public Quad(List<Edge> edges)
    {
        this.Edges = edges;

    }
}

public class SubQuad : Polygon
{
    public List<Vertex> Vertices;

    public Vector3 CenterPoint;

    public SubQuad(List<Edge> edges)
    {
        this.Edges = edges;
        var vertices = new List<Vertex>();
        foreach (var edge in edges)
        {
            foreach (var vertex in edge.Vertices)
            {
                if (!vertices.Contains(vertex))
                    vertices.Add(vertex);
            }
        }

        var quadCenter = new Vector3();
        foreach (var vertex in vertices)
        {
            quadCenter += vertex.Position;
        }

        CenterPoint = quadCenter / vertices.Count;

        Vertices = vertices;


    }
}


