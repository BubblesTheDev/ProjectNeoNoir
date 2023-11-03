//using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class gravity : MonoBehaviour
{
    public static gravity gravityReference;

    [Header("Assignables")]
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody playerRb;
    private movement playerMovement;
    private GameObject playerCam;
    public bool gravityIsFlipped { private set; get; }

    [Space, Header("Statistics")]
    public float gravitySwitchCooldownBase;
    public bool useGravity = true;
    [SerializeField] public bool canSwitch { get; private set; } = true;
    [SerializeField] private float gravityForce;
    [SerializeField] private float gravityMultiplier;
    public Vector3 currentGravityDir { private set; get; }

    [Space, Header("Debug/Design")]
    [SerializeField] private float timeToFlipGravity;
    [SerializeField] private bool instantFlip;


    private void Awake()
    {
        playerMovement = player.GetComponent<movement>();
        playerCam = GameObject.Find("Orientation");

        gravityReference = GameObject.Find("Game Manager").GetComponent<gravity>();
        currentGravityDir = -Vector3.up;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R) && canSwitch && useGravity)
        {
            AudioManager.instance.PlaySFX(FMODEvents.instance.gravitySwitch, this.transform.position);
            StartCoroutine(flipGravity());
        }
    }

    private void FixedUpdate()
    {
        if(useGravity) 
        {
            if (player != null && playerRb != null)
            {
                if (playerMovement.getGroundCheck())
                    playerRb.velocity = new Vector3(playerRb.velocity.x, currentGravityDir.y, playerRb.velocity.z);
                else playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y + ((currentGravityDir.y * gravityForce) * Mathf.Exp(2)) * Time.deltaTime, playerRb.velocity.z);
            }
        }
    }

    public IEnumerator flipGravity()
    {
        canSwitch = false;
        currentGravityDir *= -1;
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);

        if (instantFlip)
        {
            playerCam.transform.localRotation =
                Quaternion.Euler(playerCam.transform.localRotation.eulerAngles.x,
                    playerCam.transform.localRotation.eulerAngles.y,
                    playerCam.transform.localRotation.eulerAngles.z + (180 * currentGravityDir.y));
        }
        else
        {
            float currentTime = 0;
            while (currentTime <= timeToFlipGravity)
            {
                playerCam.transform.localRotation = Quaternion.Euler(playerCam.transform.localRotation.eulerAngles.x,
                    playerCam.transform.localRotation.eulerAngles.y,
                    playerCam.transform.localRotation.eulerAngles.z + (180 / timeToFlipGravity * Time.deltaTime));
                currentTime += Time.deltaTime;
                yield return null;
            }

            if(gravityIsFlipped) playerCam.transform.localRotation = Quaternion.Euler(playerCam.transform.localRotation.x, playerCam.transform.localRotation.y, 180);
            else playerCam.transform.localRotation = Quaternion.Euler(playerCam.transform.localRotation.x, playerCam.transform.localRotation.y, 0);

        }

        gravityIsFlipped = !gravityIsFlipped;

        yield return new WaitForSeconds(gravitySwitchCooldownBase);
        canSwitch = true;
    }
}
