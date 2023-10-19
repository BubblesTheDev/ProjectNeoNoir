using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    private Slider staminaBar;

    [SerializeField]
    private float maxStamina;

    [SerializeField]
    private float currentStamina;

    [SerializeField]
    private float regenRate;
    [Tooltip("The rate at which stamina is recovered per tick (0.1 seconds)")]

    public static StaminaBar instance { get; private set; }

    private WaitForSeconds w = new(0.1f);

    private Coroutine regen;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }

    public void UseStamina(float value)
    {
        if(currentStamina - value >= 0)
        {
            currentStamina -= value;
            staminaBar.value = currentStamina;

            if(regen != null)
            {
                StopCoroutine(regen);
            }

            regen = StartCoroutine(StaminaRegen());
        }
        else
        {
            Debug.Log("insert_not_enough_stamina.mp3");
        }
    }

    private IEnumerator StaminaRegen()
    {
        while(currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100 * regenRate;
            staminaBar.value = currentStamina;
            yield return w;
        }
        regen = null;
    }
}
