using UnityEngine;

public static class bitMask 
{

    public static int GetBitMask(Cube cube)
    {
        int bitMask = 0;

        var vertices = cube.Vertices.ToArray();
        if (vertices[3] != null)
        {
            if (vertices[3].IsActive)
            {
                bitMask += 1;
            }
        }
        if (vertices[0] != null)
        {
            if (vertices[0].IsActive)
            {
                bitMask += 2;
            }
        }
        if (vertices[1] != null)
        {
            if (vertices[1].IsActive)
            {
                bitMask +=4;
            }
        }
        if (vertices[2] != null)
        {
            if (vertices[2].IsActive)
            {
                bitMask += 8;
            }
        }
        if (vertices[7] != null)
        {
            if (vertices[7].IsActive)
            {
                bitMask += 16;
            }
        }
        if (vertices[4] != null)
        {
            if (vertices[4].IsActive)
            {
                bitMask += 32;
            }
        }
        if (vertices[5] != null)
        {
            if (vertices[5].IsActive)
            {
                bitMask += 64;
            }
        }
        if (vertices[6] != null)
        {
            if (vertices[6].IsActive)
            {
                bitMask += 128;
            }
        }
        //Debug.Log(bitMask);

        return bitMask;
    }
}
