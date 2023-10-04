using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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


    [Header("Ground Check Stats")]
    [Space][SerializeField] private float groundCheckDistance;
    [SerializeField] private GameObject groundCheckPosition;
    [SerializeField] private LayerMask groundCheckLayers;

    [Header("External Movement Stats")]
    [SerializeField] private int staminaCharges;
    [SerializeField] private float rechargeRate;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float slidingSpeed;
    [SerializeField] private float buttSlamForce;

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
        movementInput.playerMovment.HorizontalMovement.performed += HorizontalMovement_performed;
    }

    private void HorizontalMovement_performed(InputAction.CallbackContext obj)
    {
        horizontalMovement = movementInput.playerMovment.HorizontalMovement.ReadValue<Vector2>();

        Vector3 moveVector = transform.forward * horizontalMovement.y + transform.right * horizontalMovement.x;
        rb.AddForce(moveVector.normalized * moveSpeed * moveMulti * Time.deltaTime, ForceMode.Acceleration);
    }

    private void FixedUpdate()
    {
        movePlayer();
    }

    private void Update()
    {
        controlDrag();
    }

    void movePlayer()
    {
        //Get movement input from input system into a 2d vector
        horizontalMovement = movementInput.playerMovment.HorizontalMovement.ReadValue<Vector2>();

        //Detect if freelook, if not, then apply movment to the direction the orientation is facing and call body to follow orientation
        Vector3 moveVector = objOrientation.transform.forward * horizontalMovement.y + objOrientation.transform.right * horizontalMovement.x;
        rb.AddForce(moveVector.normalized * moveSpeed * moveMulti * Time.deltaTime, ForceMode.Acceleration);
    }

    void jump()
    {
        
    }

    void dash()
    {

    }

    void slide()
    {
        
    }

    void controlDrag()
    {
        rb.drag = drag;
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
    airborne = 1,
    dashing = 2,
    sliding = 3,
    wallrunning = 4,

    nonAirbornActions = grounded | sliding | wallrunning,
    airborneActions = airborne | dashing
}

//hi mavis