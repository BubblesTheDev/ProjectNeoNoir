using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class revolverPower : weaponPowerBase
{
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject objToThrow;
    public float throwForce = 20f;
    public float throwTorque = 5f;
    public int explosionDmg = 3;
    public float explosionRange = 5f;
    public int numOfGunsThrown = 1;
    public statusEffects statusToGive;


    public override IEnumerator usePower()
    {
        if(canUsePower && currentCharges > 0)
        {
            canUsePower = false;


            for (int i = 0; i < numOfGunsThrown; i++)
            {
                GameObject tempObj = Instantiate(objToThrow, spawnPoint.transform.position, Quaternion.identity, GameObject.Find("Bullet Storage").transform);
                Rigidbody tempRB = tempObj.GetComponent<Rigidbody>();
                tempRB.AddForce(spawnPoint.transform.forward * throwForce, ForceMode.Impulse);
                tempRB.AddTorque(spawnPoint.transform.right * throwTorque, ForceMode.Impulse);

                //add stats to the gun script upon spawning

                yield return new WaitForSeconds(0.05f);
            }



            yield return new WaitForSeconds(powerCooldown - (0.05f * numOfGunsThrown));

            canUsePower = true;
        }
    }
}
