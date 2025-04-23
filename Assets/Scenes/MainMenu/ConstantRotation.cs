using System;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public float Speed = 1f;
    public Vector3 Rotation = new Vector3(1, 0, 0);

    private void Update()
    {
        transform.Rotate(Rotation, Speed * Time.deltaTime);
    }
}