using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using SystemRandom = System.Random;
using Random = UnityEngine.Random;
using System;

public class Grid 
{

    public List<Vertex> Vertices;
    public List<Vertex3> Vertices3;

    public List<Edge> Edges;
    public List<Triangle> Triangles;
    public List<Quad> Quads;
    public List<SubQuad> SubQuads;
    public List<Cube> Cubes;

    public LineRenderer LineRenderer;

    public int Floor;
    public float Height;

    public int ringCount = 5;

    public float outerRadius = 1f;
    public Grid(int ringCount, float outerRadius, int floor, float height)
    {
        Vertices = new List<Vertex>();
        Edges = new List<Edge>();
        Triangles = new List<Triangle>();
        Quads = new List<Quad>();
        SubQuads = new List<SubQuad>();
        Cubes = new List<Cube>();

        Floor = floor;
        Height = height;
        Vertices3 = new List<Vertex3>();

        this.ringCount = ringCount;
        this.outerRadius = outerRadius; 

        GenerateVertices(ringCount, outerRadius);
        GenerateTriangles();
        MakeQuadsFromTriangles();
        SplitPolygons();
        RelaxVertices();
        FindBoundary();
        //for (int i = 0; i < Floor; i++)
        //{
        //    foreach (var item in Vertices)
        //    {
        //        var verticeY = new Vertex3(item, i * Height);
        //        Vertices3.Add(verticeY);
        //    }
        //}

        foreach (var subQuad in SubQuads)
        {
            for (int i = 0; i < Floor; i++)
            {
                var cube = new Cube(subQuad, i, Height);
                Cubes.Add(cube);

                cube.Vertices.ForEach(x => x.SubQuad = subQuad);
                Vertices3.AddRange(cube.Vertices);
                               
            }           
        }

        foreach (var vertex in Vertices3)
        {
            //if (vertex.Y == 0 || vertex.Y == Floor * Height)
            //    vertex.IsBoundary = true;
            if (vertex.Y == Floor * Height)
                vertex.IsBoundary = true;
        }
    }


    private void GenerateVertices(int ringCount, float outerRadius)
    {
        Vertices.Add(new Vertex(new Vector3(), -1));

        var innerRadius = outerRadius * 0.866025404f;

        for (var ringIndex = 0; ringIndex < ringCount; ringIndex++)
        {
            var newInnerRadius = innerRadius + innerRadius * ringIndex;
            var newOuterRadius = outerRadius + outerRadius * ringIndex;

            // hex has 6 Edges, each edge 2 vertex plus Vertices between them depends on ring index
            AddRingEdge(new Vector3(0, 0, newOuterRadius), new Vector3(newInnerRadius, 0, 0.5f * newOuterRadius), ringIndex);
            AddRingEdge(new Vector3(newInnerRadius, 0, 0.5f * newOuterRadius), new Vector3(newInnerRadius, 0, -0.5f * newOuterRadius), ringIndex);
            AddRingEdge(new Vector3(newInnerRadius, 0, -0.5f * newOuterRadius), new Vector3(0, 0, -newOuterRadius), ringIndex);
            AddRingEdge(new Vector3(0, 0, -newOuterRadius), new Vector3(-newInnerRadius, 0, -0.5f * newOuterRadius), ringIndex);
            AddRingEdge(new Vector3(-newInnerRadius, 0, -0.5f * newOuterRadius), new Vector3(-newInnerRadius, 0, 0.5f * newOuterRadius), ringIndex);
            AddRingEdge(new Vector3(-newInnerRadius, 0, 0.5f * newOuterRadius), new Vector3(0, 0, newOuterRadius), ringIndex);
        }
    }

    private void AddRingEdge(Vector3 startPoint, Vector3 endPoint, int ringIndex)
    {
        // first vertex of hex edge
        var vertex = FindVertex(startPoint);
        if (vertex == null)
        {
            Vertices.Add(new Vertex(startPoint, ringIndex));
        }



        // adding Vertices to the edge 
        var step = 1f / (ringIndex + 1);
        for (var i = 0; i < ringIndex; i++)
        {
            var nextPoint = step + step * i;

            Vertices.Add(new Vertex(Vector3.Lerp(
                startPoint,
                endPoint,
                nextPoint), ringIndex));
        }

        // second vertex of hex edge
        vertex = FindVertex(endPoint);
        if (vertex == null)
        {
            Vertices.Add(new Vertex(endPoint, ringIndex));
        }
    }

