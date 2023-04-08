using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Walker 
{
    public Vector3 Origin;

    public int Length;
    public Vector3[] AllPoints;

    Queue<Vector3> Queue;


    Vector3 CurrentPosition;

    public Walker(Vector3 origin, int length, float boundary)
    {
        Origin = origin;
        
        Length = length;

        AllPoints = new Vector3[Length];
        
        Queue = new Queue<Vector3>();
        CurrentPosition = Origin;
    }


    double[] Walk(double x, double z, double step)
    {
        Random rand = new Random();

        double[] newPos = new double[2];
        int decision = rand.Next(4);

        if (decision == 0)
        {
            x += step;
        }
        else if (decision == 1)
        {
            x -= step;
        }
        else if (decision == 2)
        {
            z += step;
        }
        else
        {
            z -= step;
        }

        newPos[0] = x;
        newPos[1] = z;

        return newPos;
    }


    public Vector3 NextPosition(double step, float limitation)
    {
        Vector3 currentPosition = CurrentPosition;
        double[] xz = Walk(currentPosition.x, currentPosition.z, step);

        var x = (float)CheckBounds(xz[0], limitation);
        var z = (float)CheckBounds(xz[1], limitation);

        var nextPosition = new Vector3(x, 0, z);
        CurrentPosition = nextPosition;
        return nextPosition;
    }


    public void Update(Vector3 nextPosition)
    {
        AddQueue(Queue, nextPosition, Length);
        AllPoints = Queue.ToArray();
    }

    double CheckBounds(double x, double limit)
    {
        if (x > limit) return limit;
        else if (x < 0) return 0;
        else return x;
    }


    void AddQueue(Queue<Vector3> queue, Vector3 pt, int length)
    {

        if (queue.Count >= length)
            queue.Dequeue();
        queue.Enqueue(pt);

    }


}
