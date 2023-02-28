using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm 
{
    /// <summary>
    /// Judge whether a point is inside or outside a polygon
    /// </summary>
    /// <param name="p"></param>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static bool IsPointInPolygon(Vector3 p, Vector3[] polygon)
    {
        float minX = polygon[0].x;
        float maxX = polygon[0].x;
        float minY = polygon[0].z;
        float maxY = polygon[0].z;
        for (int i = 1; i < polygon.Length; i++)
        {
            Vector3 q = polygon[i];
            minX = Mathf.Min(q.x, minX);
            maxX = Mathf.Max(q.x, maxX);
            minY = Mathf.Min(q.z, minY);
            maxY = Mathf.Max(q.z, maxY);
        }

        if (p.x < minX || p.x > maxX || p.z < minY || p.z > maxY)
        {
            return false;
        }

        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if ((polygon[i].z > p.z) != (polygon[j].z > p.z) &&
              p.x < (polygon[j].x - polygon[i].x) * (p.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x)
            {
                inside = !inside;
            }
        }

        return inside;
    }



}
