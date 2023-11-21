using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerJuice : MonoBehaviour
{

    public static playerJuice playerJuiceReference
    {
        get
        {
            if (playerJuiceReference == null)
            {
                if (GameObject.FindObjectOfType<playerJuice>() != null)
                    GameObject.FindObjectOfType<playerJuice>();
                else
                {
                    Debug.LogError("There is no player juice script in the scene \n <b>Please Add One To The Scene </b>");
                    Debug.Break();
                }
            }
            return playerJuiceReference;

        }
    }

    [Space, SerializeField] private bool enableHeadbob = true;
    [SerializeField] private Vector2 amplitude = new Vector2(0.03f, 0.015f), frequency = new Vector2(12f, 12f);
    [SerializeField] private float headbobActivateLimit = 3, headbobIntensity = 1;

    [Space, SerializeField] private bool enableGunLag = true;
    [SerializeField] private float sideLagDistance, heightLagDifference, frontLagDifference;
    [SerializeField] private float smoothness;
    [SerializeField] private GameObject objThatFollows;

    [Space, SerializeField] private float screenshakeIntensity;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private GameObject objToShake;

    private Rigidbody rb;
    private cameraControl camControl;
    private void Awake()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        camControl = GameObject.Find("Player").GetComponent<cameraControl>();


#if !UNITY_EDITOR
        getSettings();
#endif
    }
    private void FixedUpdate()
    {
        smoothFollow();
        headbob();
    }

    public void getSettings()
    {
        if (PlayerPrefs.GetString("weaponBounceEnableSetting") == "true") enableGunLag = true;
        else enableGunLag = false;
        if (PlayerPrefs.GetString("headbobEnableSettings") == "true") enableHeadbob = true;
        else enableHeadbob = false;
        headbobIntensity = PlayerPrefs.GetFloat("headbobIntensitySettings");
    }

    void headbob()
    {
        if (!enableHeadbob) return;

        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency.y * headbobIntensity) * amplitude.y * headbobIntensity;
        pos.x += Mathf.Sin(Time.time * frequency.x * headbobIntensity) * amplitude.x * headbobIntensity;

        if (rb.velocity.magnitude > headbobActivateLimit) objThatFollows.transform.localPosition += pos;
        objThatFollows.transform.LookAt(camControl.lookingDir.point);
    }

    void smoothFollow()
    {
        if (!enableGunLag) return;


        Vector3 localRBVelocity = objThatFollows.transform.parent.transform.InverseTransformDirection(rb.velocity);
        Vector3 targetPos = new Vector3(sideLagDistance + (-Mathf.Clamp(localRBVelocity.x, -15, 15) / 100),
            heightLagDifference * (-Mathf.Clamp(localRBVelocity.y, -15, 15) / 100),
            frontLagDifference * (-Mathf.Clamp(localRBVelocity.z, -15, 15) / 100));

        objThatFollows.transform.localPosition = Vector3.Lerp(objThatFollows.transform.localPosition, targetPos, smoothness);
    }

    public IEnumerator screenshake(float duration)
    {
        Vector3 startPos = objToShake.transform.position;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            objToShake.transform.position = startPos + (Random.insideUnitSphere * (intensityCurve.Evaluate(currentTime/duration) * screenshakeIntensity));
            yield return null;
        }
        objToShake.transform.position = startPos;
    }
}
