using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class weaponBase : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float weaponDamage = 30;
    public float maxShotDistance;
    public int numOfPellets = 1;
    public float fireRateInSeconds = 0.25f;
    public int bulletPeirce;
    [Range(0, 15)]
    public float multiPelletAngle = 0;
    public float projectileSpeed = 250f;


    [Header("Weapon Settings")]
    public bool weaponIsEquipped;
    public LayerMask layersToIgnore;
    public bool canFire = true;
    public weaponPowerBase currentWeaponPower;
    public GameObject firePoint;
    public GameObject bulletPrefab;
    public GameObject gunGFX;

    InteractionInputActions interactionInput;
    cameraControl camControl;
    weaponVFXHandler weaponVFX;

    [HideInInspector] public UnityEvent gunFiredEvent;
    [HideInInspector] public UnityEvent powerActivated;

    private void Awake()
    {
        interactionInput = new InteractionInputActions();
        camControl = GameObject.Find("Player").GetComponent<cameraControl>();
        if (GetComponent<weaponVFXHandler>() != null) weaponVFX = GetComponent<weaponVFXHandler>();
        if (GetComponent<weaponPowerBase>() != null) currentWeaponPower = GetComponent<weaponPowerBase>();

        layersToIgnore = ~layersToIgnore;
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
        if (weaponIsEquipped)
        {
            gunGFX.SetActive(true);
            if (camControl.lookingDir.point != null) firePoint.transform.LookAt(camControl.lookingDir.point);

            if (canFire && interactionInput.Combat.Fire1.IsPressed() && !interactionInput.Combat.Fire2.IsPressed())
            {
                if (firePoint == null)
                {
                    Debug.LogWarning("You are missing a <b>Firepoint</b> object decleration");
                    Debug.Break();
                }
                StartCoroutine(fireGun());
            }
            if (currentWeaponPower == null)
            {
                Debug.LogWarning("You are missing a <b>current weapon power</b> decleration");
                return;
                if (currentWeaponPower.canUsePower && interactionInput.Combat.Fire2.WasPressedThisFrame() && !interactionInput.Combat.Fire1.IsPressed())
                {
                    powerActivated.Invoke();
                    StartCoroutine(currentWeaponPower.usePower());
                }
            }
        }
        else if (!weaponIsEquipped)
        {
            gunGFX.SetActive(false);
        }
    }

    IEnumerator fireGun()
    {
        canFire = false;

        gunFiredEvent.Invoke();
        weaponVFX.playMuzzleFlash();
        weaponVFX.playFireAnimation(fireRateInSeconds);

        for (int y = 0; y < numOfPellets; y++)
        {

            //Detects if the weapon is firing a projectile by seeing if there is a prefab to fire or not
            if (bulletPrefab == null)
            {
                
                List<RaycastHit> thingsHit = new List<RaycastHit>();

                if (multiPelletAngle == 0 || y == 0) thingsHit.AddRange(Physics.RaycastAll(camControl.CameraObj.transform.position, camControl.CameraObj.transform.forward, maxShotDistance, layersToIgnore).ToList());
                else
                {
                    thingsHit.AddRange(Physics.RaycastAll(camControl.CameraObj.transform.position, Quaternion.Euler(Random.Range(-multiPelletAngle, multiPelletAngle), Random.Range(-multiPelletAngle, multiPelletAngle), 0) * camControl.CameraObj.transform.forward, maxShotDistance, layersToIgnore).ToList());
                }

                if(thingsHit.Count > 0)
                {
                    thingsHit.OrderBy(RaycastHit => Vector3.Distance(this.transform.localPosition, RaycastHit.point)).ToList();

                    for (int i = 0; i < bulletPeirce+1; i++)
                    {
                        if (thingsHit.Count - 1 == i) break;
                        if (thingsHit[i].collider.CompareTag("Enemy"))
                        {
                            enemyStats tempReference = thingsHit[i].collider.GetComponent<enemyStats>();
                            tempReference.takeDamage(weaponDamage);
                        }
                        else if (thingsHit[i].collider.gameObject.layer == LayerMask.NameToLayer("Walkable"))
                        {
                            break;
                        }
                    }

                    if (thingsHit.Count > bulletPeirce)
                    {
                        StartCoroutine(weaponVFX.spawnTrail(thingsHit[bulletPeirce]));
                    } else if(thingsHit.Count == 0) StartCoroutine(weaponVFX.spawnTrail(camControl.lookingDir));
                    else StartCoroutine(weaponVFX.spawnTrail(thingsHit[thingsHit.Count-1]));
                    //Call spawntrail function from weaponVFX scripts
                }
            }
            else if (bulletPrefab != null)
            {
                GameObject tempBullet = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation * Quaternion.Euler(Random.Range(-multiPelletAngle, multiPelletAngle), Random.Range(-multiPelletAngle, multiPelletAngle), 0), GameObject.Find("Bullet Storage").transform);

                tempBullet.GetComponent<playerProjectileBase>().loadStats(weaponDamage, projectileSpeed, maxShotDistance, bulletPeirce); 
            }
        }

        yield return new WaitForSeconds(fireRateInSeconds);
        canFire = true;
    }


    private void OnDrawGizmosSelected()
    {
    }
}
