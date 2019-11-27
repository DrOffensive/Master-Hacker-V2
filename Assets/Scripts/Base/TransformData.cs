using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TransformData
{
    Vector3 position;
    Quaternion rotation;

    public TransformData(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }

    public TransformData(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public Vector3 Position { get => position; }
    public Quaternion Rotation { get => rotation; }
}
