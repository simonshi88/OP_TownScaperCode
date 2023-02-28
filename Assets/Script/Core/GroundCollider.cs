using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateGroundCollider(Grid grid)
    {
        foreach (var subQuad in grid.SubQuads)
        {
            Mesh mesh = new Mesh();

            List<Vector3> vectors = new List<Vector3>();
            foreach (var vertex in subQuad.Vertices)
            {
                vectors.Add(vertex.Position);
            }
            mesh.vertices = vectors.ToArray();
            mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };

            mesh.RecalculateBounds();

            GameObject ground = new GameObject("GroundCollider_" + grid.SubQuads.IndexOf(subQuad),typeof(MeshCollider), typeof(GroundColliderQuad));

            ground.transform.parent = transform;
            ground.GetComponent<MeshCollider>().sharedMesh= mesh;
            ground.GetComponent<GroundColliderQuad>().SubQuad = subQuad;
            ground.layer = LayerMask.NameToLayer("GroundCollider");
        }
    }
}

public class GroundColliderQuad : MonoBehaviour
{
    public SubQuad SubQuad;
}
