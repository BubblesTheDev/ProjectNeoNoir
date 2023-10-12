using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 15f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
