using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class weaponBase : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float weaponDamage = 30;
    public int shotsPerFire = 1;
    public float fireRate = .1f;
    public int numOfPellets = 1;
    public float fireCooldown = 0.25f;
    [Range(0, 15)]
    public float multiPelletAngle = 0;
    public float muzzleVelocity = 250f;


    [Header("Weapon Settings")]
    public LayerMask layersToHit;
    public bool canFire = true;
    public weaponPowerBase currentWeaponPower;
    public GameObject firePoint;
    public GameObject bulletPrefab;
    public GameObject gunGFX;

    RaycastHit shotHit;
    InteractionInputActions interactionInput;
    cameraControl camControl;


    private void Awake()
    {
        interactionInput = new InteractionInputActions();

        if (GetComponent<weaponPowerBase>() != null) currentWeaponPower = GetComponent<weaponPowerBase>();
        if (currentWeaponPower == null || firePoint == null)
        {
            Debug.LogWarning("You are either missing a <b>FirePoint</b> game object decleration \n Or, you are missing a <b>current weapon power</b> decleration");
            Debug.Break();
        }

        camControl = GameObject.Find("Player").GetComponent<cameraControl>();
    }

    private void OnEnable()
    {
        interactionInput.Enable();
    }

    private void OnDisable()
    {
        interactionInput.Disable();
    }

    private void Update()
    {
        if (camControl.lookingDir.point != null) firePoint.transform.LookAt(camControl.lookingDir.point);

        if(canFire && interactionInput.Combat.Fire1.IsPressed())
        {
            StartCoroutine(fireGun());
        }
        if (currentWeaponPower.canUsePower && interactionInput.Combat.Fire2.IsPressed())
        {
            StartCoroutine(currentWeaponPower.usePower());
        }
    }

    

    



    IEnumerator fireGun()
    {
        //print("Starting to fire: " + gameObject.name);
        canFire = false;
        for (int x = 0; x < shotsPerFire; x++)
        {

            //print("Firing shot " + x + " of " + shotsPerFire);
            for (int y = 0; y < numOfPellets; y++)
            {
                //print("Shooting pellet " + y + " of " + numOfPellets);

                //Detects if the weapon is firing a projectile by seeing if there is a prefab to fire or not
                if (bulletPrefab == null)
                {
                    //Play muzzle flash particle effect
                    //play gun sound here
                    //play fire animation
                    AudioManager.instance.PlayGunShot(FMODEvents.instance.pistolShot, this.transform.position);



                    if (multiPelletAngle == 0) Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out shotHit, Mathf.Infinity, layersToHit);
                    else
                    {
                        Physics.Raycast(firePoint.transform.position,  Quaternion.Euler(Random.Range(-multiPelletAngle, multiPelletAngle), Random.Range(-multiPelletAngle, multiPelletAngle), 0) * firePoint.transform.forward , out shotHit, Mathf.Infinity, layersToHit);
                    }

                    if (shotHit.collider != null)
                    {
                        //play bullet hit particle,
                        //spawn decal for bullet hole


                        if (shotHit.collider.CompareTag("Enemy"))
                        {
                            print("Hit " + shotHit.collider.gameObject.name + " enemy, dealt " + weaponDamage + " damage to it");
                        }


                        //get component for enemy STATS,
                        //call deal damage function to enemy
                        //it handles the rest

                        GameObject debugObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        debugObj.transform.position = shotHit.point;
                        debugObj.transform.localScale *= .1f;
                    }
                }
                else
                {
                    //Play muzzle flash particle effect
                    //play gun sound here
                    //play fire animation
                    AudioManager.instance.PlayGunShot(FMODEvents.instance.pistolShot, this.transform.position);

                    GameObject tempBullet = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation * Quaternion.Euler(Random.Range(-multiPelletAngle, multiPelletAngle), Random.Range(-multiPelletAngle, multiPelletAngle), 0), GameObject.Find("Bullet Storage").transform);
                    if (tempBullet.GetComponent<Rigidbody>() != null)
                    {
                        Rigidbody bulletRb = tempBullet.GetComponent<Rigidbody>();
                        bulletRb.useGravity = false;
                        bulletRb.AddForce(tempBullet.transform.forward * muzzleVelocity, ForceMode.Impulse);
                    }

                    //Get bullet stats script
                    //add stats and modifiers
                    //Dmg, modifiers, etc
                }
            }

            yield return new WaitForSeconds(fireRate);
        }

        yield return new WaitForSeconds(fireCooldown - (fireRate * shotsPerFire));

        canFire = true;
    }


    private void OnDrawGizmosSelected()
    {
        if (canFire) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        if(camControl != null)
        {
            if (camControl.lookingDir.point != null) Gizmos.DrawLine(firePoint.transform.position, camControl.lookingDir.point);
        }

        Gizmos.color = Color.white;
        if (multiPelletAngle > 0)
        {
            Gizmos.DrawLine(firePoint.transform.position, Quaternion.Euler(multiPelletAngle, 0, 0) * firePoint.transform.forward * 3f);
            Gizmos.DrawLine(firePoint.transform.position, Quaternion.Euler(-multiPelletAngle, 0, 0) * firePoint.transform.forward * 3f);
            Gizmos.DrawLine(firePoint.transform.position, Quaternion.Euler(0, multiPelletAngle, 0) * firePoint.transform.forward * 3f);
            Gizmos.DrawLine(firePoint.transform.position, Quaternion.Euler(0, -multiPelletAngle, 0) * firePoint.transform.forward * 3f);
        }
    }

}

public enum statusEffects
{
    normal,
    oil,
    electric,
    fire
}
