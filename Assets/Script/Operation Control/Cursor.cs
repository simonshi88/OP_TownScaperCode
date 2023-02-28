using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Operation
{
    Cursor,
    LeftClick,
    RightClick
}

public class Cursor : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject _curGameObject;

    private Vertex currentVertex;
    private Vertex preVertex;
    private float preY;

    private Vertex preVertex_click;
    private float preY_click;

    //public delegate void OnItemAdded(ItemData item);
    //public static event OnItemAdded OnItemAddedEvent;

    public Grid grid;

    public Material material;
    public Material material_dark;
    public GridElement gridElement;

    private List<Combine> combines = new List<Combine>();

    List<Vertex3> vertex3 = new List<Vertex3>();

    private bool enterUI;


    // Start is called before the first frame update
    void Start()
    {
        grid = GridManager.Instance.grid;
        //gridElement = new GridElement();
        gridElement.Initialize();

        preVertex = GetTargetVertex(out preY);
        preVertex_click = GetTargetVertex(out preY_click);
    }

  
    // Update is called once per frame
    void Update()
    {
        enterUI = DetectMouse.enterUI;

        vertex3.Clear();

        float y;
        if (GetTargetVertex(out y) == null)
            return;

        Vertex vertex = GetTargetVertex(out y);

        vertex3 = grid.GetVertex3(vertex, y);
        foreach (var item in vertex3)
        {
            if (item.IsBoundary)
                return;
        }

        UpdateCursor();
     
        if (Input.GetMouseButtonDown(0) && !enterUI)
        {

            //foreach (var item in vertex3)
            //{
            //    item.IsActive = true;               
            //}
            interactionSound.instance.SetAudioPlay((int)Math.Round(y) + 1);

            LevelManager.Instance.UpdateGridElement(vertex, y, Operation.LeftClick, _curGameObject);

        }
        if (Input.GetMouseButtonDown(1) && !enterUI)
        {

            //foreach (var item in vertex3)
            //{
            //    item.IsActive = false;
            //}
            interactionSound.instance.SetAudioPlay((int)Math.Round(y) + 1);
            LevelManager.Instance.UpdateGridElement(vertex, y, Operation.RightClick, _curGameObject);
            //grid.GetVertex3 (GetTargetVertex(), 0).IsActive = true;
        }




    }

    public Vertex GetTargetVertex(out float y)
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("GroundCollider");
        layerMask += 1 << LayerMask.NameToLayer("Collider_TB");
        layerMask += 1 << LayerMask.NameToLayer("Collider_Sides");

        if (Physics.Raycast(ray, out hit, 10000f, layerMask))
        {           
            _curGameObject = hit.transform.gameObject;
            if(_curGameObject.layer == LayerMask.NameToLayer("GroundCollider"))
            {
                SubQuad subQuad = _curGameObject.GetComponent<GroundColliderQuad>().SubQuad;
                //Debug.Log(hit.point);
                y = 0f;
                return GetTarget(subQuad, hit.point);
            }
               
            if (_curGameObject.layer == LayerMask.NameToLayer("Collider_TB"))
            {
                int direction = _curGameObject.GetComponent<GridColliderDirectionVertical>().Direction;
                Vertex vertex = _curGameObject.GetComponent<GridColliderDirectionVertical>().Self;
                float vertex_y = _curGameObject.GetComponent<GridColliderDirectionVertical>().Y;

                //Debug.Log("top");
                //y = vertex_y + direction * grid.Height;
                y = vertex_y;
                return vertex;
            }

            if (_curGameObject.layer == LayerMask.NameToLayer("Collider_Sides"))
            {
                Vertex vertex = _curGameObject.GetComponent<GridColliderSides>().Self;
                float vertex_y = _curGameObject.GetComponent<GridColliderSides>().Y;

                // Debug.Log("sides");
                y = vertex_y;
                return vertex;
            }
 


            


        }

        y = 0f;
        return null;
    }


    public Vertex GetTarget(SubQuad subQuad, Vector3 hitPoint)
    {
        Vector3 a = subQuad.Vertices[0].Position;
        Vector3 b = subQuad.Vertices[1].Position;
        Vector3 c = subQuad.Vertices[2].Position;
        Vector3 d = subQuad.Vertices[3].Position;

        Vector3 ab = (a + b) / 2;
        Vector3 bc = (b + c) / 2;
        Vector3 cd = (c + d)/ 2;
        Vector3 da = (d + a) / 2;
        Vector3 center = (a + b + c + d) / 4;

        Vector3[] vectorA = new Vector3[4] { a, ab, center, da };
        Vector3[] vectorB = new Vector3[4] { b, bc, center, ab };
        Vector3[] vectorC = new Vector3[4] { c, cd, center, bc };
        Vector3[] vectorD = new Vector3[4] { d, da, center, cd };

        if (Algorithm.IsPointInPolygon(hitPoint, vectorA))
            return subQuad.Vertices[0];
        else if (Algorithm.IsPointInPolygon(hitPoint, vectorB))
            return subQuad.Vertices[1];
        else if (Algorithm.IsPointInPolygon(hitPoint, vectorC))
            return subQuad.Vertices[2];
        else if (Algorithm.IsPointInPolygon(hitPoint, vectorD))
            return subQuad.Vertices[3];
        //else if (grid.FindVertex(hitPoint).IsBoundary)
        //    return null;
        else
            return null;

    }

    public void Create()
    {

    }

    public void Delete()
    {

    }

    public void UpdateCursor()
    {
        float y;
        var vectex = GetTargetVertex(out y);
        currentVertex = vectex;

        if (preVertex != currentVertex || preY != y)
        {
            preVertex = currentVertex;
            vertex3 = grid.GetVertex3(currentVertex, y);
        }
     

        if (transform.childCount != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        if(vectex != null)
        {
            combines = gridElement.CreateGridElement(vectex, y);

            gridElement.CreateCursor(combines, material, material_dark,transform, _curGameObject);
        }


        
    }


    private void OnDrawGizmos()
    {
        //foreach (var vertex in vertex3)
        //{
        //    Gizmos.color = Color.red;

        //    Gizmos.DrawSphere(vertex.SubQuad.Vertices[0].Position, 1f);
        //    Gizmos.DrawSphere(vertex.SubQuad.Vertices[1].Position, 1f);
        //    Gizmos.DrawSphere(vertex.SubQuad.Vertices[2].Position, 1f);
        //    Gizmos.DrawSphere(vertex.SubQuad.Vertices[3].Position, 1f);
        //}


        //for (int i = 0; i < combines.Count; i++)
        //{
        //    var combine = combines[i];
        //    var a = grid.GetVertex3(combine.Neighbour[0].Direction, 0);
        //    var b = grid.GetVertex3(combine.Neighbour[1].Direction, 0);

        //    GUI.color = Color.yellow;
        //    Handles.Label(a[0].worldPosition, i.ToString());
 
        //}

    }



}
