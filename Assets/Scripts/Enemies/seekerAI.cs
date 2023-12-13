using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using UnityEngine.AI;
using UnityEngine.InputSystem.iOS;
using System.IO;

public class seekerAI : MonoBehaviour
{


    [Header("Debug Things")]
    [SerializeField] private seekerAIStates currentAIState;
    [SerializeField] private bool canUseSlash, canUseLeap, canUseDash;
    [SerializeField] private float slashAttackCooldown, leapAttackCooldown, dashMovmentCooldown, hitStunDuration;

    [Space, Header("Dash Movement Stats")]
    [SerializeField] private float randomnessPathRadius, pathwaySizeRadius, dashSpeed, timeBetweenDash;
    [SerializeField] private LayerMask enviromentLayer;
    [SerializeField] private TrailRenderer[] movementTrails;
    private List<Vector3> dashPath = new List<Vector3>();

    [Space, Header("Slash Attack Stats")]
    [SerializeField] private int slashDamage;
    [SerializeField] private bool hitboxActive_Slash;
    [SerializeField] private float timeBeforeSlashAttack;
    [SerializeField] private Vector3 slashAttackHitboxCenter, slashAttackHitboxSize;

    [Space, Header("Leap Attack Stats")]
    [SerializeField] private int leapDamage;
    [SerializeField] private bool hitboxActive_Leap;
    [SerializeField] private float leapAttackHitboxHeight, leapAttackHitboxRadius;
    [SerializeField] private int numOfLeaps = 3;
    [SerializeField] private float time_lineUpLeap, time_leapReaction, time_calmDown;
    [SerializeField] private LineRenderer leapAttackEyeline;
    [SerializeField] private bool playerIsVisable;
    private LayerMask ref_playerLayer;

    #region Assignables
    private NavMeshAgent ref_NavMeshAgent;
    private GameObject ref_PlayerObj;
    private playerMovement ref_PlayerMovement;
    private playerHealth ref_PlayerStats;
    private Animator ref_seekerAnimator;
    #endregion

    private void Awake()
    {
        ref_NavMeshAgent = GetComponent<NavMeshAgent>();
        ref_PlayerObj = GameObject.Find("Player");
        ref_PlayerMovement = ref_PlayerObj.GetComponent<playerMovement>();
        ref_PlayerStats = ref_PlayerObj.GetComponent<playerHealth>();
        ref_playerLayer = ref_PlayerObj.layer;
    }

    private void OnDrawGizmosSelected()
    {
        #region slashDebugs
        if (hitboxActive_Slash) Gizmos.color = Color.red;
        else if (canUseSlash) Gizmos.color = Color.green;
        else Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(transform.position + slashAttackHitboxCenter, 0.05f);
        Gizmos.DrawWireCube(transform.position + slashAttackHitboxCenter, slashAttackHitboxSize * 2f);
        #endregion
    }

    private void Update()
    {
        if (hitboxActive_Slash && Physics.CheckBox(transform.position + slashAttackHitboxCenter, slashAttackHitboxCenter, transform.rotation, ref_playerLayer))
        {
            StartCoroutine(ref_PlayerStats.takeDamage(slashDamage));
        }
    }

    IEnumerator action_Dash()
    {
        Vector3 playerPosition = ref_PlayerObj.transform.position;
        currentAIState = seekerAIStates.dashing;
        dashPath.Clear();
        ref_NavMeshAgent.isStopped = true;
        canUseDash = false;

        #region create Path to dash to
        ref_NavMeshAgent.ResetPath();
        ref_NavMeshAgent.SetDestination(playerPosition);

        RaycastHit hit;

        int overflowValue = 0;
        for (int i = 0; i < ref_NavMeshAgent.path.corners.Length;)
        {

            Vector3 potentialPosition = ref_NavMeshAgent.path.corners[i + 1] + (Vector3)(Random.insideUnitCircle * randomnessPathRadius);
            if (i == 0 && !Physics.SphereCast(transform.position, pathwaySizeRadius, potentialPosition - transform.position, out hit,
                Vector3.Distance(potentialPosition, transform.position), enviromentLayer)
                || !Physics.SphereCast(ref_NavMeshAgent.path.corners[i], pathwaySizeRadius, potentialPosition - ref_NavMeshAgent.path.corners[i], out hit,
                Vector3.Distance(potentialPosition, ref_NavMeshAgent.path.corners[i]), enviromentLayer))
            {
                dashPath.Add(potentialPosition);
                i++;
            }
            else overflowValue++;
            if (overflowValue >= 10) dashPath.Add(ref_NavMeshAgent.path.corners[i + 1]);
        }
        #endregion

        #region dash between positions
        for (int i = 0; i < dashPath.Count;)
        {
            transform.LookAt(dashPath[i]);
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
            while (Vector3.Distance(transform.position, dashPath[i]) > 1.5f)
            {
                ref_NavMeshAgent.velocity = (dashPath[i] - transform.position).normalized * dashSpeed * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(timeBetweenDash);
        }
        #endregion

        ref_NavMeshAgent.isStopped = false;
        yield return new WaitForSeconds(dashMovmentCooldown);
        canUseDash = true;
    }

    IEnumerator action_Slash()
    {
        currentAIState = seekerAIStates.slashing;
        ref_NavMeshAgent.isStopped = true;
        canUseSlash = false;

        ref_seekerAnimator.Play("seekerSlash");
        yield return new WaitForSeconds(ref_seekerAnimator.GetCurrentAnimatorClipInfo(0).Length + 0.1f);

        ref_NavMeshAgent.isStopped = false;
        currentAIState = seekerAIStates.following;
        yield return new WaitForSeconds(slashAttackCooldown);
        canUseSlash = true;
    }

    IEnumerator action_Leap()
    {
        currentAIState = seekerAIStates.leaping;
        ref_NavMeshAgent.isStopped = true;
        canUseLeap = false;

        ref_seekerAnimator.Play("leapStartPose");
        yield return new WaitForSeconds(ref_seekerAnimator.GetCurrentAnimatorClipInfo(0).Length);

        #region Line Up Leap

        #endregion
    }

    public void toggle_HitboxActive_Slash()
    {
        hitboxActive_Slash = !hitboxActive_Slash;
    }
    public void toggle_HitboxActive_Leap()
    {
        hitboxActive_Leap = !hitboxActive_Leap;
    }
}

[System.Serializable]
enum seekerAIStates
{
    following,
    dashing,
    slashing,
    leaping,
    hitstun
}
