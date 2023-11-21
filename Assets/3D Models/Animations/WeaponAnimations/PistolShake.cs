using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolShake : MonoBehaviour
{

    private bool IsCharging = false;
    [SerializeField]
    private float shakeAmt;

    private Vector3 startPos;

    private float timer = 0;

    [SerializeField]
    private float shakeIncrease;

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
        if(Input.GetMouseButtonDown(1)) startPos = transform.localPosition;

        if (Input.GetMouseButton(1))
        {
            if(timer < shakeAmt)
            {
                timer += Time.deltaTime * shakeIncrease;
            }
            IsCharging = true;
        }
        else
        {
            IsCharging = false;
            timer = 0;
        }

        if (Input.GetMouseButtonUp(1)) transform.localPosition = startPos;

        if (IsCharging == true) {

            Vector3 newPos = startPos + (Random.insideUnitSphere * (Time.deltaTime * timer));
            //newPos.y = startPos.y;
            transform.localPosition = new Vector3(newPos.x, newPos.y, transform.localPosition.z);
            
        }
       
    }

}
