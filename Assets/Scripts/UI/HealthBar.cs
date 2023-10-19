using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;
using System.Text.RegularExpressions;

public class HealthBar : MonoBehaviour
{
    //[SerializeField]
    //private GameObject text;
    //private TextMeshProUGUI tm;

    [Header("Scene Objects")]
    [SerializeField]
    private GameObject HUD;
    private Image im;

    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Slider healthDrain;

    [SerializeField]
    private Slider staticMeter;

    [Space,Header("Health UI Variables")]

    [SerializeField] private int maxHealth;
    [SerializeField] private int maxCharge;
    private Coroutine dmgLerp;

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

    public static HealthBar instance { get; private set; }

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
        im = HUD.GetComponent<Image>();
        startPos = HUD.transform.position;
        staticMeter.maxValue = maxCharge;
        staticMeter.value = maxCharge;
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
        //staticMeter.value = playerHealth.healthRef.currentStaticEnergy;
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
            im.color = Color.Lerp(Color.black, Color.red, shakeCurve);
            yield return null;
        }
        im.color = Color.black;
        HUD.transform.position = startPos;
    }

}
