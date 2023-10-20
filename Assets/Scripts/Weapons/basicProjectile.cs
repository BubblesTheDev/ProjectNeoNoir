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
        if(collision.collider.CompareTag("Player"))
        {
            StartCoroutine(collision.gameObject.GetComponent<playerHealth>().takeDamage(1));
        }
        Destroy(gameObject);
    }
}