    private void GenerateTriangles()
    {
        for (var ringIndex = 0; ringIndex < ringCount - 1; ringIndex++)
        {
            var ringVertices = Vertices.Where(vertex => vertex.RingIndex == ringIndex).ToList();

            foreach (var firstPointOfTriangle in ringVertices)
            {
                for (var degreesIndex = 1; degreesIndex <= 6; degreesIndex++)
                {
                    var secondPointOfTriangle = GetVertexByDegreesFromStartPoint(60 * degreesIndex - 30, firstPointOfTriangle);
                    var thirdPointOfTriangle = GetVertexByDegreesFromStartPoint(60 * (degreesIndex + 1) - 30, firstPointOfTriangle);

                    var firstEdge = FindEdge(new Edge(firstPointOfTriangle, secondPointOfTriangle));
                    if (firstEdge == null)
                    {
                        firstEdge = new Edge(firstPointOfTriangle, secondPointOfTriangle);
                        firstPointOfTriangle.Edges.Add(firstEdge);
                        secondPointOfTriangle.Edges.Add(firstEdge);
                        Edges.Add(firstEdge);

                    }

                    var secondEdge = FindEdge(new Edge(secondPointOfTriangle, thirdPointOfTriangle));
                    if (secondEdge == null)
                    {
                        secondEdge = new Edge(secondPointOfTriangle, thirdPointOfTriangle);
                        // second edge is always 'top' of triangle and could be outside of hex
                        if (secondPointOfTriangle.RingIndex == ringCount - 1)
                        {
                            secondEdge.IsLastRing = true;
                        }
                        secondPointOfTriangle.Edges.Add(secondEdge);
                        thirdPointOfTriangle.Edges.Add(secondEdge);
                        Edges.Add(secondEdge);
                    }

                    var thirdEdge = FindEdge(new Edge(thirdPointOfTriangle, firstPointOfTriangle));
                    if (thirdEdge == null)
                    {
                        thirdEdge = new Edge(thirdPointOfTriangle, firstPointOfTriangle);
                        thirdPointOfTriangle.Edges.Add(thirdEdge);
                        firstPointOfTriangle.Edges.Add(thirdEdge);
                        Edges.Add(thirdEdge);
                    }

                    var triangle = new Triangle(new List<Edge>(3) { firstEdge, secondEdge, thirdEdge });
                    if (IsTriangleAlreadyExist(triangle))
                    {
                        continue;
                    }
                    firstEdge.Triangles.Add(triangle);
                    secondEdge.Triangles.Add(triangle);
                    thirdEdge.Triangles.Add(triangle);

                    Triangles.Add(triangle);
                }
            }
        }
    }

    private Vertex GetVertexByDegreesFromStartPoint(float degrees, Vertex startPoint)
    {
        var radians = degrees * Mathf.Deg2Rad;
        var x = Mathf.Cos(radians);
        var z = Mathf.Sin(radians);

        return Vertices.Where(vertex => vertex.Position == startPoint.Position + new Vector3(x, 0, z) * outerRadius).ToArray()[0];
    }

