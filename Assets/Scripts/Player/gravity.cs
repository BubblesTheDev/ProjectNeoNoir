using Palmmedia.ReportGenerator.Core.Reporting.Builders;
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

        gravityReference = GameObject.Find("Game Manager").GetComponent<gravity>();
        currentGravityDir = -Vector3.up;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R) && canSwitch && useGravity)
        {
            GravswitchBar.instance.UseGravity();
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
            player.transform.localRotation =
                Quaternion.Euler(player.transform.localRotation.eulerAngles.x,
                    player.transform.localRotation.eulerAngles.y,
                    player.transform.localRotation.eulerAngles.z + (180 * currentGravityDir.y));
        }
        else
        {
            float currentTime = 0;
            while (currentTime <= timeToFlipGravity)
            {
                player.transform.localRotation = Quaternion.Euler(player.transform.localRotation.eulerAngles.x,
                    player.transform.localRotation.eulerAngles.y,
                    player.transform.localRotation.eulerAngles.z + (180 / timeToFlipGravity * Time.deltaTime));
                currentTime += Time.deltaTime;
                yield return null;
            }
            if (currentGravityDir.y == -1) player.transform.localRotation = Quaternion.Euler(player.transform.localRotation.eulerAngles.x, player.transform.localRotation.eulerAngles.y, 0);
            else player.transform.localRotation = Quaternion.Euler(player.transform.localRotation.eulerAngles.x, player.transform.localRotation.eulerAngles.y, 180);
        }

        gravityIsFlipped = !gravityIsFlipped;

        yield return new WaitForSeconds(gravitySwitchCooldownBase);
        canSwitch = true;
    }
}
