using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Module
{
    public string Name;
    public Mesh Mesh;

    public int Rotation;
    public bool Flip;

    public Module(string name, Mesh mesh, int rotation, bool flip)
    {
        Name = name;
        Mesh = mesh;
        Rotation = rotation;
        Flip = flip;
    }
}
