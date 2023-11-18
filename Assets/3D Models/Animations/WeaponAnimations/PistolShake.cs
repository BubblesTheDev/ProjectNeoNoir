using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolShake : MonoBehaviour
{

    private bool IsCharging = true;
    [SerializeField]
    private float shakeAmt;

    public void PistolShakeWork() {

        StartCoroutine("ShakeNow");

    }
    IEnumerator ShakeNow() {
    Vector3 originalPos = transform.position;
        Debug.Log(originalPos);

        if (IsCharging == false) {

            IsCharging = true;
        }

        yield return new WaitForSeconds(.05f);

        IsCharging = false;
        transform.position = originalPos;
    }
    void Awake()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (IsCharging == true) {

            Vector3 newPos = Random.insideUnitSphere * (Time.deltaTime * shakeAmt);
            newPos.y = transform.position.y;
            transform.position = newPos;
            
        }
       
    }

}
