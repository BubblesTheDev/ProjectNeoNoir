using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class playerMovement : MonoBehaviour
{
    [Header("Important Information")]
    public playerMovementAction current_playerMovementAction;
    public bool grounded;
    public bool onAcceptableSlope;
    public bool gravityAffected = true;
    public bool canAffectMovement = true;
    public bool canAffectRotation = true;
    public bool dragAffected = true;

    #region Action Cooldowns
    [Space, Header("Cooldowns and If The Player Can Do That Action")]
    public float action_jumpCooldownTime = 0.1f;
    public float action_dashCooldownTime = 0.1f;
    public float action_slideCooldownTime = 0.1f;
    public float action_slamCooldownTime = 0.1f;
    public float action_flipCooldownTime = 0.1f;
    public bool action_CanJump { private set; get; } = true;
    public bool action_CanDash { private set; get; } = true;
    public bool action_CanSlide { private set; get; } = true;
    public bool action_CanSlam { private set; get; } = true;
    public bool action_CanFlip { private set; get; } = true;
    #endregion

    #region Ground Check Calculations
    [Space, Header("Ground Calculations")]
    public Vector3 groundCheck_PositionalOffset;
    public float timeInSeconds_GroundedCoyoteTime;
    public float current_CoyoteTime;
    private RaycastHit groundCheck_HitInformation;
    public float groundCheck_Distance;
    public LayerMask groundCheck_LayersToHit;
    #endregion

    #region horizontal Movement
    [Space, Header("Horizontal Movement")]
    [SerializeField] private Vector3 horizontal_playerVelocity;
    public float acceleration_Ground, acceleration_Air;
    public float terminalVelocity_Ground, terminalVelocity_Air;
    public float speedStopThreshold;
    public float maxSlopeAngle;
    #endregion

    #region Vertical Movement
    [Space, Header("Vertical Movement")]
    [SerializeField] private Vector3 vertical_playerVelocity;
    public float jumpHeight;
    public int numberOf_MidairJumps = 1;
    private int current_NumberOfMidairJumps;
    public float gravityAcceleration;
    public float terminalVelocity_Gravity;
    public float terminalVelocity_Jump;
    #endregion

    #region Action Dash
    [Space, Header("Action Dash")]
    public float dashDistance;
    public float dashDuration;
    public float dashVelocityDivider = 0.75f;
    public int numberOf_MaximumDashCharges = 3;
    public float current_NumberOfDashCharges { private set; get; }
    public float timeInSeconds_ToRechargeOneCharge = 1f;
    #endregion

    #region Action Slide
    [Space, Header("Action Slide")]
    public float acceleration_Slide;
    public float acceleration_SlideJumpBoost;
    [HideInInspector] public bool canSlideJump;
    public float timeInSeconds_ExtraJumpCoyoteTime;
    public float terminalVelocity_Slide;
    [HideInInspector] public float colliderHeight_normal;
    public float colliderHeight_Slide;
    #endregion

    #region Action Slam
    [Space, Header("Action Slam")]
    public float raiseDistance_Slam;
    public float timeInSeconds_ToRaise;
    public float acceleration_Slam_Downward;
    public float terminalVelocity_Slam_Downward;
    #endregion

    #region Action Flip
    [Space, Header("Action Gravity Flip")]
    public playerRotationState current_PlayerRotationState;
    public float timeInSeconds_ToFlip;
    public float timeInSeconds_CurrentGravityFlipDuration;
    public float timeInSeconds_GravityFlipDuration;
    public float timeInSeconds_ToFullyRechargeGravity;
    public bool overchargedGravityFlip;
    public float overchargeRechargePenalty = .75f;
    public Vector3 directionalVector_NonFlipped = new Vector3(0, -1, 0);
    public Vector3 directionalVector_Flipped = new Vector3(0, 1, 0);
    [HideInInspector] public Vector3 rotationalOffset;
    #endregion

    #region Assignables
    private Rigidbody rb;
    private GameObject directionalOrientation;
    private MovementInputActions current_PlayerInputActions;
    private CapsuleCollider playerCollider;
    #endregion

    #region control variables
    private Vector3 current_playerDirectionalVector;
    #endregion

    #region Action Events
    [HideInInspector] public UnityEvent onAction_Jump_Start;
    [HideInInspector] public UnityEvent onAction_SlideJumpStart;
    [HideInInspector] public UnityEvent onAction_Dash_Start;
    [HideInInspector] public UnityEvent onAction_Slide_Start;
    [HideInInspector] public UnityEvent onAction_Slam_Start;
    [HideInInspector] public UnityEvent onAction_Flip_Start;
    [HideInInspector] public UnityEvent onAction_Slide_End;
    [HideInInspector] public UnityEvent onAction_Slam_End;
    [HideInInspector] public UnityEvent onAction_Flip_End;
    [HideInInspector] public UnityEvent onAction_CannotAirJump;
    [HideInInspector] public UnityEvent onAction_CannotDash;
    [HideInInspector] public UnityEvent onAction_CannotFlip;
    [HideInInspector] public UnityEvent onAction_OverchargeFlip;
    #endregion

    #region Debug Variables
    private Vector3 positionLastFrame;
    private float current_DragForce;
    Vector3 tempJumpStartVector;
    #endregion

    #region Editor Options
    [Space, Header("Debug Descisions")]
    public bool debug_ShowGroundedRay;
    public bool debug_ShowJumpHeight;
    public bool debug_ShowDashDistance;
    public bool debug_showPlayerSpeed;
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
        colliderHeight_normal = playerCollider.height;
        timeInSeconds_CurrentGravityFlipDuration = timeInSeconds_GravityFlipDuration;
        if (GameObject.Find("Orientation").gameObject) directionalOrientation = GameObject.Find("Orientation").gameObject;
        else Debug.LogWarning("There is no object in the scene named 'Orientation'. \nThe player movement script will not function properly without one. Please create one and try again.");
    }

    private void Update()
    {
        getPlayerInput();
        groundDetection();
        rechargeDashCharges();
        rechargeGravity();
    }

    private void FixedUpdate()
    {
        if (gravityAffected) applyGravity();
        if (canAffectRotation) transform.rotation = directionalOrientation.transform.rotation;
        if (canAffectMovement)
        {
            applyHorizontalAcceleration();
        }

        rb.velocity = horizontal_playerVelocity + vertical_playerVelocity;
        if (dragAffected) applyDrag();
        positionLastFrame = transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        #region Ground check raycast gizmo
        if (debug_ShowGroundedRay)
        {
            if (grounded && onAcceptableSlope) Gizmos.color = Color.green;
            else if (grounded && !onAcceptableSlope) Gizmos.color = Color.yellow;
            else Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + groundCheck_PositionalOffset, -transform.up * groundCheck_Distance);
        }
        #endregion

        #region Dash Distance Gizmo
        if (debug_ShowDashDistance)
        {
            if (current_NumberOfDashCharges > 0) Handles.color = Color.green;
            else Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.up, dashDistance);
        }
        #endregion

        #region Draw Jump Heigt
        if (debug_ShowJumpHeight && current_PlayerInputActions != null)
        {
            if (current_playerMovementAction != playerMovementAction.jumping && grounded) tempJumpStartVector = transform.position;
            if (current_PlayerInputActions.playerMovment.Jump.WasPressedThisFrame()) tempJumpStartVector = transform.position;
            if (action_CanJump)
            {
                if (grounded || !grounded && current_NumberOfMidairJumps > 0)
                {
                    Gizmos.color = Color.green;
                }
            }
            else Gizmos.color = Color.red;
            for (int i = 0; i < jumpHeight - 1; i++)
            {
                Handles.DrawWireDisc(tempJumpStartVector + (transform.up * (i - 0.75f)), transform.up, playerCollider.radius);
            }
        }
        #endregion
    }

    private void OnGUI()
    {
        GUIStyle tempstyle = new GUIStyle();
        tempstyle.fontSize = 30;
        tempstyle.normal.textColor = Color.white;
        if (debug_showPlayerSpeed)
        {
            GUI.Label(new Rect(10, 10 + 100, 100 * 4, 20 * 4), "Speed: " + Mathf.Round((Vector3.Distance(transform.position, positionLastFrame) / Time.deltaTime)).ToString() + "Ups", tempstyle);
            GUI.Label(new Rect(10, 40 + 100, 100 * 4, 20 * 4), "Velocity Magnitude: " + directionalOrientation.transform.InverseTransformDirection(rb.velocity).magnitude.ToString(), tempstyle);
        }
    }
