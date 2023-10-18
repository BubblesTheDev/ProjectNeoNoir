using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicPlayerBullet : MonoBehaviour
{
    public float damage;
    public float bulletSpeed;
    public float bulletLifeTime;
    public statusEffects effectOfBullet;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, bulletLifeTime);
    }


    private void FixedUpdate()
    {
        rb.velocity = transform.forward * bulletSpeed * Time.deltaTime;
    }



    private void OnCollisionEnter(Collision collision)
    {

        //play on enviroment modifiers

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponentInParent<enemyStats>().takeDamage(damage, effectOfBullet);

            //Play on hit modifiers
        }

        Destroy(gameObject);
    }
}
