using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;

public class cameraControl : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private GameObject Orientation;
    [SerializeField] private GameObject CameraObj;
    CameraInputActions controls;
    gravity playerGravReference;

    [Space, Header("Stats")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float maxAngle;
    [SerializeField] private float minAngle;

    public float mouseX;
    public float mouseY;
    float xRot;
    float yRot;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controls = new CameraInputActions();
        controls.CameraControls.CameraRotation.performed += CameraRotation_performed;

        playerGravReference = GameObject.Find("Game Manager").GetComponent<gravity>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        CameraObj.transform.localRotation = Quaternion.Euler(yRot, CameraObj.transform.localRotation.y, CameraObj.transform.localRotation.z);
        if(playerGravReference.gravityIsFlipped) transform.localRotation = Quaternion.Euler(0, -xRot, 180);
        else transform.localRotation = Quaternion.Euler(0, xRot, 0);
    }


    private void CameraRotation_performed(InputAction.CallbackContext obj)
    {
        mouseX = obj.ReadValue<Vector2>().x;
        mouseY = obj.ReadValue<Vector2>().y;

        xRot += mouseX * mouseSensitivity * Time.deltaTime;
        yRot -= mouseY * mouseSensitivity * Time.deltaTime;

        yRot = Mathf.Clamp(yRot, minAngle, maxAngle);
    }
}
