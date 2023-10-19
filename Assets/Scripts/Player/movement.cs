using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class movement : MonoBehaviour
{
    [Header("Assignables")]
    public movementStates movmentState;
    [SerializeField] private Rigidbody rb;
    public GameObject objOrientation;
    [SerializeField] private bool freeLook;
    MovementInputActions movementInput;

    [Space, Header("Base Movement Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveMulti;
    [SerializeField] private float drag = 6f;
    [SerializeField] private float airDrag = .95f;
    [SerializeField] private movementStates currentMovementState;
    [SerializeField] private bool isGrounded;


    [Header("Ground Check Stats")]
    [Space][SerializeField] private float groundCheckDistance;
    [SerializeField] private GameObject groundCheckPosition;
    [SerializeField] private LayerMask groundCheckLayers;

    [Header("External Movement Stats")]
    [SerializeField, Space] private int staminaCharges;
    [SerializeField] private bool instantRecharge;
    [SerializeField] private float rechargeRate;

    //Jump variables
    [SerializeField, Space] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    public bool canJump = true;

    //Dash variables
    [SerializeField, Space] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashDrag;
    [SerializeField] private float maxDashTime;
    public bool canDash = true;

    //Sliding Variables
    [SerializeField, Space] private float slideSpeed;
    [SerializeField] private float slideCooldown;
    private bool canSlide = true;

    //Buttslam Variables
    [SerializeField, Space] private float buttSlamForce;
    [SerializeField] private float buttSlamCooldown;
    [SerializeField] private float slamReboundTime;
    [SerializeField] private float slamDrag;
    [SerializeField] private float timeToRaiseSlam;
    [SerializeField] private float raiseDistance;
    private bool slamRebound;
    private bool canSlam = true;

    //Wallrun Variables
    [SerializeField, Space] private float distanceToDetectWall;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private float maxWallrunTime;
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpAngle;
    [SerializeField] private float wallRunDistance;
    [SerializeField] private float cameraAngleTilt;

    private Vector2 horizontalMovement;

    private void OnEnable()
    {
        movementInput.Enable();
    }
    private void OnDisable()
    {
        movementInput.Disable();
    }

    private void Awake()
    {
        movementInput = new MovementInputActions();
        rb = GetComponent<Rigidbody>();
        movementInput.playerMovment.HorizontalMovement.performed += HorizontalMovement_performed => movePlayer();
        movementInput.playerMovment.Dash.performed += DashMovement_performed => StartCoroutine(dash());
        movementInput.playerMovment.Jump.performed += JumpMovement_performed => StartCoroutine(jump());
        movementInput.playerMovment.Slam.performed += Slam_performed => StartCoroutine(buttSlam());
        movementInput.playerMovment.Slide.started += Slide_performed => StartCoroutine(startSlide());
        movementInput.playerMovment.Slide.canceled += Slide_canceled => StartCoroutine(endslide());
    }


    private void FixedUpdate()
    {
        if(!freeLook) movePlayer();

    }

    private void Update()
    {
        controlDrag();
        if (!freeLook)
        {
            gameObject.transform.localRotation = objOrientation.transform.localRotation;
        }


    }

    

    void movePlayer()
    {
        //Get movement input from input system into a 2d vector
        horizontalMovement = movementInput.playerMovment.HorizontalMovement.ReadValue<Vector2>();

        //Detect if freelook, if not, then apply movment to the direction the orientation is facing and call body to follow orientation
        Vector3 moveVector = gameObject.transform.forward * horizontalMovement.y + gameObject.transform.right * horizontalMovement.x;
        rb.AddForce(moveVector.normalized * moveSpeed * moveMulti * Time.deltaTime, ForceMode.Force);
    }

    IEnumerator jump()
    {
        if (!canJump) yield break;
        if (currentMovementState == movementStates.sliding) StartCoroutine(endslide());
        if (getGroundCheck() == false)
        {
            if (staminaCharges > 0)
            {
                //print("jumping 3");
                staminaCharges--;
            }
            else yield break;
        }

        canJump = false;
        currentMovementState = movementStates.jumping;


        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(-gravity.gravityReference.currentGravityDir * jumpForce, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpCooldown);

        canJump = true;
    }

    IEnumerator dash()
    {
        if (!canDash) yield break;
        if (currentMovementState == movementStates.sliding) StartCoroutine(endslide());
        if (!getGroundCheck())
        {
            if (staminaCharges > 0)
                staminaCharges--;
            else yield break;
        }

        StaminaBar.instance.UseStamina(33.333f);

        canDash = false;
        currentMovementState = movementStates.dashing;

        Vector3 originDashPos = transform.position;
        float originalDrag = rb.drag;
        float timer = 0;
        gravity.gravityReference.useGravity = false;
        rb.drag = dashDrag;



        rb.velocity = new Vector3(0, 0, 0);

        if (movementInput.playerMovment.HorizontalMovement.ReadValue<Vector2>().magnitude == 0)
        {
            rb.AddForce(gameObject.transform.forward * dashForce, ForceMode.Impulse);
        }
        else
        {
            Vector3 moveVector = objOrientation.transform.forward * horizontalMovement.y + objOrientation.transform.right * horizontalMovement.x;
            rb.AddForce(moveVector * dashForce, ForceMode.Impulse);
        }

        while (Vector3.Distance(transform.position, originDashPos) < dashDistance || timer < maxDashTime)
        {
            timer += Time.deltaTime;
            //print(timer);
            yield return null;
        }

        rb.drag = originalDrag;
        gravity.gravityReference.useGravity = true;

        if (getGroundCheck()) currentMovementState = movementStates.grounded;
        else currentMovementState = movementStates.jumping;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    IEnumerator startSlide()
    {
        if (!getGroundCheck() || !canSlide || currentMovementState == movementStates.dashing || currentMovementState == movementStates.sliding || currentMovementState == movementStates.wallrunning) yield break;

        Vector3 moveVector = gameObject.transform.forward * horizontalMovement.y + gameObject.transform.right * horizontalMovement.x;
        freeLook = true;
        currentMovementState = movementStates.sliding;
        canSlide = false;

        while (movementInput.playerMovment.Slide.IsPressed())
        {
            print("im sliding");
            rb.AddForce(moveVector * slideSpeed * moveMulti * Time.deltaTime, ForceMode.Force);
            yield return null;
        }
        
        //play particle effects,
        //play slide sound,
    }

    IEnumerator endslide()
    {
        freeLook = false;
        currentMovementState = movementStates.grounded;

        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }


    IEnumerator buttSlam()
    {
        //Detects if the player can slam
        if(currentMovementState == movementStates.grounded 
            || currentMovementState == movementStates.sliding 
            || currentMovementState == movementStates.slamming 
            || getGroundCheck() && canSlam)
        {
            yield break;
        }

        //Sets the player to start slamming, turns off gravity and stops any vertical movement while
        //causing the player to slow a lot with a high drag 
        currentMovementState = movementStates.slamming;
        gravity.gravityReference.useGravity = false;
        rb.drag = slamDrag;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        freeLook = true;
        canSlam = false;

        //The player will then raise into the air slightly before slamming
        float tempTime = 0;
        while(tempTime < timeToRaiseSlam)
        {
            tempTime += Time.deltaTime;
            rb.Move(transform.position + -gravity.gravityReference.currentGravityDir * raiseDistance, transform.rotation);
            yield return null;
        }

        //Once the player has raised slightly they will slam down quickly
        rb.AddForce(gravity.gravityReference.currentGravityDir * buttSlamForce, ForceMode.Impulse);

        //Waits till certain seconds, or till the player hits the ground
        tempTime = 0;
        while(tempTime < 0.5f || getGroundCheck())
        {
            tempTime += Time.deltaTime;
            yield return null;
        }


        freeLook = false;
        gravity.gravityReference.useGravity = true;
        rb.drag = drag;
        currentMovementState = movementStates.grounded;

        //if slam hits the ground,
        //deal dmg to enemies and knock them back,
        //screenshake
        //Start the rebound timer

        if (getGroundCheck())
        {
            slamRebound = true;
            yield return new WaitForSeconds(slamReboundTime);
        }

        

        float tempSlamCooldown = buttSlamCooldown;
        if (getGroundCheck()) tempSlamCooldown = buttSlamCooldown - slamReboundTime;
        yield return new WaitForSeconds(tempSlamCooldown);
        canSlam = true;
    }

    void controlDrag()
    {
        switch (currentMovementState)
        {
            case movementStates.grounded:
                rb.drag = drag;
                break;
            case movementStates.jumping:
                rb.drag = airDrag;
                break;
            case movementStates.sliding:
                rb.drag = drag;
                break;
            case movementStates.dashing:
                rb.drag = dashDrag;
                break;
        }
    }

    public bool getGroundCheck()
    {

        if (Physics.OverlapSphere(groundCheckPosition.transform.position, groundCheckDistance, groundCheckLayers).Length > 0)
        {
            return true;
        }
        else return false;
    }



}

[Flags]
public enum movementStates
{
    grounded = 0,
    jumping = 1,
    dashing = 2,
    sliding = 3,
    wallrunning = 4,
    slamming = 5
}