using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridElement : MonoBehaviour
{
	private Collider col;
	private Renderer rend;
	private bool isEnabled;
	private float elementHeight;
	public CornerElement[] Corners;

    public Material Material;

    private Vertex Vertex;
    private List<Vertex3> vertex3s;
    private Grid Grid;
	// Start is called before the first frame update
	void Start()
	{
        elementHeight = GridManager.Instance.Height;
        Grid = GridManager.Instance.grid;

    }

	// Update is called once per frame
	void Update()
	{

	}

    public void Initialize()
    {
        elementHeight = GridManager.Instance.Height;
        Grid = GridManager.Instance.grid;
    }

    //public void Initialize(List<Vertex3> vertex3, float setElementHeight)
    //{
    //    float height = GridManager.Instance.Height;


    //    coord = new coord(setX, setY, setZ);
    //    this.name = "GE_" + this.coord.x + "_" + this.coord.y + "_" + this.coord.z;
    //    this.elementHeight = setElementHeight;
    //    this.transform.localScale = new Vector3(1.0f, elementHeight, 1.0f);
    //    this.col = this.GetComponent<Collider>();
    //    this.rend = this.GetComponent<Renderer>();

    //    //setting corner elements
    //    corners[0] = levelGenerator.instance.cornerElements[coord.x + (width + 1) * (coord.z + (width + 1) * coord.y)];
    //    corners[1] = levelGenerator.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + (width + 1) * coord.y)];
    //    corners[2] = levelGenerator.instance.cornerElements[coord.x + (width + 1) * (coord.z + 1 + (width + 1) * coord.y)];
    //    corners[3] = levelGenerator.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + 1 + (width + 1) * coord.y)];
    //    corners[4] = levelGenerator.instance.cornerElements[coord.x + (width + 1) * (coord.z + (width + 1) * (coord.y + 1))];
    //    corners[5] = levelGenerator.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + (width + 1) * (coord.y + 1))];
    //    corners[6] = levelGenerator.instance.cornerElements[coord.x + (width + 1) * (coord.z + 1 + (width + 1) * (coord.y + 1))];
    //    corners[7] = levelGenerator.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + 1 + (width + 1) * (coord.y + 1))];


    //    //positioning corner elements
    //    corners[0].SetPosition(col.bounds.min.x, col.bounds.min.y, col.bounds.min.z);
    //    corners[1].SetPosition(col.bounds.max.x, col.bounds.min.y, col.bounds.min.z);
    //    corners[2].SetPosition(col.bounds.min.x, col.bounds.min.y, col.bounds.max.z);
    //    corners[3].SetPosition(col.bounds.max.x, col.bounds.min.y, col.bounds.max.z);
    //    corners[4].SetPosition(col.bounds.min.x, col.bounds.max.y, col.bounds.min.z);
    //    corners[5].SetPosition(col.bounds.max.x, col.bounds.max.y, col.bounds.min.z);
    //    corners[6].SetPosition(col.bounds.min.x, col.bounds.max.y, col.bounds.max.z);
    //    corners[7].SetPosition(col.bounds.max.x, col.bounds.max.y, col.bounds.max.z);

    //}



    //public void SetEnable()
    //{
    //    this.isEnabled = true;
    //    this.col.enabled = true;
    //    // this.rend.enabled = true;
    //    foreach (cornerElement ce in corners)
    //    {
    //        ce.SetCornerElement();
    //    }
    //    interactionSound.instance.SetAudioPlay(coord.y);
    //}

    //public void SetDisable()
    //{
    //    this.isEnabled = false;
    //    this.col.enabled = false;
    //    // this.rend.enabled = false;
    //    foreach (cornerElement ce in corners)
    //    {
    //        ce.SetCornerElement();
    //    }
    //    interactionSound.instance.SetAudioPlay(coord.y);
    //}

    public bool GetEnabled()
    {
        return isEnabled;
    }

    public float GetElementHeight()
    {
        return elementHeight;
    }

    public List<Combine> CreateGridElement(Vertex vertex, float height)
    {

        vertex3s = Grid.GetVertex3(vertex, height);

        Corners = new CornerElement[vertex3s.Count * 2];

        List<Combine> combines = new List<Combine>();
        foreach (var vertex3 in vertex3s)
        {
            List<Vertex> directions = new List<Vertex>();
            List<Vector3> vectors = Grid.GetQuarterSubQuad(vertex3, out directions);

            Neighbour[] neighbour = new Neighbour[2];
            for (int i = 0; i < neighbour.Length; i++)
            {
                neighbour[i] = new Neighbour();
            }
            neighbour[0].Vertex3 = vectors[1];
            neighbour[0].Direction = directions[0];
            neighbour[1].Vertex3 = vectors[3];
            neighbour[1].Direction = directions[1];

            Combine combine = new Combine();
            combine.Neighbour = neighbour;
            combine.Signal = vectors[2];
            combine.Self = vertex;
            combine.Y = height;

            combines.Add(combine);
        }

        var quadCenter = new Vector3();
        foreach (var combine in combines)
        {
            quadCenter += combine.Signal;
        }

        var vertexOfQuadCenter = quadCenter / combines.Count;

        combines.Sort((a, b) =>
        {
            double a1 = (Mathf.Rad2Deg * Math.Atan2(a.Signal.x - vertexOfQuadCenter.x, a.Signal.z - vertexOfQuadCenter.z) + 360) % 360;
            double a2 = (Mathf.Rad2Deg * Math.Atan2(b.Signal.x - vertexOfQuadCenter.x, b.Signal.z - vertexOfQuadCenter.z) + 360) % 360;
            return (int)(a1 - a2);
        });

        return combines;
      
        //for (int i = 0; i < combines.Count; i++)
        //{
        //    var combine = combines[i];
        //    var a = Grid.GetVertex3(combine.Neighbour[0].Direction, elementHeight);
        //    var b = Grid.GetVertex3(combine.Neighbour[1].Direction, elementHeight);

        //    foreach (var item in a)
        //    {
        //        item.IsActive = true;
        //    }
        //    foreach (var item in b)
        //    {
        //        item.IsActive = true;
        //    }
        //}

      

    }


    public static void CreatElement(Vertex3 vertex3, Transform parent, Material material)
    {

        var grid = GridManager.Instance.grid;
        Mesh meshA = new Mesh();
        Mesh meshB = new Mesh();

        List<Vertex> neighbours = new List<Vertex>();
        List<Vector3> vectors = grid.GetQuarterSubQuad(vertex3, out neighbours);
        var height = grid.Height / 2;
        
        var mesh_a = new Vector3[4] { vectors[1] - Vector3.up * height, vectors[1] + Vector3.up * height,
                                    vectors[2] + Vector3.up * height, vectors[2] - Vector3.up * height};
        var mesh_b = new Vector3[4] { vectors[3] - Vector3.up * height, vectors[2] - Vector3.up * height,
                                     vectors[2] + Vector3.up * height, vectors[3] + Vector3.up * height };

        meshA.vertices = mesh_a;
        meshA.triangles = new int[6] { 0, 2, 1, 0, 3, 2 };

        meshB.vertices = mesh_b;
        meshB.triangles = new int[6] { 0, 2, 1, 0, 3, 2 };

        meshA.RecalculateBounds();
        meshB.RecalculateBounds();
        

        GameObject gridPartElement = new GameObject("GridColliderA_" + grid.SubQuads.IndexOf(vertex3.SubQuad), typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer));
        gridPartElement.transform.parent = parent.transform;
        gridPartElement.GetComponent<MeshFilter>().mesh = meshA;
        gridPartElement.GetComponent<MeshCollider>().sharedMesh = meshA;
        gridPartElement.GetComponent<MeshRenderer>().material = material;
        gridPartElement.layer = LayerMask.NameToLayer("Cursor");

        GameObject gridPartElementB = new GameObject("GridColliderB_" + grid.SubQuads.IndexOf(vertex3.SubQuad), typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer));
        gridPartElementB.transform.parent = parent.transform;
        gridPartElementB.GetComponent<MeshFilter>().mesh = meshB;
        gridPartElementB.GetComponent<MeshCollider>().sharedMesh = meshB;
        gridPartElementB.GetComponent<MeshRenderer>().material = material;
        gridPartElementB.layer = LayerMask.NameToLayer("Cursor");

    }


    public void CreateCursor(List<Combine> combines, Material material, Material material_dark,Transform parent, GameObject currentCollider)
    {
        int colliderFace = -1;
        if(currentCollider != null && currentCollider.layer != LayerMask.NameToLayer("GroundCollider"))
        {
            colliderFace = currentCollider.GetComponent<ColliderSequences>().Queue;
        }

        var numberOfPart = combines.Count;

        List<Vector3> middlePoints = new List<Vector3>();
        List<Vector3> cornerPoints = new List<Vector3>();
        foreach (var combine in combines)
        {
            middlePoints.Add(combine.Neighbour[1].Vertex3);
            cornerPoints.Add(combine.Signal);
        }
        List<Vector3> upperPoints = new List<Vector3>();
        upperPoints.AddRange(CreateBilateralPoints(cornerPoints, elementHeight));
        upperPoints.AddRange(CreateBilateralPoints(middlePoints, elementHeight));

        List<Vector3> lowerPoints = new List<Vector3>();
        lowerPoints.AddRange(CreateBilateralPoints(cornerPoints, -elementHeight));
        lowerPoints.AddRange(CreateBilateralPoints(middlePoints, -elementHeight));


        //创建侧面collider
        for (int i = 0; i < numberOfPart; i++)
        {
            List<Vector3> oneDirection = new List<Vector3>();
            oneDirection.Add(lowerPoints[i]);
            oneDirection.Add(lowerPoints[i + numberOfPart]);
            oneDirection.Add(lowerPoints[(i + 1) % numberOfPart]);

            oneDirection.Add(upperPoints[i]);
            oneDirection.Add(upperPoints[i + numberOfPart]);
            oneDirection.Add(upperPoints[(i + 1) % numberOfPart]);

            int[] triangles = new int[] { 0, 1, 3, 1, 4, 3, 1, 2, 4, 2, 5, 4 };

            Mesh mesh = new Mesh();
            mesh.vertices = oneDirection.ToArray();
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            GameObject gridPartElement = new GameObject("Cursor_" + i, typeof(MeshFilter), typeof(MeshRenderer), typeof(ColliderSequences));
            gridPartElement.transform.parent = parent;

            gridPartElement.GetComponent<MeshFilter>().mesh = mesh;

            if(colliderFace == i && colliderFace != -1)
                gridPartElement.GetComponent<MeshRenderer>().material = material_dark;
            else
                gridPartElement.GetComponent<MeshRenderer>().material = material;

            gridPartElement.GetComponent<ColliderSequences>().Queue = i;
            gridPartElement.layer = LayerMask.NameToLayer("Cursor");
        }


        CreatePlanCursor(upperPoints, "upper", 1, combines[0], material, material_dark,parent, 10, colliderFace);
        CreatePlanCursor(lowerPoints, "lower", -1, combines[0], material, material_dark,parent, 11, colliderFace);

    }


    public void CreateGridCollier(List<Combine> combines)
    {
        var numberOfPart = combines.Count;

        List<Vector3> middlePoints = new List<Vector3>();
        List<Vector3> cornerPoints = new List<Vector3>();
        foreach (var combine in combines)
        {
            middlePoints.Add(combine.Neighbour[1].Vertex3);
            cornerPoints.Add(combine.Signal);
        }
        List<Vector3> upperPoints = new List<Vector3>();
        upperPoints.AddRange(CreateBilateralPoints(cornerPoints, elementHeight));
        upperPoints.AddRange(CreateBilateralPoints(middlePoints, elementHeight));

        List<Vector3> lowerPoints = new List<Vector3>();
        lowerPoints.AddRange(CreateBilateralPoints(cornerPoints, -elementHeight));
        lowerPoints.AddRange(CreateBilateralPoints(middlePoints, -elementHeight));


        //创建侧面collider
        for (int i = 0; i < numberOfPart; i++)
        {
            List<Vector3> oneDirection = new List<Vector3>();
            oneDirection.Add(lowerPoints[i]);
            oneDirection.Add(lowerPoints[i+ numberOfPart]);
            oneDirection.Add(lowerPoints[(i+ 1) % numberOfPart]);

            oneDirection.Add(upperPoints[i]);
            oneDirection.Add(upperPoints[i + numberOfPart]);
            oneDirection.Add(upperPoints[(i + 1) % numberOfPart]);

            int[] triangles = new int[] {0,1,3,1,4,3,1,2,4,2,5,4};

            Mesh mesh = new Mesh();
            mesh.vertices = oneDirection.ToArray();
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            GameObject gridPartElement = new GameObject("GridColliderSide_" + i, typeof(MeshCollider), typeof(GridColliderSides), typeof(MeshRenderer), typeof(ColliderSequences));
            gridPartElement.transform.parent = transform;

            gridPartElement.GetComponent<MeshCollider>().sharedMesh = mesh;
            gridPartElement.GetComponent<GridColliderSides>().Vertex = combines[i].Neighbour[1].Direction;
            gridPartElement.GetComponent<GridColliderSides>().Self = combines[i].Self;
            gridPartElement.GetComponent<GridColliderSides>().Y = combines[i].Y;
            gridPartElement.GetComponent<ColliderSequences>().Queue = i;
            gridPartElement.layer = LayerMask.NameToLayer("Collider_Sides");
        }

        //创建上下面collider
        CreatePlanCollider(upperPoints, "upper", 1, combines[0], 10);
        CreatePlanCollider(lowerPoints, "lower", -1, combines[0], 11);
    }

    public void CreatePlanCollider(List<Vector3> vector, string name,  int direction, Combine combine, int queue)
    {
        var numberOfPart = vector.Count / 2;
        
        List<int> triangles = new List<int>();
        for (int i = 0; i < numberOfPart; i++)
        {
            triangles.Add(i);
            triangles.Add(i + numberOfPart);
            triangles.Add((i - 1 + numberOfPart) % numberOfPart + numberOfPart);
        }
        for (int i = 0; i < numberOfPart - 2; i++)
        {
            triangles.Add(numberOfPart);
            triangles.Add(numberOfPart + i + 1);
            triangles.Add(numberOfPart + i + 2);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vector.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();

        GameObject gridPartElement = new GameObject(name + "_" + numberOfPart, typeof(MeshCollider), typeof(GridColliderDirectionVertical), typeof(MeshRenderer), typeof(ColliderSequences));
        gridPartElement.transform.parent = transform;

        gridPartElement.GetComponent<MeshCollider>().sharedMesh = mesh;
        gridPartElement.GetComponent<GridColliderDirectionVertical>().Direction = direction;
        gridPartElement.GetComponent<GridColliderDirectionVertical>().Self = combine.Self;
        gridPartElement.GetComponent<GridColliderDirectionVertical>().Y = combine.Y;
        gridPartElement.GetComponent<ColliderSequences>().Queue = queue;
        gridPartElement.layer = LayerMask.NameToLayer("Collider_TB");

    }



    public void CreatePlanCursor(List<Vector3> vector, string name, int direction, Combine combine, Material material, Material material_dark, Transform parent, int queue, int colliderFace)
    {
        var numberOfPart = vector.Count / 2;

        List<int> triangles = new List<int>();
        for (int i = 0; i < numberOfPart; i++)
        {
            triangles.Add(i);
            triangles.Add(i + numberOfPart);
            triangles.Add((i - 1 + numberOfPart) % numberOfPart + numberOfPart);
        }
        for (int i = 0; i < numberOfPart - 2; i++)
        {
            triangles.Add(numberOfPart);
            triangles.Add(numberOfPart + i + 1);
            triangles.Add(numberOfPart + i + 2);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vector.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();

        GameObject gridPartElement = new GameObject("Cursor_" + name, typeof(MeshFilter), typeof(MeshRenderer), typeof(ColliderSequences));
        gridPartElement.transform.parent = parent;

        gridPartElement.GetComponent<MeshFilter>().mesh = mesh;

        if (colliderFace == queue && colliderFace != -1)
            gridPartElement.GetComponent<MeshRenderer>().material = material_dark;
        else
            gridPartElement.GetComponent<MeshRenderer>().material = material;

        
        gridPartElement.GetComponent<ColliderSequences>().Queue = queue;
        gridPartElement.layer = LayerMask.NameToLayer("Cursor");

    }


    public List<Vector3> CreateBilateralPoints(List<Vector3> vectors, float height)
    {
        List<Vector3> points = new List<Vector3>();

        foreach (var vector in vectors)
        {
            points.Add(vector + Vector3.up * height / 2);
        }
        return points;
    }

}




//储存grid element对应方向的邻居
public class Neighbour
{
    public Vector3 Vertex3; //subquad在该方向的中点
    public Vertex Direction; //subqua在该方向的终点
    
}

public class Combine
{
    public Neighbour[] Neighbour = new Neighbour[2]; // 选中点的其他两个方向的邻居
    public Vector3 Signal; //位于中间的标志点，用于排序
    public Vertex Self;  //自身坐标
    public float Y;   //纵坐标的值
}

public class GridColliderSides: MonoBehaviour
{
    public Vertex Vertex;
    public Vertex Self;
    public float Y;
}
public class GridColliderDirectionVertical : MonoBehaviour
{
    public int Direction; //up:1, down:-1
    public Vertex Self;
    public float Y;
}

public class ColliderSequences : MonoBehaviour
{
    public int Queue;
}
