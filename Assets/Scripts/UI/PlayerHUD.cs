using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHUD : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private GameObject HUD;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider healthDrain;
    [SerializeField] private Slider staticMeter;
    [SerializeField] private Slider gravSwitch;
    [SerializeField] private Slider staminaBar;


    [Space,Header("Health UI Variables")]
    [SerializeField] private float shakeStrength;
    [SerializeField] private AnimationCurve screenshakeAnimationCurve;
    private Vector3 startPos;


    [Space, Header("Gravswitch UI Variables")]
    [SerializeField] private float maxGravCharge;
    [SerializeField] private float currentGravCharge;
    [SerializeField] private float arrowGlowTime;

    private playerHealth healthStats;
    private movement movementStats;

    private void Awake()
    {
        healthStats = GameObject.Find("Player").GetComponent<playerHealth>();
        movementStats = GameObject.Find("Player").GetComponent<movement>();
        startPos = HUD.transform.position;

        setupStats();

        healthStats.tookDamage.AddListener(startDecreasingHP);
        healthStats.healedDamage.AddListener(increaseHP);

    }

    private void Update()
    {
        constantStats();
    }


    void setupStats()
    {
        healthBar.maxValue = healthStats.maxHp;
        healthBar.value = healthStats.maxHp;
        staticMeter.maxValue = healthStats.maxStaticEnergy;
        staminaBar.maxValue = 3;
    }

    void startDecreasingHP()
    {
        StartCoroutine(decreaseHP());
    }

    private IEnumerator decreaseHP()
    {
        healthBar.value = healthStats.currentHP;

        StartCoroutine(ShakeHUD());
        while(healthDrain.value < healthBar.value)
        {
            healthDrain.value -= healthStats.immunityTime * Time.deltaTime;
            yield return null;
        }
    }

    private void increaseHP()
    {
        healthBar.value++;
        healthDrain.value = healthBar.value;
    }

    void constantStats()
    {
        if (!movementStats.instantRecharge)
        {
            staminaBar.value = movementStats.staminaCharges + movementStats.currentCharge;
        }
        else staminaBar.value = movementStats.staminaCharges;
        staticMeter.value = healthStats.currentStaticEnergy;
    }

    private IEnumerator ShakeHUD()
    {
        float timer = 0;

        while(timer < healthStats.immunityTime)
        {
            timer += Time.deltaTime;
            float shakeCurve = screenshakeAnimationCurve.Evaluate(timer / healthStats.immunityTime);
            HUD.transform.position = startPos + shakeCurve * shakeStrength * Random.insideUnitSphere;
            yield return null;
        }
        HUD.transform.position = startPos;
    }   
}
