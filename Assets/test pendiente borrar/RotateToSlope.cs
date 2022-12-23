using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToSlope : MonoBehaviour
{
    
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 999))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }
}
