using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Rigidbody rb;
    MovementInputActions movementInput;

    [Space, Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveMulti;
    [SerializeField] private float drag = 6f;
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
        horizontalMovement = movementInput.playerMovment.HorizontalMovement.ReadValue<Vector2>();

        Vector3 moveVector = transform.forward * horizontalMovement.y + transform.right * horizontalMovement.x;
        rb.AddForce(moveVector.normalized * moveSpeed * moveMulti * Time.deltaTime, ForceMode.Acceleration);
    }

    void controlDrag()
    {
        rb.drag = drag;
    }



   
}

