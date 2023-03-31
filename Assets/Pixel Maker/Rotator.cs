using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float RotationSpeed = 0;
    
    void FixedUpdate()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up * RotationSpeed * Time.fixedDeltaTime);    
    }
}
