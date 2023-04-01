using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkOnGround : MonoBehaviour
{
    public GameObject Cube;

    public int Number = 3;
    public int Length = 10;

    public float Boundary = 20.0f;

    public List<Walker> walkers;
    public List<GameObject> cubes;

    public float second;
    float speed;


    // Start is called before the first frame update
    void Start()
    {
        walkers = new List<Walker>();
        cubes = new List<GameObject>();

        for (int i = 0; i < Number; i++)
        {
            var origin = GenerateRandomPoint(Boundary);
            var walker = new Walker(origin,Length,Boundary);
            walkers.Add(walker);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(speed >= second)
        {
            Clear();

            for (int i = 0; i < walkers.Count; i++)
            {
                var nextPosition = walkers[i].NextPosition(1, Boundary);
                walkers[i].Update(nextPosition);

                GenerateCubes(walkers[i].AllPoints);
            }

            speed = 0.0f;
        }

        
        speed += Time.deltaTime;



    }

    public Vector3 GenerateRandomPoint(float boundary)
    {
        var x = Random.value * boundary;
        var z = Random.value * boundary;

        Vector3 vector3 = new Vector3(x,0,z);

        return vector3;
    }

    public void GenerateCubes(Vector3[] points)
    {
        foreach (var point in points)
        {           
            GameObject cube = Instantiate(Cube, point, Quaternion.identity);

            cubes.Add(cube);
        }
        
    }

    public void Clear()
    {
        
        foreach (var cube in cubes)
        {
            Destroy(cube);
        }
        cubes.Clear();

    }
}
