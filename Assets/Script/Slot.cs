using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Module Modules;

    public Cube Cube;
    public GameObject ModuleObject;

    public Material Material;

    private void Awake()
    {
        ModuleObject = new GameObject("Module", typeof(MeshFilter), typeof(MeshRenderer));
        ModuleObject.transform.SetParent(transform, false);
        ModuleObject.transform.localPosition = Vector3.zero;
    }

    public void Initialize(ModuleManager moduleManager, Cube cube, Material material)
    {
        this.Cube = cube;
        ResetModules(moduleManager);
        this.Material = material;
    }

    public void ResetModules(ModuleManager moduleManager)
    {
        this.Modules = moduleManager.GetModule(Cube.Bit);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void DeformModule(Mesh mesh, Cube cube)
    {
        SubQuad subQuad = cube.SubQuad;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 ad_x = Vector3.Lerp(subQuad.Vertices[0].Position, subQuad.Vertices[3].Position, (vertices[i].x + 0.5f));
            Vector3 bc_x = Vector3.Lerp(subQuad.Vertices[1].Position, subQuad.Vertices[2].Position, (vertices[i].x + 0.5f));
            vertices[i] = Vector3.Lerp(ad_x, bc_x, (vertices[i].z + 0.5f)) + Vector3.up * vertices[i].y * GridManager.Instance.grid.Height - subQuad.CenterPoint;

        }
        mesh.vertices = vertices;
    }

    public void UpdateModule(Module module)
    {

        this.ModuleObject.GetComponent<MeshFilter>().mesh = module.Mesh;
        DeformModule(this.ModuleObject.GetComponent<MeshFilter>().mesh, Cube);       
        this.ModuleObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        this.ModuleObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        this.ModuleObject.GetComponent<MeshRenderer>().material = Material;
    }
}
