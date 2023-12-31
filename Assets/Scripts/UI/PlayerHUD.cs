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
    private Image im_HUD;


    [Space, Header("Gravswitch UI Variables")]
    [SerializeField] private float arrowGlowTime;
    [SerializeField] private Color arrowColor = new Color(0, 228, 255);
    [SerializeField] private Image gravSlider;

    private playerHealth healthStats;
    private playerMovement movementStats;


    [Space, Header("Weapon HUD")]
    [SerializeField] private Image pistolIcon;
    [SerializeField] private Image shotgunIcon;
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject shotgun;

    private void Awake()
    {
        healthStats = GameObject.Find("Player").GetComponent<playerHealth>();
        movementStats = GameObject.Find("Player").GetComponent<playerMovement>();

        setupStats();

        healthStats.tookDamage.AddListener(startDecreasingHP);
        healthStats.healedDamage.AddListener(increaseHP);
        movementStats.onAction_Flip_Start.AddListener(flipGrav);
        movementStats.onAction_Flip_End.AddListener(flipGrav);
        im_HUD = HUD.GetComponent<Image>();
        


    }

    private void Start()
    {
        startPos = HUD.transform.position;

    }

    private void Update()
    {
        constantStats();
        if (pistol.GetComponent<weaponBase>().weaponIsEquipped) pistolIcon.enabled = true;
        else pistolIcon.enabled = false;
        if (shotgun.GetComponent<weaponBase>().weaponIsEquipped) shotgunIcon.enabled = true;
        else shotgunIcon.enabled = false;
    }


    void setupStats()
    {
        healthBar.maxValue = healthStats.maxHp;
        healthBar.value = healthStats.maxHp;
        healthDrain.maxValue = healthStats.maxHp;
        healthDrain.value = healthStats.maxHp;

        staticMeter.maxValue = healthStats.maxStaticEnergy;
        staminaBar.maxValue = movementStats.numberOf_MaximumDashCharges;
        gravSwitch.maxValue = movementStats.timeInSeconds_GravityFlipDuration;
        gravSwitch.value = movementStats.timeInSeconds_GravityFlipDuration;
    }

    void startDecreasingHP()
    {
        StartCoroutine(decreaseHP());
    }

    private IEnumerator decreaseHP()
    {
        healthBar.value = healthStats.currentHP;

        StartCoroutine(ShakeHUD());
        while(healthDrain.value > healthBar.value)
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
        staminaBar.value = movementStats.current_NumberOfDashCharges;
        staticMeter.value = healthStats.currentStaticEnergy;
        gravSwitch.value = movementStats.timeInSeconds_CurrentGravityFlipDuration;
    }

    private void flipGrav()
    {
        StartCoroutine(switchGravity());
    }

    IEnumerator switchGravity()
    {
        gravSlider.color = Color.white;
        yield return new WaitForSeconds(movementStats.timeInSeconds_ToFlip + arrowGlowTime);
        if (movementStats.overchargedGravityFlip) 
        { 
            gravSlider.color = Color.red;
            yield return new WaitForSeconds(movementStats.timeInSeconds_ToFullyRechargeGravity);
            gravSlider.color = arrowColor;
        }
        else
        {
            gravSlider.color = arrowColor;
        }
    }

    private IEnumerator ShakeHUD()
    {
        float timer = 0;

        while(timer < healthStats.immunityTime)
        {
            timer += Time.deltaTime;
            float shakeCurve = screenshakeAnimationCurve.Evaluate(timer / healthStats.immunityTime);
            HUD.transform.position = startPos + shakeCurve * shakeStrength * Random.insideUnitSphere;
            im_HUD.color = Color.Lerp(Color.black, Color.red, shakeCurve * (shakeStrength / 40));
            yield return null;
        }
        HUD.transform.position = startPos;
        im_HUD.color = Color.black;
    }   
}
