using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    public int currentHP { private set; get; }
    public int maxHp { private set; get; }
    public float maxStaticEnergy { private set; get; }
    public float currentStaticEnergy { private set; get; }

    [SerializeField] private float staticEnergyRate;
    [SerializeField] private float immunityTime;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine(takeDamage(1));
        }
        if (rb.velocity.magnitude > 0 || rb.velocity.magnitude < 0) currentStaticEnergy += staticEnergyRate * Time.deltaTime;
        if (currentStaticEnergy > maxStaticEnergy && currentHP != maxHp) currentHP++;
    }

    public IEnumerator takeDamage(int damage)
    {
        currentHP--;
        HealthBar.instance.TakeDamage(damage);
        //Play health dmg sound
        //frame stutter
        //

        yield return new WaitForSeconds(immunityTime);
    }
}
