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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }



    private void Update()
    {
        if (rb.velocity.magnitude > 0 || rb.velocity.magnitude < 0) currentStaticEnergy += staticEnergyRate * Time.deltaTime;
        if (currentStaticEnergy > maxStaticEnergy && currentHP != maxHp)
        {
            currentHP++;
            currentStaticEnergy = 0;
        }
    }

    public IEnumerator takeDamage(int damage)
    {
        currentHP--;
        //Play health dmg sound
        //frame stutter

        yield return new WaitForSeconds(immunityTime);
    }
}
