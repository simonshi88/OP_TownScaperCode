using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class InitializeGridValue : MonoBehaviour
{
    public GridManager grid;

    public TMP_Dropdown ring;

    public TMP_Dropdown floor;

    public Button Reset;

    public TransmitParameter Parameter;

    private int floor_value;
    private int ring_value;


    private int pre_ring;
    private int pre_floor;


    private void Awake()
    {
        if (Parameter.pre_ring_index == -1)
            Parameter.pre_ring_index = ring.value;
        else
            ring.value = Parameter.pre_ring_index;


        if (Parameter.pre_floor_index == -1)
            Parameter.pre_floor_index = floor.value;
        else
            floor.value = Parameter.pre_floor_index;
    }

    // Start is called before the first frame update
    void Start()
    {
        ring.onValueChanged.AddListener(SetRing);
        floor.onValueChanged.AddListener(SetFloor);

        Reset.onClick.AddListener(ResetGrid);



        ring_value = GetRingCount(ring.value);
        floor_value = GetFloor(floor.value);
        
    }

    private void SetFloor(int arg0)
    {
        floor_value = GetFloor(arg0);
        Parameter.pre_floor_index = arg0;
    }

    private void SetRing(int arg0)
    {

        ring_value = GetRingCount(arg0);
        Parameter.pre_ring_index = arg0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetGrid()
    {
        Debug.Log(ring_value + "  "  + floor_value);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Parameter.ring = ring_value;
        Parameter.floor = floor_value;
    }

    public int GetRingCount(int index)
    {
        
        switch (index)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 4;
            case 3:
                return 5;
            case 4:
                return 6;
            default:
                return 4;
            
        }
    }


    public int GetFloor(int index)
    {

        switch (index)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 4;
            case 3:
                return 5;
            case 4:
                return 6;
            default:
                return 5;

        }
    }
}
