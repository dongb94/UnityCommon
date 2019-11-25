using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform Target;
    [SerializeField]
    private float Radius;
    [SerializeField][Range(0,1)]
    private float CameraSpeed;
    [SerializeField]
    private Vector3 CameraPosition;
    
    // Update is called once per frame
    void LateUpdate()
    {
        var direction = transform.position - Target.position - CameraPosition;
        var magnitude = direction.magnitude;
        var difference = magnitude - Radius;

        if (difference > 0f)
        {
            transform.position = Target.position + direction.normalized * (Radius - 0.01f) + CameraPosition;
        }
        else// if (difference > -0.1f)
        {
            transform.position -= direction * CameraSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position - CameraPosition, Radius);
    }
}
