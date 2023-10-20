using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;
using System.Text.RegularExpressions;

public class PlayerHUD : MonoBehaviour
{
    //[SerializeField]
    //private GameObject text;
    //private TextMeshProUGUI tm;

    [Header("Scene Objects")]
    [SerializeField]
    private GameObject HUD;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private Slider healthDrain;

    [SerializeField]
    private Slider staticMeter;

    [SerializeField]
    private Slider gravSwitch;

    [SerializeField]
    private Slider staminaBar;


    [Space,Header("Health UI Variables")]

    [SerializeField] private int maxHealth;
    [SerializeField] private int maxStaticCharge;
    private Coroutine dmgLerp;
    private Image im_HUD;

    [SerializeField]
    private AnimationCurve healthDrainCurve;

    [SerializeField]
    private float healthDrainWaitTime;

    [SerializeField]
    private float screenshakeDuration;
    [SerializeField]
    private float shakeStrength;
    private Vector3 startPos;

    [SerializeField]
    private AnimationCurve screenshakeAnimationCurve;


    [Space, Header("Gravswitch UI Variables")]

    [SerializeField]
    private float maxGravCharge;
    [SerializeField]
    private float currentGravCharge;

    private WaitForSeconds w = new(0.1f);

    [SerializeField]
    private float arrowGlowTime;
    private Image im_gravSwitch;
    private Color gravSwitchColour;

    private Coroutine gravCD;


    [Space,Header("Stamina UI Variables")]

    [SerializeField]
    private float maxStamina;

    [SerializeField]
    private float currentStamina;

    [SerializeField]
    private float regenRate;
    [Tooltip("The rate at which stamina is recovered per tick (0.1 seconds)")]

    private Coroutine regenStamina;


    public static PlayerHUD instance { get; private set; }

    //private WaitForSeconds waitInterval = new WaitForSeconds(3);

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthDrain.maxValue = maxHealth;
        healthDrain.value = maxHealth;
        im_HUD = HUD.GetComponent<Image>();
        startPos = HUD.transform.position;
        staticMeter.maxValue = maxStaticCharge;
        staticMeter.value = maxStaticCharge;

        currentGravCharge = maxGravCharge;
        gravSwitch.maxValue = maxGravCharge;
        gravSwitch.value = maxGravCharge;
        im_gravSwitch = gravSwitch.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        gravSwitchColour = im_gravSwitch.color;

        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;

        //    tm = text.GetComponent<TextMeshProUGUI>();
        //    tm.text = currentHealth + "/" + maxHealth;
        //StartCoroutine("test");

    }

    // OLD STUFF
    /*void Update()
    {
        currentBarScale = new Vector3((float) currentHealth / (float) maxHealth, transform.localScale.y, transform.localScale.z);
        transform.localScale = Vector3.Lerp(transform.localScale, currentBarScale, 5 * Time.deltaTime);
        tm.text = currentHealth + "/" + maxHealth;

    }*/

    /* testing health bar functionality by making player periodically take damage
     private IEnumerator test()
    {
        while (true) {
            yield return waitInterval;
            onTakeDamage(5);
        }
    }*/

    void Update()
    {
        staticMeter.value = gameObject.GetComponent<playerHealth>().currentStaticEnergy;
    }

    public void TakeDamage(int dmg)
    {
        dmg *= 20;
        float lerpHP = healthBar.value;
        if (dmgLerp != null)
        {
            HUD.transform.position = startPos;
            healthDrain.value = healthBar.value;
            StopCoroutine(dmgLerp);
            StopCoroutine(ShakeHUD());
        }
        healthBar.value -= dmg;
        //    tm.text = currentHealth + "/" + maxHealth;
        StartCoroutine(ShakeHUD());
        dmgLerp = StartCoroutine(LerpHealth(dmg, (int) lerpHP));

    }

    public void StaticHeal(int amount)
    {
        healthBar.value += amount * 20;
    }

    private IEnumerator LerpHealth(int dmg, int startHP)
    {
        yield return new WaitForSeconds(healthDrainWaitTime);
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime * 5;
            healthDrain.value = Mathf.Lerp(startHP, startHP - dmg, healthDrainCurve.Evaluate(t));
            //Debug.Log(healthBar.value);
            yield return null;
        }
        //healthBar.value = currentHP - dmg;
    }

    private IEnumerator ShakeHUD()
    {
        float timer = 0;

        while(timer < screenshakeDuration)
        {
            timer += Time.deltaTime;
            float shakeCurve = screenshakeAnimationCurve.Evaluate(timer / screenshakeDuration);
            HUD.transform.position = startPos + shakeCurve * shakeStrength * Random.insideUnitSphere;
            im_HUD.color = Color.Lerp(Color.black, Color.red, shakeCurve);
            yield return null;
        }
        im_HUD.color = Color.black;
        HUD.transform.position = startPos;
    }

    public void UseGravity()
    {
        if (gravity.gravityReference.canSwitch)
        {
            currentGravCharge = 0;
            if (gravCD != null)
            {
                StopCoroutine(gravCD);
            }

            gravCD = StartCoroutine(GravityCooldown());

        }
        else
        {
            Debug.Log("grav_switch_on_CD.mp3");
        }
    }
    private IEnumerator GravityCooldown()
    {
        //float timer = 0;
        im_gravSwitch.color = Color.white;
        yield return new WaitForSeconds(arrowGlowTime);
        im_gravSwitch.color = gravSwitchColour;
        gravSwitch.value = 0;
        while (currentGravCharge < maxGravCharge)
        {
            //timer += Time.deltaTime;
            currentGravCharge += 100 / (gravity.gravityReference.gravitySwitchCooldownBase - arrowGlowTime) * Time.deltaTime;
            gravSwitch.value = currentGravCharge;
            // Debug.Log("HUD value = " + gravSwitch.value + " at " + timer);
            yield return null;
        }
        gravCD = null;
    }
    public void UseStamina(float value)
    {
        value *= (100 / 3);
        if (currentStamina - value >= 0)
        {
            currentStamina -= value;
            staminaBar.value = currentStamina;

            if (regenStamina != null)
            {
                StopCoroutine(regenStamina);
            }

            regenStamina = StartCoroutine(StaminaRegen());
        }
        else
        {
            Debug.Log("insert_not_enough_stamina.mp3");
        }
    }
    private IEnumerator StaminaRegen()
    {
        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100 * regenRate;
            staminaBar.value = currentStamina;
            yield return w;
        }
        regenStamina = null;
    }
}
