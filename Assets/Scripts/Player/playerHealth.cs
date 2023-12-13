using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerHealth : MonoBehaviour
{

    public int currentHP;
    public int maxHp;
    public float maxStaticEnergy;
    public float currentStaticEnergy;
    [SerializeField] private float movementMulti;

    [SerializeField] private float staticEnergyRate;
    public float immunityTime;
    private Rigidbody rb;

    [HideInInspector] public UnityEvent tookDamage;
    [HideInInspector] public UnityEvent healedDamage;

    private void Awake()
    {
        currentHP = maxHp;
        rb = GetComponent<Rigidbody>();
    }



    private void Update()
    {
        
        
        if (currentStaticEnergy >= maxStaticEnergy && currentHP != maxHp)
        {
            currentHP++;
            currentStaticEnergy = 0;
            healedDamage.Invoke();
        }

        if (rb.velocity.magnitude > 0 || rb.velocity.magnitude < 0)
        {
            if(currentHP < 5)
            {
                float movementStaticIncrease = Mathf.Sqrt(Mathf.Pow(rb.velocity.magnitude, 2));
                currentStaticEnergy += staticEnergyRate * (1 + (movementStaticIncrease / movementMulti)) * Time.deltaTime;
                if (currentStaticEnergy >= maxStaticEnergy) currentStaticEnergy = maxStaticEnergy;
            }
        }
        if (Input.GetKeyDown(KeyCode.F1)) StartCoroutine(takeDamage(1));
    }

    public IEnumerator takeDamage(int damage)
    {
        currentHP--;
        //Play health dmg sound
        //frame stutter
        tookDamage.Invoke();

        yield return new WaitForSeconds(immunityTime);
    }
}
