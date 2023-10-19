using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicEnemyProjectile : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float bulletLifetime = 15f;
    [SerializeField] private int damage = 1;

    private void Awake()
    {
        Destroy(gameObject, bulletLifetime);
    }

    private void Update()
    {
        rb.velocity = transform.forward * moveSpeed * Time.deltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<playerHealth>().takeDamage(damage);
            Destroy(gameObject);
        }
    }
}
