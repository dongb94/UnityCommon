using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform Target;
    public float Radius;
    public Vector3 CameraPosition;
    
    // Update is called once per frame
    void LateUpdate()
    {
        var direction = transform.position - Target.position - CameraPosition;
        var magnitude = direction.magnitude;
        var difference = magnitude - Radius;

        if (magnitude > 0f)
        {
            transform.position = Target.position + direction.normalized * Radius + CameraPosition;
        }
        else if (magnitude > -0.1f)
        {
            transform.position -= direction * 0.1f + CameraPosition;
        }
    }
}
