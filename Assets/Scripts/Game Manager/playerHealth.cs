using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    public int currentHP;
    public int maxHp;
    public float maxStaticEnergy;
    public float currentStaticEnergy;

    [SerializeField] private float staticEnergyRate;
    [SerializeField] private float immunityTime;
    private Rigidbody rb;

    public static playerHealth healthRef { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }



    private void Update()
    {
        Debug.Log(currentStaticEnergy);
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine(takeDamage(1));
        }
        if (rb.velocity.magnitude > 0 || rb.velocity.magnitude < 0) currentStaticEnergy += staticEnergyRate * Time.deltaTime;
        if (currentStaticEnergy > maxStaticEnergy && currentHP != maxHp)
        {
            currentHP++;
            currentStaticEnergy = 0;
            HealthBar.instance.StaticHeal(1);
        }
    }

    public IEnumerator takeDamage(int damage)
    {
        currentHP--;
        HealthBar.instance.TakeDamage(damage);
        //Play health dmg sound
        //frame stutter

        yield return new WaitForSeconds(immunityTime);
    }
}
