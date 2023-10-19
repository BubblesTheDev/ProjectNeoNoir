using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GravswitchBar : MonoBehaviour
{
    [SerializeField]
    private Slider gravSwitch;

    [SerializeField]
    private float maxCharge;

    [SerializeField]
    private float currentCharge;

    private WaitForSeconds w = new WaitForSeconds(0.1f);

    [SerializeField]
    private float arrowGlowTime;

    private Coroutine gravCD;
    public static GravswitchBar instance { get; private set; }

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
        currentCharge = maxCharge;
        gravSwitch.maxValue = maxCharge;
        gravSwitch.value = maxCharge;
    }

    public void UseGravity()
    {
        if(gravity.gravityReference.canSwitch)
        {
            currentCharge = 0;
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
        yield return new WaitForSeconds(arrowGlowTime);
        gravSwitch.value = 0;
        while (currentCharge < maxCharge)
        {
            //timer += Time.deltaTime;
            currentCharge += 100 / (gravity.gravityReference.gravitySwitchCooldownBase - arrowGlowTime) * Time.deltaTime;
            gravSwitch.value = currentCharge;
            // Debug.Log("HUD value = " + gravSwitch.value + " at " + timer);
            yield return null;
        }
        gravCD = null;
    }

}