    private void MakeQuadsFromTriangles()
    {
        var rnd = new SystemRandom();
        foreach (var triangle in Triangles.OrderBy(item => rnd.Next()).Where(x => !x.EdgeWasRemoved))
        {
            var triedIndices = new List<int>();
            var isEdgeRemoved = false;

            while (!isEdgeRemoved && triedIndices.Count < 3)
            {
                var randomIndex = Random.Range(0, 3);
                if (triedIndices.Contains(randomIndex))
                {
                    continue;
                }
                triedIndices.Add(randomIndex);

                var edgeToRemove = triangle.Edges.ToArray()[randomIndex];
                if (edgeToRemove.IsLastRing)
                {
                    continue;
                }

                var edgeTriangles = edgeToRemove.Triangles;
                var isEdgeCouldBeRemoved = edgeTriangles.All(edgeTriangle => !edgeTriangle.EdgeWasRemoved);

                if (isEdgeCouldBeRemoved)
                {
                    Edges.Remove(edgeToRemove);
                    foreach (var vertex in edgeToRemove.Vertices)
                    {
                        vertex.Edges.Remove(edgeToRemove);
                    }

                    isEdgeRemoved = true;

                    // creating quad
                    var quadEdges = new List<Edge>();
                    foreach (var edgeTriangle in edgeTriangles)
                    {
                        edgeTriangle.Edges.Remove(edgeToRemove);
                        edgeTriangle.EdgeWasRemoved = true;
                        quadEdges.Add(edgeTriangle.Edges.ToArray()[0]);
                        quadEdges.Add(edgeTriangle.Edges.ToArray()[1]);
                    }

                    Quads.Add(new Quad(quadEdges));
                }
            }
        }
    }

    private void SplitPolygons()
    {
        SplitToQuads(Quads);
        SplitToQuads(Triangles.Where(x => x.Edges.Count == 3));
    }
    private void SplitToQuads(IEnumerable<Polygon> polygons)
    {        
        foreach (var polygon in polygons)
        {
            var edgeCenters = new List<Vertex>();
            var quadCenter = new Vector3();

            foreach (var edge in polygon.Edges)
            {
                var centerOfEdge = (edge.Vertices.ToArray()[0].Position + edge.Vertices.ToArray()[1].Position) / 2;
                var isLastRing = edge.Vertices.ToArray()[0].RingIndex == ringCount - 1 &&
                                 edge.Vertices.ToArray()[1].RingIndex == ringCount - 1;

                var vertex = FindVertex(centerOfEdge);
                if (vertex == null)
                {
                    Edges.Remove(edge);

                    vertex = new Vertex(centerOfEdge, (isLastRing) ? ringCount - 1 : 0);
                    Vertices.Add(vertex);

                    var newEdge = new Edge(edge.Vertices.ToArray()[0], vertex);
                    Edges.Add(newEdge);
                    edge.Vertices.ToArray()[0].Edges.Remove(edge);
                    edge.Vertices.ToArray()[0].Edges.Add(newEdge);
                    vertex.Edges.Add(newEdge);

                    newEdge = new Edge(edge.Vertices.ToArray()[1], vertex);
                    Edges.Add(newEdge);
                    edge.Vertices.ToArray()[1].Edges.Remove(edge);
                    edge.Vertices.ToArray()[1].Edges.Add(newEdge);
                    vertex.Edges.Add(newEdge);
                }

                edgeCenters.Add(vertex);
                quadCenter += vertex.Position;
            }

            var vertexOfQuadCenter = new Vertex(quadCenter / polygon.Edges.Count, 0);
            Vertices.Add(vertexOfQuadCenter);

            foreach (var edgeCenter in edgeCenters)
            {
                var edgeToCenter = new Edge(edgeCenter, vertexOfQuadCenter);
                Edges.Add(edgeToCenter);
                vertexOfQuadCenter.Edges.Add(edgeToCenter);
                edgeCenter.Edges.Add(edgeToCenter);
            }



            

            List<Vertex> verticesSorted = SortPointsClockwise(polygon.Edges);
            List<Edge> edgesSorted = SortEdges(polygon.Edges);

            //Debug.Log(polygon.Edges.Count);

            //foreach (var item in polygon.Edges)
            //{
            //    Debug.Log("point1: " + item.Vertices[0].Position);
            //    Debug.Log("point2: " + item.Vertices[1].Position);
            //    Debug.Log("\n");
            //}

            //foreach (var item in verticesSorted)
            //{
            //    Debug.Log(item.Position);
            //}
            for (int i = 0; i < edgesSorted.Count; i++)
            {
                Vertex[] fourVertices = new Vertex[4];

                var edge = edgesSorted[i];
                var previousEdge = edgesSorted[(i - 1 + edgesSorted.Count) % edgesSorted.Count];

                var centerOfEdge = (edge.Vertices.ToArray()[0].Position + edge.Vertices.ToArray()[1].Position) / 2;

                var centerOfPreEdge = (previousEdge.Vertices.ToArray()[0].Position + previousEdge.Vertices.ToArray()[1].Position) / 2;

                fourVertices[0] = FindVertex(verticesSorted[i].Position);
                fourVertices[3] = FindVertex(centerOfPreEdge);

                fourVertices[1] = FindVertex(centerOfEdge);
                fourVertices[2] = vertexOfQuadCenter;

                SubQuads.Add(CreateSubQuad(fourVertices));
            }
            
            
        }
    }

