using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followRot : MonoBehaviour
{
    [SerializeField] private GameObject objToFollow;
    [SerializeField] private bool xRot, yRot, zRot;

    private void Update()
    {
        if(xRot) transform.rotation = Quaternion.Euler(objToFollow.transform.rotation.x, transform.rotation.y, transform.rotation.z);
        if(yRot) transform.rotation = Quaternion.Euler(transform.rotation.x, objToFollow.transform.rotation.y, transform.rotation.z);
        if(zRot) transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, objToFollow.transform.rotation.z);
    }
}
