using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Rigidbody rb;
    public GameObject objOrientation { private set; get; }
    [SerializeField] private bool freeLook;
    MovementInputActions movementInput;

    [Space, Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveMulti;
    [SerializeField] private float drag = 6f;
    public movementStates movmentState { private set; get; }

    [Space][SerializeField] private float groundCheckDistance;
    [SerializeField] private GameObject groundCheckPosition;
    [SerializeField] private LayerMask groundCheckLayers;
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
        //Reset artifical gravity of player,
        //Apply jump force dependant on how long player holds button for
        //set state of player to be airborne
    }

    void controlDrag()
    {
        rb.drag = drag;
    }

    bool getGroundCheck()
    {
        RaycastHit groundCheckHit;
        Physics.SphereCast(groundCheckPosition.transform.position, groundCheckDistance, -groundCheckPosition.transform.up, out groundCheckHit, Mathf.Infinity, groundCheckLayers);
        if (groundCheckHit.point != null)
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

