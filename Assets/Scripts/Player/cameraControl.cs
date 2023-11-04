using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
//using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine;
//using Palmmedia.ReportGenerator.Core.Reporting.Builders;

public class cameraControl : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private GameObject Orientation;
    public GameObject CameraObj;
    CameraInputActions controls;
    gravity playerGravReference;
    public RaycastHit lookingDir;
    public LayerMask layersToIgnoreForAimingDir;

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

        layersToIgnoreForAimingDir = ~layersToIgnoreForAimingDir;
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
        
        if(!playerGravReference.gravityIsFlipped )
        {
            Orientation.transform.localRotation = Quaternion.Euler(Orientation.transform.localRotation.x, xRot, 0);
        } else if (playerGravReference.gravityIsFlipped)
        {
            Orientation.transform.localRotation = Quaternion.Euler(Orientation.transform.localRotation.x, -xRot, 180);

        }


        Physics.Raycast(CameraObj.transform.position, CameraObj.transform.forward, out lookingDir, Mathf.Infinity, layersToIgnoreForAimingDir);
    }


    private void CameraRotation_performed(InputAction.CallbackContext obj)
    {
        mouseX = obj.ReadValue<Vector2>().x;
        mouseY = obj.ReadValue<Vector2>().y;

        xRot += mouseX * mouseSensitivity * Time.deltaTime;
        yRot -= mouseY * mouseSensitivity * Time.deltaTime;

        yRot = Mathf.Clamp(yRot, minAngle, maxAngle);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 tempDir = CameraObj.transform.forward * 100f;
        Gizmos.DrawRay(CameraObj.transform.position, tempDir);
    }
}
