using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmitParameter : MonoBehaviour
{
    public int ring;

    public int floor;

    public int pre_ring_index = -1;
    public int pre_floor_index = -1;

    static TransmitParameter m_instance;
    public static TransmitParameter Instance => m_instance;


    private void Awake()
    {
        CreateSingleton();
        DontDestroyOnLoad(this);
    }
        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateSingleton()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }
}
