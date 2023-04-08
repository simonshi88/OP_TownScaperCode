using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeformOperation : MonoBehaviour
{
    public GameObject ImportMesh;
    public LineRenderer LineRenderer;
    public Material Material;

    GameObject deformObject;
    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        deformObject = new GameObject("deformObject", typeof(MeshFilter), typeof(MeshRenderer));
        deformObject.transform.SetParent(transform, false);
        deformObject.transform.localPosition = Vector3.zero;

        mesh = new Mesh();

        foreach (Transform child in ImportMesh.transform)
        {
            mesh = child.GetComponent<MeshFilter>().mesh;
        }

        Vector3[] boundary = GetPositions(LineRenderer);

        DeformMesh(mesh, boundary);

        //DeformMeshOrigin(mesh, boundary);

        deformObject.GetComponent<MeshFilter>().mesh = mesh;
        deformObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        deformObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        deformObject.GetComponent<MeshRenderer>().material = Material;

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    Vector3[] GetPositions(LineRenderer lineRenderer)
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            positions.Add(lineRenderer.GetPosition(i));
        }
        return positions.ToArray();
    }


    public void DeformMesh(Mesh mesh, Vector3[] vectors)
    {
        if (vectors.Length != 4)
            return;

        Vector3[] vertices = mesh.vertices;

        List<Vector3> orderedVectices = vertices.ToList();
        orderedVectices = orderedVectices.OrderBy(a => a.x).ThenBy(p => p.z).ToList();
        
        Vector3 min = orderedVectices[0];
        Vector3 max = orderedVectices[orderedVectices.Count - 1];

        List<float> percentageX = new List<float>();
        foreach (var vertex in vertices)
        {
            percentageX.Add((vertex.x - min.x)/ (max.x - min.x));
        }

        List<float> percentageZ = new List<float>();
        foreach (var vertex in vertices)
        {
            percentageZ.Add((vertex.z - min.z) / (max.z - min.z));
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 adX = Vector3.Lerp(vectors[0], vectors[3], percentageX[i]);
            Vector3 bcX = Vector3.Lerp(vectors[1], vectors[2], percentageX[i]);

            vertices[i] = Vector3.Lerp(adX, bcX, percentageZ[i]) + Vector3.up * vertices[i].y;
        }
        mesh.vertices = vertices;
    }


    public void DeformMeshOrigin(Mesh mesh, Vector3[] vectors)
    {
        if (vectors.Length != 4)
            return;

        Vector3[] vertices = mesh.vertices;

        
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 adX = Vector3.Lerp(vectors[0], vectors[3], (vertices[i].x - (-2.5f)) / 5);
            Vector3 bcX = Vector3.Lerp(vectors[1], vectors[2], (vertices[i].x - (-2.5f)) / 5);

            vertices[i] = Vector3.Lerp(adX, bcX, (vertices[i].z - (-2.5f)) / 5) + Vector3.up * vertices[i].y;
        }
        mesh.vertices = vertices;
    }
}