#endif

    private void groundDetection()
    {
        if (current_playerMovementAction == playerMovementAction.jumping || current_playerMovementAction == playerMovementAction.flipping) return;

        Vector3 tempPositionaloffSet = groundCheck_PositionalOffset;
        if (current_PlayerRotationState == playerRotationState.nonFlipped) tempPositionaloffSet *= directionalVector_Flipped.y;
        else tempPositionaloffSet *= directionalVector_NonFlipped.y;
        if (Physics.Raycast(transform.position + tempPositionaloffSet, -transform.up, out groundCheck_HitInformation, groundCheck_Distance, groundCheck_LayersToHit))
        {
            float tempAngle = Vector3.Angle(transform.up, groundCheck_HitInformation.normal);
            if (tempAngle < maxSlopeAngle)
            {
                grounded = true;
                onAcceptableSlope = true;
                if (current_CoyoteTime <= 0)
                {
                    current_CoyoteTime = timeInSeconds_GroundedCoyoteTime;
                    current_NumberOfMidairJumps = numberOf_MidairJumps;
                }
                return;
            }
            else
            {
                grounded = true;
                onAcceptableSlope = false;
                if (current_CoyoteTime <= 0)
                {
                    current_CoyoteTime = timeInSeconds_GroundedCoyoteTime;
                    current_NumberOfMidairJumps = numberOf_MidairJumps;
                }
                return;
            }
        }
        else if (current_CoyoteTime > 0)
        {
            current_CoyoteTime -= Time.deltaTime;
            grounded = true;
            onAcceptableSlope = false;
            return;
        }
        else
        {
            grounded = false;
            onAcceptableSlope = false;
            return;
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
        if (current_PlayerInputActions.playerMovment.Slide.IsPressed()) StartCoroutine(action_Slide());
        if (current_PlayerInputActions.playerMovment.Jump.WasPressedThisFrame()) StartCoroutine(action_Jump());
        if (current_PlayerInputActions.playerMovment.FlipGravity.WasPressedThisFrame()) StartCoroutine(action_Flip());
    }

    private void applyDrag()
    {
        #region horizontal drag
        float idealHDragForce = 0;
        if (grounded)
        {
            switch (current_playerMovementAction)
            {
                case playerMovementAction.moving:
                    idealHDragForce = acceleration_Ground / terminalVelocity_Ground;
                    break;
                case playerMovementAction.sliding:
                    idealHDragForce = acceleration_Slide / terminalVelocity_Slide;
                    break;
            }
        }
        else idealHDragForce = acceleration_Air / terminalVelocity_Air;

        current_DragForce = idealHDragForce;
        horizontal_playerVelocity *= 1 - Time.deltaTime * current_DragForce;
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
        if (grounded) horizontal_playerVelocity += current_playerDirectionalVector.normalized * acceleration_Ground * Time.deltaTime;
        else horizontal_playerVelocity += current_playerDirectionalVector.normalized * acceleration_Air * Time.deltaTime;
    }

    private void rechargeDashCharges()
    {
        if (current_NumberOfDashCharges < numberOf_MaximumDashCharges && action_CanDash)
        {
            current_NumberOfDashCharges += timeInSeconds_ToRechargeOneCharge * Time.deltaTime;
        }
        else if (current_NumberOfDashCharges > numberOf_MaximumDashCharges) current_NumberOfDashCharges = numberOf_MaximumDashCharges;
    }

    private void rechargeGravity()
    {
        if (current_PlayerRotationState == playerRotationState.nonFlipped)
        {
            if (timeInSeconds_CurrentGravityFlipDuration < timeInSeconds_GravityFlipDuration && !overchargedGravityFlip)
            {
                timeInSeconds_CurrentGravityFlipDuration += (timeInSeconds_ToFullyRechargeGravity / timeInSeconds_GravityFlipDuration) * Time.deltaTime;
            }
            else if (timeInSeconds_CurrentGravityFlipDuration < timeInSeconds_GravityFlipDuration && overchargedGravityFlip)
            {
                timeInSeconds_CurrentGravityFlipDuration += (timeInSeconds_ToFullyRechargeGravity / timeInSeconds_GravityFlipDuration) * Time.deltaTime * overchargeRechargePenalty;
            }
        }
        else if (current_PlayerRotationState == playerRotationState.flipped)
        {
            timeInSeconds_CurrentGravityFlipDuration -= Time.deltaTime;
            if (timeInSeconds_CurrentGravityFlipDuration <= 0)
            {
                StartCoroutine(action_Flip());
                overchargedGravityFlip = true;
            }
        }

        if (timeInSeconds_CurrentGravityFlipDuration > timeInSeconds_GravityFlipDuration)
        {
            timeInSeconds_CurrentGravityFlipDuration = timeInSeconds_GravityFlipDuration;
            overchargedGravityFlip = false;
        }
    }

    private IEnumerator action_Jump()
    {
        if (!action_CanJump || current_playerMovementAction != playerMovementAction.moving && current_playerMovementAction != playerMovementAction.sliding) yield break;
        else if (!grounded && current_NumberOfMidairJumps <= 0)
        {
            onAction_CannotAirJump.Invoke();
            yield break;
        }
        else if (!grounded && current_NumberOfMidairJumps > 0) current_NumberOfMidairJumps--;

        current_playerMovementAction = playerMovementAction.jumping;
        action_CanJump = false;
        current_CoyoteTime = 0;
        grounded = false;
        vertical_playerVelocity = Vector3.zero;
        float jumpVelocity = Mathf.Sqrt(-2 * -(gravityAcceleration * Mathf.Exp(2)) * jumpHeight);
        vertical_playerVelocity += transform.up * jumpVelocity;
        if (canSlideJump)
        {
            onAction_SlideJumpStart.Invoke();
            horizontal_playerVelocity += directionalOrientation.transform.forward.normalized * acceleration_SlideJumpBoost;
        }
        else onAction_Jump_Start.Invoke();
        
        
        yield return new WaitForSeconds(0.015f);
        current_playerMovementAction = playerMovementAction.moving;
        yield return new WaitForSeconds(action_jumpCooldownTime);
        action_CanJump = true;
    }
    private IEnumerator action_Dash()
    {
        if (!action_CanDash || current_playerMovementAction != playerMovementAction.moving || current_NumberOfDashCharges < 1)
        {
            onAction_CannotDash.Invoke();
            yield break;
        }

        onAction_Dash_Start.Invoke();

        current_playerMovementAction = playerMovementAction.dashing;
        action_CanDash = false;
        dragAffected = false;
        canAffectMovement = false;
        gravityAffected = false;
        current_NumberOfDashCharges--;

        Vector3 tempDashDirectionalVector = Vector3.zero;
        if (current_PlayerInputActions.playerMovment.HorizontalMovement.ReadValue<Vector2>().magnitude == 0) tempDashDirectionalVector = Vector3.Scale(directionalOrientation.transform.forward, new Vector3(1, 0, 1)).normalized;
        else tempDashDirectionalVector = current_playerDirectionalVector.normalized;
        Vector3 dashStartPos = transform.position;

        vertical_playerVelocity *= .75f;
        float dashVelocity = dashDistance / dashDuration;
        horizontal_playerVelocity += tempDashDirectionalVector * dashVelocity;


        float dashOverride = 0f;
        while (Vector3.Distance(dashStartPos, transform.position) < dashDistance && dashOverride < dashDuration)
        {
            dragAffected = false;
            dashOverride += Time.deltaTime;
            yield return null;
        }
        horizontal_playerVelocity -= tempDashDirectionalVector * dashVelocity;

        dragAffected = true;
        gravityAffected = true;
        canAffectMovement = true;
        current_playerMovementAction = playerMovementAction.moving;
        yield return new WaitForSeconds(action_dashCooldownTime);
        action_CanDash = true;
        
    }
    private IEnumerator action_Slide()
    {

        if (!action_CanSlide || !grounded || current_playerMovementAction != playerMovementAction.moving) yield break;

        onAction_Slide_Start.Invoke();
        current_playerMovementAction = playerMovementAction.sliding;
        action_CanSlide = false;
        playerCollider.height = colliderHeight_Slide;
        Vector3 slideTempDir = Vector3.zero;
        if (current_PlayerInputActions.playerMovment.HorizontalMovement.ReadValue<Vector2>().magnitude == 0) slideTempDir = directionalOrientation.transform.forward.normalized;
        else slideTempDir = current_playerDirectionalVector.normalized;
        transform.position -= Vector3.up * colliderHeight_Slide;
        canAffectMovement = false;
        canSlideJump = true;

        while (current_PlayerInputActions.playerMovment.Slide.IsPressed() && current_playerMovementAction != playerMovementAction.jumping)
        {
            horizontal_playerVelocity += slideTempDir * acceleration_Slide * Time.deltaTime;
            yield return null;
        }
        // transform.position += Vector3.up * colliderHeight_Slide;
        playerCollider.height = colliderHeight_normal;
        canAffectMovement = true;
        current_playerMovementAction = playerMovementAction.moving;

        onAction_Slide_End.Invoke();

        float timeToWaitForCooldown = 0;
        while (canSlideJump || !action_CanSlide)
        {
            if(timeToWaitForCooldown >= action_dashCooldownTime) action_CanSlide = true;
            if(timeToWaitForCooldown >= timeInSeconds_ExtraJumpCoyoteTime) canSlideJump = false;
            timeToWaitForCooldown += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator action_Slam()
    {
        if (!action_CanSlam || grounded || current_playerMovementAction != playerMovementAction.moving) yield break;

        onAction_Slam_Start.Invoke();

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

        onAction_Slam_End.Invoke();

        gravityAffected = true;
        canAffectMovement = true;
        vertical_playerVelocity = Vector3.zero;
        current_playerMovementAction = playerMovementAction.moving;
        yield return new WaitForSeconds(action_slamCooldownTime);
        action_CanSlam = true;
    }
    private IEnumerator action_Flip()
    {
        if (!action_CanFlip || current_playerMovementAction != playerMovementAction.moving)
        {
            onAction_CannotFlip.Invoke();
            yield break;
        }
        if (current_PlayerRotationState == playerRotationState.nonFlipped && overchargedGravityFlip
            || current_PlayerRotationState == playerRotationState.nonFlipped && timeInSeconds_CurrentGravityFlipDuration < 1f)
        {
            onAction_CannotFlip.Invoke();
            yield break;
        }

        if (!overchargedGravityFlip)
        {
            if(current_PlayerRotationState == playerRotationState.nonFlipped) onAction_Flip_Start.Invoke();
            else onAction_Flip_End.Invoke();
        }
        else
        {
            onAction_OverchargeFlip.Invoke();
        }
        current_playerMovementAction = playerMovementAction.flipping;
        action_CanFlip = false;
        gravityAffected = false;
        canAffectMovement = false;
        canAffectRotation = false;
        vertical_playerVelocity = Vector3.zero;



        float tempTimer = 0;
        while (tempTimer < timeInSeconds_ToFlip)
        {
            directionalOrientation.transform.localEulerAngles += new Vector3(0, 0, (180 / timeInSeconds_ToFlip) * Time.deltaTime);
            tempTimer += Time.deltaTime;
            yield return null;
        }

        current_CoyoteTime = 0;
        grounded = false;

        if (current_PlayerRotationState == playerRotationState.nonFlipped) current_PlayerRotationState = playerRotationState.flipped;
        else current_PlayerRotationState = playerRotationState.nonFlipped;

        gravityAffected = true;
        canAffectRotation = true;
        canAffectMovement = true;
        current_playerMovementAction = playerMovementAction.moving;

        yield return new WaitForSeconds(action_flipCooldownTime);
        action_CanFlip = true;
    }

}
#if !UNITY_EDITOR
[CustomEditor(typeof(playerMovement)), CanEditMultipleObjects]
public class playerMovementEditor : Editor
{
    private bool showDebug;
    private bool showCooldowns;
    private bool showHorizontalMovement;
    private bool showVerticalMovement;
    private bool showGroundedCalculations;
    private bool showActions;
    private bool showActionJump;
    private bool showActionDash;
    private bool showActionSlide;
    private bool showActionSlam;
    private bool showActionFlip;
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        playerMovement reference = (playerMovement)target;

        GUILayout.Label("Important Information", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Player Is Grounded");
        EditorGUILayout.Toggle(reference.grounded);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Player Is On acceptable Slope");
        EditorGUILayout.Toggle(reference.onAcceptableSlope);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Player Is Affected By Drag");
        EditorGUILayout.Toggle(reference.dragAffected);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Player Is Affected By Gravity");
        EditorGUILayout.Toggle(reference.gravityAffected);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Player Can Affect Movement");
        EditorGUILayout.Toggle(reference.canAffectMovement);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Player Can Affect Rotation");
        EditorGUILayout.Toggle(reference.canAffectRotation);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        showDebug = EditorGUILayout.Foldout(showDebug, "Show Debug Menu", true, EditorStyles.foldoutHeader);
        if (showDebug)
        {
            EditorGUILayout.BeginVertical();
            reference.debug_showAllowedActions = EditorGUILayout.Toggle("Enable Show Allowed Actions", reference.debug_showAllowedActions);
            reference.debug_ShowDashDistance = EditorGUILayout.Toggle("Enable Show Dash Distance", reference.debug_ShowDashDistance);
            reference.debug_ShowGroundedRay = EditorGUILayout.Toggle("Enable Show Ground Check Ray", reference.debug_ShowGroundedRay);
            reference.debug_ShowJumpHeight = EditorGUILayout.Toggle("Enable Show Jump Height", reference.debug_ShowJumpHeight);
            reference.debug_showPlayerSpeed = EditorGUILayout.Toggle("Enable Show Current Player Speed", reference.debug_showPlayerSpeed);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();

        showCooldowns = EditorGUILayout.Foldout(showCooldowns, "Show Action Cooldowns", true, EditorStyles.foldoutHeader);
        if (showCooldowns)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Player Can Jump", reference.action_CanJump);
            reference.action_jumpCooldownTime = EditorGUILayout.FloatField("Player Jump Cooldown", reference.action_jumpCooldownTime);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Player Can Dash", reference.action_CanDash);
            reference.action_dashCooldownTime = EditorGUILayout.FloatField("Player Dash Cooldown", reference.action_dashCooldownTime);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Player Can Slide", reference.action_CanSlide);
            reference.action_slideCooldownTime = EditorGUILayout.FloatField("Player Slide Cooldown", reference.action_slideCooldownTime);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Player Can Buttslam", reference.action_CanSlam);
            reference.action_slamCooldownTime = EditorGUILayout.FloatField("Player Buttslam Cooldown", reference.action_slamCooldownTime);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Player Can Flip Gravity", reference.action_CanFlip);
            reference.action_flipCooldownTime = EditorGUILayout.FloatField("Player Gravity Flip Cooldown", reference.action_flipCooldownTime);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();

        showHorizontalMovement = EditorGUILayout.Foldout(showHorizontalMovement, "Show Horizontal Movement Variables", true, EditorStyles.foldoutHeader);
        if (showHorizontalMovement)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Terminal velocity on The ground");
            GUILayout.Label("Terminal velocity in The Air");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            reference.terminalVelocity_Ground = EditorGUILayout.FloatField(reference.terminalVelocity_Ground);
            reference.terminalVelocity_Air = EditorGUILayout.FloatField(reference.terminalVelocity_Air);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Acceleration on The ground");
            GUILayout.Label("Acceleration in The Air");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            reference.acceleration_Ground = EditorGUILayout.FloatField(reference.acceleration_Ground);
            reference.acceleration_Air = EditorGUILayout.FloatField(reference.acceleration_Air);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Slope Angle Player Can Climb");
            GUILayout.Label("Minimum Speed Threshold to fully stop");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            reference.maxSlopeAngle = EditorGUILayout.FloatField(reference.maxSlopeAngle);
            reference.speedStopThreshold = EditorGUILayout.FloatField(reference.speedStopThreshold);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();

        showVerticalMovement = EditorGUILayout.Foldout(showVerticalMovement, "Show Vertical Movement Variables", true, EditorStyles.foldoutHeader);
        if (showVerticalMovement)
        {
            EditorGUILayout.BeginHorizontal();
            reference.gravityAcceleration = EditorGUILayout.FloatField("Gravity Acceleration", reference.gravityAcceleration);
            reference.terminalVelocity_Gravity = EditorGUILayout.FloatField("Gravity Terminal Velocity", reference.terminalVelocity_Gravity);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();

        showGroundedCalculations = EditorGUILayout.Foldout(showGroundedCalculations, "Show Ground Calculations", true, EditorStyles.foldoutHeader);
        if (showGroundedCalculations)
        {
            reference.groundCheck_PositionalOffset = EditorGUILayout.Vector3Field("Groundcheck Start Position Offset", reference.groundCheck_PositionalOffset);
            reference.groundCheck_Distance = EditorGUILayout.FloatField("Ground Check Max Distance", reference.groundCheck_Distance);
            reference.groundCheck_LayersToHit = EditorGUILayout.LayerField("Layers Ground Check Will Look For", reference.groundCheck_LayersToHit);
            EditorGUILayout.BeginHorizontal();
            reference.timeInSeconds_GroundedCoyoteTime = EditorGUILayout.FloatField("Max Coyote Time", reference.timeInSeconds_GroundedCoyoteTime);
            EditorGUILayout.FloatField("Coyote Time Reimaining", reference.current_CoyoteTime);
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.Space();

        showActions = EditorGUILayout.Foldout(showActions, "Show Actions", true, EditorStyles.foldoutHeader);
        if (showActions)
        {
            EditorGUILayout.BeginHorizontal();
            showActionDash = EditorGUILayout.Toggle("Show Dash Variables", showActionDash);
            showActionSlide = EditorGUILayout.Toggle("Show Slide Variables", showActionSlide);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            showActionJump = EditorGUILayout.Toggle("Show Jump Variables", showActionJump);
            showActionSlam = EditorGUILayout.Toggle("Show Buttslam Variables", showActionSlam);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            showActionFlip = EditorGUILayout.Toggle("Show Gravity Flip Variables", showActionFlip);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        if (showActionDash)
        {
            GUILayout.Label("Dash Varialbes", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            reference.numberOf_MaximumDashCharges = EditorGUILayout.IntField("Max Number Of Dash Charges", reference.numberOf_MaximumDashCharges);
            EditorGUILayout.FloatField("Current Number Of Dash Charges Available", Mathf.Round(reference.current_NumberOfDashCharges));
            EditorGUILayout.EndHorizontal();
            reference.timeInSeconds_ForASingleCharge = EditorGUILayout.FloatField("Charge Rate Per charge", reference.timeInSeconds_ForASingleCharge);
            EditorGUILayout.BeginHorizontal();
            reference.dashDistance = EditorGUILayout.FloatField("Maximum Dash Distance", reference.dashDistance);
            reference.dashDuration = EditorGUILayout.FloatField("Time To Complete Dash", reference.dashDuration);
            EditorGUILayout.EndHorizontal();
        }
        if (showActionSlide)
        {
            GUILayout.Label("Slide Varialbes", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            reference.acceleration_Slide = EditorGUILayout.FloatField("Acceleration During Slide", reference.acceleration_Slide);
            reference.terminalVelocity_Slide = EditorGUILayout.FloatField("Terminal Velocity During Slide", reference.terminalVelocity_Slide);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            reference.acceleration_EndSlideJump = EditorGUILayout.FloatField("Extra Height Given For Slide Jump", reference.acceleration_EndSlideJump);
            reference.timeInSeconds_ExtraJumpCoyoteTime = EditorGUILayout.FloatField("Time In Seconds Given To Do Slide Jump", reference.timeInSeconds_ExtraJumpCoyoteTime);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            reference.colliderHeight_Slide = EditorGUILayout.FloatField("Player Height When Sliding", reference.colliderHeight_Slide);
            EditorGUILayout.FloatField("Player Height When Not Sliding", reference.colliderHeight_normal);
            EditorGUILayout.EndHorizontal();

        }
        if (showActionJump)
        {
            GUILayout.Label("Jump Varialbes", EditorStyles.boldLabel);
            reference.numberOf_MidairJumps = EditorGUILayout.IntField("Number Of Jumps Allowed In Midair", reference.numberOf_MidairJumps);
            reference.jumpHeight = EditorGUILayout.FloatField("Jump Height In Units", reference.jumpHeight);
            reference.terminalVelocity_Jump = EditorGUILayout.FloatField("Terminal Velocity Player Is allowed To Jump", reference.terminalVelocity_Jump);
        }
        if (showActionSlam)
        {
            GUILayout.Label("Slam Varialbes", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            reference.raiseDistance_Slam = EditorGUILayout.FloatField("Distance To Raise at beginning of Slam", reference.raiseDistance_Slam);
            reference.timeInSeconds_ToRaise = EditorGUILayout.FloatField("Time In Seconds To Raise To Max Slam Height", reference.timeInSeconds_ToRaise);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            reference.acceleration_Slam_Downward = EditorGUILayout.FloatField("Slam Acceleration", reference.acceleration_Slam_Downward);
            reference.terminalVelocity_Slam_Downward = EditorGUILayout.FloatField("Slam Terminal Velocity", reference.terminalVelocity_Slam_Downward);
            EditorGUILayout.EndHorizontal();
        }
        if (showActionFlip)
        {
            GUILayout.Label("Gravity Flip Varialbes", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Current Player Rotation State");
            EditorGUILayout.EnumPopup(reference.current_PlayerRotationState);
            EditorGUILayout.EndHorizontal();
            reference.timeInSeconds_ToFlip = EditorGUILayout.FloatField("Time In Seconds To Flip gravity", reference.timeInSeconds_ToFlip);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Normal Gravity Directional Vector");
            GUILayout.Label("Flipped Gravity Directional Vector");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            reference.directionalVector_NonFlipped = EditorGUILayout.Vector3Field("", reference.directionalVector_NonFlipped);
            reference.directionalVector_Flipped = EditorGUILayout.Vector3Field("", reference.directionalVector_Flipped);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUI.EndChangeCheck();

    }
}
#endif

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