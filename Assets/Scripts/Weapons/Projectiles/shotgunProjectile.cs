using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shotgunProjectile : playerProjectileBase
{

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startingPos = transform.position;
        projectileDead = false;
        layersToIgnore = ~layersToIgnore;
    }   

    private void Update()
    {
        if (Vector3.Distance(startingPos,transform.position) > maxShotDistance)
        {
            killProjectile();
        }

    }

    private void FixedUpdate()
    {
        if(!projectileDead) rb.velocity = transform.forward * projectileSpeed * Time.deltaTime;
    }

    public override void loadStats(float damage, float speed, float distance, int peirce)
    {
        projectileDamage = damage;
        projectilePeirce = peirce;
        projectileSpeed = speed;
        maxShotDistance = distance;
    }

    void killProjectile()
    {
        projectileDead = true;
        rb.isKinematic = true;
        projectileDead = true;
        if(transform.childCount >0) Destroy(transform.GetChild(0).gameObject);
        Destroy(gameObject, 1f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (projectileDead) return;
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<enemyStats>().takeDamage(projectileDamage);
            if (projectilePeirce > 0) projectilePeirce--;
            else if (projectilePeirce <= 0) killProjectile();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walkable")) killProjectile();
    }

}
