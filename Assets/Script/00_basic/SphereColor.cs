using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereColor : MonoBehaviour
{
    public float ForceAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {            
            var rigidbody = this.GetComponent<Rigidbody>();
            rigidbody.AddForce(Vector3.up * ForceAmount * 10000);

            Debug.Log(rigidbody.ToString());
        }
    }



    void OnCollisionEnter(Collision collision)
    {
        Color randomColor = GetRandomColor();
        GetComponent<Renderer>().material.color = randomColor;
    }

    public Color GetRandomColor()
    {
        return new Color(r: Random.Range(0f, 1f),
            g: Random.Range(0f, 1f),
            b: Random.Range(0f, 1f));
    }
}
