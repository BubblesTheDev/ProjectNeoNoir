using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hydraTurret : MonoBehaviour
{
    public float damage;
    public float fireRate;
    public float burstNum;
    public float range;
    statusEffects effectTurretApplies;

    private List<GameObject> enemiesInRange;
    private bool canFire = true;
    [SerializeField] private LayerMask layersToHit;



    private void Awake()
    {
        layersToHit = ~layersToHit;
    }

    private void Update()
    {
        if (canFire && enemiesInRange.Count > 0) StartCoroutine(fireShot());
    }

    public void applyStats(float turretDmg, float turretFireRate, float turretBurstNum, float turretRange, statusEffects turretShotEffect)
    {
        damage = turretDmg;
        fireRate = turretFireRate;
        burstNum = turretBurstNum;
        range = turretRange;
        effectTurretApplies = turretShotEffect;
    }

    private IEnumerator fireShot()
    {
        canFire = false;
        for (int i = 0; i < burstNum; )
        {
            RaycastHit hydraHit;
            Physics.Raycast(transform.position, (enemiesInRange[Random.Range(0, enemiesInRange.Count)].transform.position - transform.position).normalized, out hydraHit, Mathf.Infinity, layersToHit);


            if (hydraHit.collider.CompareTag("Enemy"))
            {
                hydraHit.collider.GetComponent<enemyStats>().takeDamage(damage, effectTurretApplies);
                i++;
            }
            yield return null;
        }

        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) enemiesInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) enemiesInRange.Remove(other.gameObject);
    }
}
