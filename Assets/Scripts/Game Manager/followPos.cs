using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPos : MonoBehaviour
{
    [SerializeField] private GameObject objToFollow;
    [SerializeField] private bool xAxis = true, yAxis = true, zAxis = true;
    [SerializeField] private bool smoothFollow;
    [SerializeField] private float smoothness;
    private Vector3 velocity;

    private void FixedUpdate()
    {
        if (!smoothFollow)
        {
            if (xAxis) gameObject.transform.position = new Vector3(objToFollow.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            if (yAxis) gameObject.transform.position = new Vector3(gameObject.transform.position.x, objToFollow.transform.position.y, gameObject.transform.position.z);
            if (zAxis) gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, objToFollow.transform.position.z);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, objToFollow.transform.position, ref velocity, smoothness);
        }
    }
}
