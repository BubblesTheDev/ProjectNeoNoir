using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class pistolPower : weaponPowerBase
{
    [Header("Power Variables")]
    [SerializeField] private float maxChargeTime;
    private float currentChargeTime;
    [SerializeField] private float chargeScaleMulti;
    [SerializeField] private float chargeScaleVel;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject firePoint;


    [Header("VFX Variables")]
    [SerializeField] private VisualEffect chargeEffect;
    [SerializeField] private GameObject shakingObj;
    [SerializeField] private float shakingIntensity;

    private Vector3 startingPos;
    InteractionInputActions inputActions;

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Awake()
    {
        inputActions = new InteractionInputActions();
        firePoint = GetComponent<weaponBase>().firePoint;
        startingPos = shakingObj.transform.localPosition;

    }

    private void FixedUpdate()
    {
        chargeShake();
    }

    public override IEnumerator usePower()
    {
        if (!canUsePower) yield break;

        canUsePower = false;

        while(inputActions.Combat.Fire2.IsPressed())
        {
            if (currentChargeTime >= maxChargeTime)
            {
                currentChargeTime = maxChargeTime;
                yield return null;
            }
            else currentChargeTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(fireSpecialBullet(currentChargeTime));

    }

    private IEnumerator fireSpecialBullet(float chargeTime)
    {
        GameObject temp = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation, GameObject.Find("Bullet Storage").transform);
        float chargePercent = currentChargeTime / maxChargeTime;

        shakingObj.transform.position = startingPos;

        temp.transform.localScale *= 1 + (chargeScaleMulti * chargePercent);
        temp.GetComponent<implosionBullet>().bulletEffect.SetFloat("Start Size", 1 + (chargeScaleMulti * chargePercent));
        temp.GetComponent<implosionBullet>().velocity *= 1 + (chargeScaleVel * chargePercent);
        //increase the projectile velocity based on charge percent
        currentChargeTime = 0;


        yield return new WaitForSeconds(powerCooldown);
        canUsePower = true;
    }


    void chargeShake()
    {
        float chargePercent = currentChargeTime / maxChargeTime;
        if(currentChargeTime > 0.001f) shakingObj.transform.localPosition = startingPos + (Random.insideUnitSphere * shakingIntensity * chargePercent);
    }
}
