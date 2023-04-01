using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstScript : MonoBehaviour
{
    public List<GameObject> walls;

    public GameObject Sphere;
    public Transform BirthPoint;

    public Material blue;
    public Material green;
    public List<GameObject> spheres;

    // Start is called before the first frame update
    void Start()
    {
        spheres = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spheres.Count > 10)
        {           
            ChangerMaterial(walls, blue);  
        }
        if (spheres.Count > 30)
        {
            ChangerMaterial(walls, green);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Color randomColor = GetRandomColorWithAlpha();

        
        GetComponent<Renderer>().material.color = randomColor;

        GameObject newSphere = InstantiateSphere(BirthPoint.position);

        spheres.Add(newSphere);
    }

    public Color GetRandomColorWithAlpha()
    {
        return new Color(r: Random.Range(0f, 1f),
            g: Random.Range(0f, 1f),
            b: Random.Range(0f, 1f),
            a: 0.25f);
    }


    public GameObject InstantiateSphere(Vector3 vector)
    {
        return Instantiate(Sphere, vector, Quaternion.identity);
    }


    public void ChangerMaterial(List<GameObject> walls, Material material)
    {
        foreach (var wall in walls)
        {
            wall.GetComponent<MeshRenderer>().material = new Material(material);
        }
    }
}