    private void RelaxVertices()
    {
        for (var i = 0; i < 300; i++)
        {
            foreach (var vertex in Vertices)
            {
                if (vertex.RingIndex == ringCount - 1)
                {                   
                    continue;
                }

                var positionToChange = new Vector3();
                foreach (var edge in vertex.Edges)
                {
                    var directionEdge = (edge.Vertices.ToArray()[0] == vertex)
                        ? edge.Vertices.ToArray()[1]
                        : edge.Vertices.ToArray()[0];

                    positionToChange += Vector3.Lerp(vertex.Position, directionEdge.Position, 0.05f);

                }
                vertex.Position = positionToChange / vertex.Edges.Count;
            }

        }
    }


    private void FindBoundary()
    {
        foreach (var vertex in Vertices)
        {
            if (vertex.RingIndex == ringCount - 1)
            {
                vertex.IsBoundary = true;
            }
        }        
    }

    [CanBeNull]
    public Vertex FindVertex(Vector3 position)
    {
        var vertex = Vertices.Where(x => x.Position == position).ToArray();
        return vertex.Length > 0 ? vertex[0] : null;
    }

    [CanBeNull]
    public Edge FindEdge(Edge edge)
    {
        foreach (var existingEdge in Edges)
        {
            if (Edge.IsEdgesSame(edge, existingEdge))
            {
                return existingEdge;
            }
        }

        return null;
    }
    /// <summary>
    /// 通过grid的Vertex，以及高度，返回所有与此相关的Vertex3的点
    /// </summary>
    /// <param name="vertex"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [CanBeNull]
    public List<Vertex3> GetVertex3(Vertex vertex, float y)
    {
        List<Vertex3> vertices = new List<Vertex3>();
        foreach(var existingVertex3 in Vertices3)
        {
            if (existingVertex3.Vertex == vertex &&
               existingVertex3.worldPosition == vertex.Position + Vector3.up * y)
                vertices.Add(existingVertex3);
        }

        return vertices;
    }

    /// <summary>
    /// 通过grid的Vertex，以及高度，返回所有与此相关的Vertex3的点
    /// </summary>
    /// <param name="vertex"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [CanBeNull]
    public List<Vertex3> GetVertex3(Vector3 vertex)
    {
        List<Vertex3> vertices = new List<Vertex3>();
        foreach (var existingVertex3 in Vertices3)
        {
            if (existingVertex3.Vertex == FindVertex(vertex) &&
               existingVertex3.worldPosition == vertex)
                vertices.Add(existingVertex3);
        }

        return vertices;
    }


