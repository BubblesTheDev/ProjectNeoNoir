using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class playerMovement : MonoBehaviour
{
    public playerMovementAction current_playerMovementAction;
    public bool grounded;
    public bool onAcceptableSlope;
    public bool gravityAffected = true;
    public bool canAffectMovement = true;
    public bool canAffectRotation = true;
    [Space]

    #region Action Cooldowns
    [SerializeField] private float action_jumpCooldownTime;
    [SerializeField] private bool action_CanJump;
    [SerializeField] private float action_dashCooldownTime;
    [SerializeField] private bool action_CanDash;
    [SerializeField] private float action_slideCooldownTime;
    [SerializeField] private bool action_CanSlide;
    [SerializeField] private float action_slamCooldownTime;
    [SerializeField] private bool action_CanSlam;
    [SerializeField] private float action_flipCooldownTime;
    [SerializeField] private bool action_CanFlip;
    [Space]
    #endregion

    #region Ground Check Calculations
    [SerializeField] private Vector3 groundCheck_PositionalOffset;
    [SerializeField] private RaycastHit groundCheck_HitInformation;
    [SerializeField] private float groundCheck_Distance;
    [SerializeField] private LayerMask groundCheck_LayersToHit;
    [Space]
    #endregion

    #region horizontal Movement
    [SerializeField] private Vector3 horizontal_playerVelocity;
    [SerializeField] private float acceleration;
    [SerializeField] private float terminalVelocity_Ground, terminalVelocity_Air;
    [SerializeField] private float speedStopThreshold;
    [SerializeField] private float maxSlopeAngle;
    [Space]
    #endregion

    #region Vertical Movement
    [SerializeField] private Vector3 vertical_playerVelocity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityAcceleration;
    [SerializeField] private float terminalVelocity_Gravity;
    [SerializeField] private float terminalVelocity_Jump;
    [Space]
    #endregion

    #region Action Dash
    [SerializeField] private float dashDistance;
    [Space]
    #endregion

    #region Action Slide
    [SerializeField] private float acceleration_Slide;
    [SerializeField] private float acceleration_EndSlideJump;
    [SerializeField] private float terminalVelocity_Slide;
    [SerializeField] private float colliderHeight_normal, colliderHeight_Slide;
    [Space]
    #endregion

    #region Action Slam
    [SerializeField] private float raiseDistance_Slam;
    [SerializeField] private float timeInSeconds_ToRaise;
    [SerializeField] private float acceleration_Slam_Downward;
    [SerializeField] private float terminalVelocity_Slam_Downward;
    [Space]
    #endregion

    #region Action Flip
    [SerializeField] private playerRotationState current_PlayerRotationState;
    [SerializeField] private float timeInSeconds_ToFlip;
    [SerializeField] private Vector3 directionalVector_NonFlipped = new Vector3(0, -1, 0);
    [SerializeField] private Vector3 directionalVector_Flipped = new Vector3(0, 1, 0);
    [Space]
    #endregion

    #region Assignables
    private Rigidbody rb;
    private GameObject directionalOrientation;
    private MovementInputActions current_PlayerInputActions;
    private CapsuleCollider playerCollider;
    #endregion

    #region control variables
    private Vector3 current_playerDirectionalVector;
    private float currentDragForce;
    #endregion

    #region Action Events
    [HideInInspector] public UnityEvent onAction_Jump_Start;
    [HideInInspector] public UnityEvent onAction_Dash_Start;
    [HideInInspector] public UnityEvent onAction_Slide_Start;
    [HideInInspector] public UnityEvent onAction_Slam_Start;
    [HideInInspector] public UnityEvent onAction_Flip_Start;
    [HideInInspector] public UnityEvent onAction_Slide_End;
    [HideInInspector] public UnityEvent onAction_Slam_End;
    [HideInInspector] public UnityEvent onAction_Flip_End;
    #endregion

    #region Debug Variables
    private Vector3 positionLastFrame;
    #endregion

    private void OnEnable()
    {
        current_PlayerInputActions.Enable();
    }

    private void OnDisable()
    {
        current_PlayerInputActions.Disable();
    }

    private void Awake()
    {
        current_PlayerInputActions = new MovementInputActions();
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        colliderHeight_normal = playerCollider.height;
        groundCheck_HitInformation = new RaycastHit();
        if (GameObject.Find("Orientation").gameObject) directionalOrientation = GameObject.Find("Orientation").gameObject;
        else Debug.LogWarning("There is no object in the scene named 'Orientation'. \nThe player movement script will not function properly without one. Please create one and try again.");
    }

    private void Update()
    {
        getPlayerInput();
        groundDetection();
    }

    private void FixedUpdate()
    {
        if (gravityAffected) applyGravity();
        if (canAffectMovement)
        {
            applyHorizontalAcceleration();
        }

        rb.velocity = horizontal_playerVelocity + vertical_playerVelocity;
        applyDrag();
        positionLastFrame = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (grounded && onAcceptableSlope) Gizmos.color = Color.green;
        else if(grounded && !onAcceptableSlope) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + groundCheck_PositionalOffset, -transform.up * groundCheck_Distance);
    }

    private void OnGUI()
    {
        GUIStyle tempstyle = new GUIStyle();
        tempstyle.fontSize = 30;
        tempstyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10+100, 100*4, 20 * 4), "Speed: " + Mathf.Round((Vector3.Distance(transform.position, positionLastFrame) / Time.deltaTime)).ToString() + "Ups", tempstyle);
        GUI.Label(new Rect(10, 40+100, 100 * 4, 20 * 4), "Velocity Magnitude: " + Mathf.Abs(Vector3.Scale(horizontal_playerVelocity, current_playerDirectionalVector.normalized).magnitude).ToString(), tempstyle);

    }

    private void groundDetection()
    {
        if(Physics.Raycast(transform.position + groundCheck_PositionalOffset, -transform.up, out groundCheck_HitInformation, groundCheck_Distance, groundCheck_LayersToHit))
        {
            float tempAngle = Vector3.Angle(transform.up, groundCheck_HitInformation.normal);
            if (tempAngle < maxSlopeAngle)
            {
                grounded = true;
                onAcceptableSlope = true;
                return;
            }
            else
            {
                grounded = true;
                onAcceptableSlope = false;
                return;
            }
        } else
        {
            grounded = false;
            onAcceptableSlope = false;
        }
    }

    private void getPlayerInput()
    {
        current_playerDirectionalVector = directionalOrientation.transform.forward *
            current_PlayerInputActions.playerMovment.HorizontalMovement.ReadValue<Vector2>().y +
            directionalOrientation.transform.right *
            current_PlayerInputActions.playerMovment.HorizontalMovement.ReadValue<Vector2>().x;

        if (current_PlayerInputActions.playerMovment.Dash.WasPressedThisFrame()) StartCoroutine(action_Dash());
        if (current_PlayerInputActions.playerMovment.Slam.WasPressedThisFrame()) StartCoroutine(action_Slam());
        if (current_PlayerInputActions.playerMovment.Slide.WasPressedThisFrame()) StartCoroutine(action_Slide());
        if (current_PlayerInputActions.playerMovment.Jump.IsPressed()) StartCoroutine(action_Jump());
    }

    private void applyDrag()
    {
        #region horizontal drag
        float idealHDragForce = 0;
        currentDragForce = 0;
        if (grounded)
        {
            switch (current_playerMovementAction)
            {
                case playerMovementAction.moving:
                    idealHDragForce = acceleration / terminalVelocity_Ground;
                    break;
                case playerMovementAction.sliding:
                    idealHDragForce = acceleration_Slide / terminalVelocity_Slide;
                    break;
            }
        }
        else idealHDragForce = acceleration / terminalVelocity_Air;

        horizontal_playerVelocity *= 1 - Time.deltaTime * idealHDragForce;
        if (horizontal_playerVelocity.magnitude < speedStopThreshold) horizontal_playerVelocity *= 0;
        #endregion

        float idealVDragForce = 0;
        if (!grounded)
        {
            switch (current_playerMovementAction)
            {
                case playerMovementAction.moving:
                    idealVDragForce = gravityAcceleration / terminalVelocity_Gravity;
                    break;
                case playerMovementAction.jumping:
                    idealVDragForce = gravityAcceleration / terminalVelocity_Jump;
                    break;
                case playerMovementAction.slamming:
                    idealVDragForce = gravityAcceleration / terminalVelocity_Slam_Downward;
                    break;
            }
        }
        vertical_playerVelocity *= 1 - Time.deltaTime * idealVDragForce;
    }

    private void applyGravity()
    {
        if (!grounded)
        {
            switch (current_PlayerRotationState)
            {
                case playerRotationState.nonFlipped:
                    vertical_playerVelocity += (directionalVector_NonFlipped * gravityAcceleration) * Mathf.Exp(2) * Time.deltaTime;
                    break;
                case playerRotationState.flipped:
                    vertical_playerVelocity += (directionalVector_Flipped * gravityAcceleration) * Mathf.Exp(2) * Time.deltaTime;
                    break;
            }
        }
        else if (grounded && current_playerMovementAction != playerMovementAction.jumping) vertical_playerVelocity *= 0;
    }

    private void applyHorizontalAcceleration()
    {
        if (current_PlayerInputActions.playerMovment.HorizontalMovement.ReadValue<Vector2>().magnitude == 0) return;
        horizontal_playerVelocity += current_playerDirectionalVector.normalized * acceleration * Time.deltaTime;
    }

    private IEnumerator action_Jump()
    {
        if (!grounded || !action_CanJump || current_playerMovementAction != playerMovementAction.moving) yield break;
        current_playerMovementAction = playerMovementAction.jumping;
        action_CanJump = false;


        float jumpVelocity = Mathf.Sqrt(-2 * -(gravityAcceleration * Mathf.Exp(2)) * jumpHeight);
        vertical_playerVelocity = Vector3.zero;
        vertical_playerVelocity += transform.up * jumpVelocity;

        yield return new WaitForSeconds(0.015f);
        current_playerMovementAction = playerMovementAction.moving;
        yield return new WaitForSeconds(action_jumpCooldownTime);
        action_CanJump = true;
    }
    private IEnumerator action_Dash()
    {
        if (!action_CanDash || current_playerMovementAction != playerMovementAction.moving) yield break;
        current_playerMovementAction = playerMovementAction.dashing;
        action_CanDash = false;

        Vector3 tempDashDirectionalVector = Vector3.zero;
        if (current_PlayerInputActions.playerMovment.HorizontalMovement.ReadValue<Vector2>().magnitude == 0) tempDashDirectionalVector = directionalOrientation.transform.forward.normalized;
        else tempDashDirectionalVector = current_playerDirectionalVector.normalized;


        horizontal_playerVelocity *= 0;
        float dashVelocity = Mathf.Sqrt(-2 * -acceleration * dashDistance * 3.25f);
        horizontal_playerVelocity += tempDashDirectionalVector * dashVelocity;

        current_playerMovementAction = playerMovementAction.moving;
        yield return new WaitForSeconds(action_dashCooldownTime);
        action_CanDash = true;
    }
    private IEnumerator action_Slide()
    {
        if (!action_CanSlide || !grounded || current_playerMovementAction != playerMovementAction.moving) yield break;
        current_playerMovementAction = playerMovementAction.sliding;
        action_CanSlide = false;
        playerCollider.height = colliderHeight_Slide;
        Vector3 slideTempDir = Vector3.zero;
        if (current_PlayerInputActions.playerMovment.HorizontalMovement.ReadValue<Vector2>().magnitude == 0) slideTempDir = directionalOrientation.transform.forward.normalized;
        else slideTempDir = current_playerDirectionalVector.normalized;
        transform.position -= Vector3.up * colliderHeight_Slide;
        canAffectMovement = false;
        gravityAffected = false;

        while (current_PlayerInputActions.playerMovment.Slide.IsPressed())
        {
            horizontal_playerVelocity += slideTempDir * acceleration_Slide * Time.deltaTime;
            yield return null;
        }

        transform.position += Vector3.up * colliderHeight_Slide;
        playerCollider.height = colliderHeight_normal;
        gravityAffected = true;
        canAffectMovement = true;
        current_playerMovementAction = playerMovementAction.moving;
        yield return new WaitForSeconds(action_slideCooldownTime);
        action_CanSlide = true;
    }
    private IEnumerator action_Slam()
    {
        if (!action_CanSlam || grounded || current_playerMovementAction != playerMovementAction.moving) yield break;

        current_playerMovementAction = playerMovementAction.slamming;
        action_CanSlam = false;
        gravityAffected = false;
        canAffectMovement = false;
        float tempTimer = 0;
        vertical_playerVelocity = Vector3.zero;

        while (tempTimer < timeInSeconds_ToRaise)
        {
            vertical_playerVelocity = directionalOrientation.transform.up * (raiseDistance_Slam / timeInSeconds_ToRaise);

            tempTimer += Time.deltaTime;
            yield return null;
        }
        vertical_playerVelocity = Vector3.zero;
        while (!grounded)
        {
            vertical_playerVelocity += -directionalOrientation.transform.up * acceleration_Slam_Downward * Time.deltaTime;
            yield return null;

        }
        gravityAffected = true;
        canAffectMovement = true;
        vertical_playerVelocity = Vector3.zero;
        current_playerMovementAction = playerMovementAction.moving;
        yield return new WaitForSeconds(action_slamCooldownTime);
        action_CanSlam = true;
    }

}

[System.Serializable]
public enum playerMovementAction
{
    moving = default,
    jumping,
    dashing,
    sliding,
    slamming,
    flipping
}

[System.Serializable]
public enum playerRotationState
{
    nonFlipped = default,
    flipped
}