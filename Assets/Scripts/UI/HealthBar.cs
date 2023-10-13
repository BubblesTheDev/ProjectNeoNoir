using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private GameObject text;
    private TextMeshProUGUI tm;

    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    private Vector3 currentBarScale;

    //private WaitForSeconds waitInterval = new WaitForSeconds(3);

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        currentHealth = 100;
        tm = text.GetComponent<TextMeshProUGUI>();
        //StartCoroutine("test");

    }

    // Update is called once per frame
    void Update()
    {
        currentBarScale = new Vector3((float) currentHealth / (float) maxHealth, transform.localScale.y, transform.localScale.z);
        transform.localScale = Vector3.Lerp(transform.localScale, currentBarScale, 5 * Time.deltaTime);
        tm.text = currentHealth + "/" + maxHealth;

    }

    /* testing health bar functionality by making player periodically take damage
     private IEnumerator test()
    {
        while (true) {
            yield return waitInterval;
            onTakeDamage(5);
        }
    }*/

    public void onTakeDamage(int dmg)
    {
        currentHealth -= dmg;
    }
}