    /// <summary>
    /// 返回1/4的subquad的4个点
    /// </summary>
    /// <param name="vertex3"></param>
    /// <param name="neighbours">获得该点的顺时针方向第一个以及第二个邻居</param>
    /// <returns></returns>
    public List<Vector3> GetQuarterSubQuad(Vertex3 vertex3, out List<Vertex> neighbours )
    {
        Vector3[] quarters = new Vector3[4];
        var subQuad = vertex3.SubQuad;
        Vector3 a = subQuad.Vertices[0].Position + Vector3.up * vertex3.Y;
        Vector3 b = subQuad.Vertices[1].Position + Vector3.up * vertex3.Y;
        Vector3 c = subQuad.Vertices[2].Position + Vector3.up * vertex3.Y;
        Vector3 d = subQuad.Vertices[3].Position + Vector3.up * vertex3.Y;

        Vector3 ab = (a + b) / 2;
        Vector3 bc = (b + c) / 2;
        Vector3 cd = (c + d) / 2;
        Vector3 da = (d + a) / 2;
        Vector3 center = (a + b + c + d) / 4;

        var twoNeighbours = new List<Vertex>();

        if(vertex3.Vertex == subQuad.Vertices[0])
        {
            quarters = new Vector3[4] { a, ab, center, da };
            twoNeighbours.Add(subQuad.Vertices[1]);
            twoNeighbours.Add(subQuad.Vertices[3]);
        }
            
        else if(vertex3.Vertex == subQuad.Vertices[1])
        {
            quarters = new Vector3[4] { b, bc, center, ab };
            twoNeighbours.Add(subQuad.Vertices[2]);
            twoNeighbours.Add(subQuad.Vertices[0]);
        }
            
        else if(vertex3.Vertex == subQuad.Vertices[2])
        {
            quarters = new Vector3[4] { c, cd, center, bc };
            twoNeighbours.Add(subQuad.Vertices[3]);
            twoNeighbours.Add(subQuad.Vertices[1]);
        }
            
        else if(vertex3.Vertex == subQuad.Vertices[3])
        {
            quarters = new Vector3[4] { d, da, center, cd };
            twoNeighbours.Add(subQuad.Vertices[0]);
            twoNeighbours.Add(subQuad.Vertices[2]);
        }

        neighbours = twoNeighbours;

        return quarters.ToList();    
    }


    private bool IsTriangleAlreadyExist(Triangle triangle)
    {
        foreach (var existingTriangle in Triangles)
        {
            if (Triangle.IsTrianglesSame(triangle, existingTriangle))
            {
                return true;
            }
        }

        return false;
    }


    public SubQuad CreateSubQuad(Vertex[] vertices)
    {
        List<Edge> edges = new List<Edge>();
        for (int i = 0; i < vertices.Length; i++)
        {
            edges.Add(FindEdge(new Edge(vertices[i], vertices[(i + 1) % vertices.Length])));
        }
        return new SubQuad(edges);
    }

    public List<Edge> SortEdges(List<Edge> originEdges)
    {
        List<Vertex> vertices = SortPointsClockwise(originEdges);
        List<Edge> edges = new List<Edge>();
       
        for (int i = 0; i < vertices.Count; i++)
        {
            foreach (Edge edge in originEdges)
            {
                var a = edge.Vertices[0].Position;
                var b = edge.Vertices[1].Position;

                if (vertices[i].Position == a && vertices[(i + 1) % vertices.Count].Position == b ||
                    vertices[i].Position == b && vertices[(i + 1) % vertices.Count].Position == a)
                    edges.Add(edge);
            }           
        }

        return edges;
    }



    public List<Vertex> SortPointsClockwise(List<Edge> edges)
    {
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

        var vertexOfQuadCenter = FindVertex(quadCenter / vertices.Count);

        vertices.Sort((a, b) =>
        {
            double a1 = (Mathf.Rad2Deg * Math.Atan2(a.Position.x - vertexOfQuadCenter.Position.x, a.Position.z - vertexOfQuadCenter.Position.z) + 360) % 360;
            double a2 = (Mathf.Rad2Deg * Math.Atan2(b.Position.x - vertexOfQuadCenter.Position.x, b.Position.z - vertexOfQuadCenter.Position.z) + 360) % 360;
            return (int)(a1 - a2);
        });

        

        return vertices;
    }



    public void CreateLineRender(SubQuad quad, LineRenderer lineRenderer)
    {  
        lineRenderer.useWorldSpace = true;
              
        List<Vertex> vertices = quad.Vertices;
        lineRenderer.positionCount = vertices.Count;
        for (int i = 0; i < vertices.Count; i++)
        {
            lineRenderer.SetPosition(i, vertices[i].Position);
        }
    }
}
