using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotgunPower : weaponPowerBase
{
    public float numShots;
    public float fireRate;
    public float burstCount;
    public float damage;
    public float range;
    statusEffects effectToGive;
    LayerMask layersToHit;
    [SerializeField] private GameObject hydraTurret;
    private GameObject currentTurret;

    [SerializeField] private GameObject firePoint;

    private void Awake()
    {
        layersToHit = ~layersToHit;
    }

    public override IEnumerator usePower()
    {
        if(canUsePower && numCharges < 0)
        {
            canUsePower = false;

            RaycastHit hit;
            Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, Mathf.Infinity, layersToHit.value);
            if (hit.point != null)
            {
                Quaternion rot = Quaternion.LookRotation(hydraTurret.transform.forward, hit.normal);
                currentTurret = Instantiate(hydraTurret, hit.point, rot, GameObject.Find("Bullet Storage").transform);
                currentTurret.GetComponent<hydraTurret>().applyStats(damage, fireRate, burstCount, range, effectToGive);

                currentCharges--;
            }

            yield return new WaitForSeconds(powerCooldown);
            canUsePower = true;
        }


    }


}
