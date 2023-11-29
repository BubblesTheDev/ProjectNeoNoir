using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunnerProjectile : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed, distanceToKill;
    private Vector3 startPos;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed * Time.deltaTime;
        startPos = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(startPos, transform.position) > distanceToKill) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) StartCoroutine(other.gameObject.GetComponent<playerHealth>().takeDamage(damage));
        Destroy(gameObject);
    }

}
