using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class revolverGrenade : MonoBehaviour
{
    public int explosionDmg;
    public float explosionRadius;
    public float timeToExplode;
    public statusEffects effectsToGive;
    [SerializeField] private LayerMask layersToHit;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy")) StartCoroutine(explode(0f));
        else if (collision.collider.CompareTag("Untagged")) StartCoroutine(explode(timeToExplode));
    }

    public IEnumerator explode(float timeBeforeExplosion)
    {
        List<Collider> enmies = Physics.OverlapSphere(transform.position, explosionRadius, layersToHit).ToList();
        yield return new WaitForSeconds(timeBeforeExplosion);


        foreach (Collider enemy in enmies)
        {
            //Get enemy stats and deal dmg, 
            //apply the correct status effects
            //apply knockback

            //if player, only deal 1 dmg, and apply 1.5x knockback

            //Call on explosion modifiers
        }


        Destroy(gameObject);
    }
}
