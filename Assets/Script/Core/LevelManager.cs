using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    static LevelManager m_instance;
    public static LevelManager Instance => m_instance;

    public GroundCollider GroundCollider;

    public GridElement GridElement;

    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        GenerateCollier();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCollier()
    {
        GroundCollider.CreateGroundCollider(GridManager.Instance.grid);

    }

    public void RandomSet()
    {
        var gridCollider = transform.Find("Grid Collider");

        if (gridCollider != null)
        {
            var vertex3 = GridManager.Instance.verticesAll;
            vertex3.ForEach(x => x.IsActive = false);

            for (int i = 0; i < gridCollider.childCount; i++)
            {
                Destroy(gridCollider.GetChild(i).gameObject);
                Resources.UnloadUnusedAssets();
            }
        }

        var slot = GameObject.Find("Grid").transform.Find("Slot");

        if(slot != null)
        {
            for (int i = 0; i < slot.childCount; i++)
            {
                Destroy(slot.GetChild(i).gameObject);
                Resources.UnloadUnusedAssets();
            }
        }


        var grid = GridManager.Instance.grid;

        for (int i = 0; i < grid.Floor - 1; i++)
        {
            foreach (Vertex vertex in grid.Vertices)
            {
                if (!vertex.IsBoundary)
                {

                    if (Random.value < 0.3f)
                    {
                        UpdateGridElement(vertex, i * grid.Height, Operation.LeftClick, null);
                    }

                }
            }

        }
    }



    public void UpdateGridElement(Vertex vertex, float height, Operation operation, GameObject currentObject)
    {
        List<Vertex> vertices = GridManager.Instance.vertices;

        string name = "GridElement_" + vertices.IndexOf(vertex) + "_" + height;

        GameObject grid_gameObject;

        if (transform.GetChild(1).Find(name))
        {
            grid_gameObject = transform.GetChild(1).Find(name).gameObject;
        }
        else
            grid_gameObject = null;

        if (grid_gameObject == null)
        {
            if(operation == Operation.LeftClick)
            {
                Vector3 vector3 = vertex.Position + Vector3.up * height;

                grid_gameObject = new GameObject(name, typeof(GridElement));
                grid_gameObject.transform.SetParent(transform.GetChild(1));
                grid_gameObject.transform.localPosition = transform.GetChild(1).position;

                GridElement gridElement = grid_gameObject.GetComponent<GridElement>();
                gridElement.Initialize();

                var combines = gridElement.CreateGridElement(vertex, height);

                gridElement.CreateGridCollier(combines);

                var vertex3 = GridManager.Instance.grid.GetVertex3(vertex, height);
                vertex3.ForEach(x => x.IsActive = true);

            }

        }
        else
        {
            //Debug.Log(grid_gameObject.name);

            if (operation == Operation.LeftClick)
            {
                if(currentObject != null)
                {
                    if (currentObject.layer == LayerMask.NameToLayer("Collider_TB"))
                    {
                        int direction = currentObject.GetComponent<GridColliderDirectionVertical>().Direction;
                        Vertex vertex_next = currentObject.GetComponent<GridColliderDirectionVertical>().Self;
                        float vertex_y_next = currentObject.GetComponent<GridColliderDirectionVertical>().Y;

                        var y_next = vertex_y_next + direction * GridManager.Instance.grid.Height;

                        if (y_next < GridManager.Instance.grid.Height * (GridManager.Instance.grid.Floor -1))
                            UpdateGridElement(vertex_next, y_next, Operation.LeftClick, currentObject);

                    }

                    if (currentObject.layer == LayerMask.NameToLayer("Collider_Sides"))
                    {
                        Vertex vertex_next = currentObject.GetComponent<GridColliderSides>().Vertex;
                        float vertex_y = currentObject.GetComponent<GridColliderSides>().Y;
                        

                        UpdateGridElement(vertex_next, vertex_y, Operation.LeftClick, currentObject);

                    }
                }
            }


            if (operation == Operation.RightClick)
            {
                var vertex3 = GridManager.Instance.grid.GetVertex3(vertex, height);
                vertex3.ForEach(x => x.IsActive = false);


                Destroy(grid_gameObject);
                Resources.UnloadUnusedAssets();
            }
            
        }
    }
}
