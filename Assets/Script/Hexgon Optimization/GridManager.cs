using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SystemRandom = System.Random;
using Random = UnityEngine.Random;
using UnityEditor;

public class GridManager : MonoBehaviour
{
    static GridManager m_instance;
    public static GridManager Instance => m_instance;

    public int ringCount = 5;
    
    public float outerRadius = 5f;

    public int Floor = 3;
    public float Height = 3f;

    public GameObject lineRenderer;

    public Transform add;
    public Transform subtract;
    public float distance = 2f;
    public Grid grid;

    public List<Vertex> vertices = new List<Vertex>();

    public List<Vertex3> verticesAll = new List<Vertex3>();
    private List<Edge> edges = new List<Edge>();
    private List<Triangle> triangles = new List<Triangle>();
    public List<Quad> quads = new List<Quad>();
    public List<SubQuad> subQuads = new List<SubQuad>();
    public List<Cube> cubes = new List<Cube>();


    public ModuleManager ModuleManager;
    public Material ModuleMaterial;

    private void Awake()
    {
        CreateSingleton();

        ringCount = TransmitParameter.Instance.ring;
        Floor = TransmitParameter.Instance.floor;

        grid = new Grid(ringCount, outerRadius, Floor, Height);

        ModuleManager = Instantiate(ModuleManager);

        vertices = grid.Vertices;
        edges = grid.Edges;
        quads = grid.Quads;
        triangles = grid.Triangles;
        subQuads = grid.SubQuads;
        verticesAll = grid.Vertices3;
        cubes = grid.Cubes;


        
        CreateLineRenders();

    }

    public void CreateLineRenders()
    {
        GameObject gameObject = new GameObject("Lines");
        gameObject.transform.parent = transform;

        foreach (var subQuad in subQuads)
        {            
            var line = Instantiate(lineRenderer, gameObject.transform);
            var render = line.GetComponent<LineRenderer>();

            grid.CreateLineRender(subQuad, render);
        }
        
    }

    void CreateSingleton()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
    }

    private void Update()
    {
        foreach (var item in verticesAll)
        {
            if (!item.IsActive && Vector3.Distance(item.worldPosition, add.position) < distance  && !item.IsBoundary)
                item.IsActive = true;
            else if (item.IsActive && Vector3.Distance(item.worldPosition, subtract.position) < distance)
                item.IsActive = false;
        }

        foreach (var cube in cubes)
        {
            cube.UpdateBit();
            if(cube.PreBit != cube.Bit)
            {
                UpdateSlot(cube);
            }

        }
    }


    private void UpdateSlot(Cube cube)
    {
        string name = "Slot_" + grid.Cubes.IndexOf(cube) + "_" + cube.CenterPoint.y;

        GameObject slot_gameObject;

        if (transform.GetChild(0).Find(name))
        {
            slot_gameObject = transform.GetChild(0).Find(name).gameObject;
        }
        else
            slot_gameObject = null;

        if(slot_gameObject == null)
        {
            if(cube.Bit != 0 && cube.Bit != 255)
            {
                slot_gameObject = new GameObject(name, typeof(Slot));
                slot_gameObject.transform.SetParent(transform.GetChild(0));
                slot_gameObject.transform.localPosition = cube.CenterPoint;
                Slot slot = slot_gameObject.GetComponent<Slot>();
                slot.Initialize(ModuleManager, cube, ModuleMaterial);
                slot.UpdateModule(slot.Modules);
            }
        }
        else
        {
            Slot slot = slot_gameObject.GetComponent<Slot>();
            if (cube.Bit == 0 || cube.Bit == 255)
            {
                Destroy(slot_gameObject);
                Resources.UnloadUnusedAssets();
            }
            else
            {
                slot.ResetModules(ModuleManager);
                slot.UpdateModule(slot.Modules);
            }
        }

    }


    private void OnDrawGizmos () {
        if (vertices == null) {
            return;
        }

        //foreach (var vertex in vertices)
        //{
        //    Gizmos.DrawSphere(vertex.Position, 0.1f);
        //}

        //foreach (var vertex in verticesAll)
        //{
        //    if (vertex.IsActive)
        //        Gizmos.color = Color.red;
        //    else
        //        Gizmos.color = Color.gray;
        //    Gizmos.DrawSphere(vertex.worldPosition, 0.1f);
        //}

        //foreach (var edge in edges)
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawLine(edge.Vertices.ToArray()[0].Position, edge.Vertices.ToArray()[1].Position);
        //}


        for (int j = 0; j < subQuads.Count; j++)
        {
            var quad = subQuads[j];

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawSphere(quad.Edges[0].Vertices.ToArray()[0].Position, 0.2f);

            //for (int i = 0; i < quad.Edges.Count; i++)
            //{               
            //    var edge = quad.Edges[i];
            //    var edge_next = quad.Edges[(i +1) % quad.Edges.Count];
            //    Gizmos.color = Color.cyan;
            //    Gizmos.DrawLine(edge.Vertices.ToArray()[0].Position, edge.Vertices.ToArray()[1].Position);
            //    //Gizmos.color = Color.red;
            //    //Gizmos.DrawRay(new Ray(edge.Vertices.ToArray()[0].Position, edge.Vertices.ToArray()[1].Position- edge.Vertices.ToArray()[0].Position));
            //    //GUI.color = Color.yellow;
            //    //Handles.Label(edge.Vertices.ToArray()[0].Position, edge.Vertices.ToArray()[0].Position.ToString());
            //    //Handles.Label(edge.Vertices.ToArray()[1].Position, edge.Vertices.ToArray()[1].Position.ToString());
            //}
        }

        //foreach (var quad in quads)
        //{
        //    for (int i = 0; i < quad.Edges.Count; i++)
        //    {
        //        var edge = quad.Edges[i];
        //        Gizmos.color = Color.red;
        //        Gizmos.DrawLine(edge.Vertices.ToArray()[0].Position, edge.Vertices.ToArray()[1].Position);

        //    }
        //}


        //for (int j = 0; j < cubes.Count; j++)
        //{
        //    var cube = cubes[j];
        //    GUI.color = Color.yellow;
        //    Handles.Label(cube.CenterPoint, cube.Bit.ToString());
        //    //for (int i = 0; i < cube.Edges.Count; i++)
        //    //{
        //    //    var edge = cube.Edges[i];

        //    //    Gizmos.color = Color.cyan;
        //    //    Gizmos.DrawLine(edge.Vertices3.ToArray()[0].worldPosition, edge.Vertices3.ToArray()[1].worldPosition);

        //    //}

        //    foreach (var vertex in cube.Vertices)
        //    {
        //        if (vertex.IsActive)
        //            Gizmos.color = Color.red;
        //        else
        //            Gizmos.color = Color.gray;
        //        Gizmos.DrawSphere(vertex.worldPosition, 0.3f);
        //    }
        //}



    }


}
