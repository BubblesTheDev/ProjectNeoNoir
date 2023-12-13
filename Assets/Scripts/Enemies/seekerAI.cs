using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using UnityEngine.AI;
using UnityEngine.InputSystem.iOS;
using System.IO;
using UnityEditor;

public class seekerAI : MonoBehaviour
{


    [Header("Debug Things")]
    [SerializeField] private seekerAIStates currentAIState;
    [SerializeField] private bool canUseSlash = true, canUseLeap = true, canUseDash = true;
    [SerializeField] private float slashAttackCooldown, leapAttackCooldown, dashMovmentCooldown, hitStunDuration;

    [Header("Dash Movement Stats")]
    [SerializeField] private float distanceToStartDash;
    [SerializeField] private float randomnessPathRadius;
    [SerializeField] private float pathwaySizeRadius;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float numOfDashes;
    [SerializeField] private float timeBetweenDash;
    [SerializeField] private LayerMask enviromentLayer;
    [SerializeField] private TrailRenderer[] movementTrails;
    [SerializeField] private List<Vector3> dashPath = new List<Vector3>();

    [Header("Slash Attack Stats")]
    [SerializeField] private int slashDamage;
    [SerializeField] private bool hitboxActive_Slash;
    [SerializeField] private float timeBeforeSlashAttack;
    [SerializeField] private Vector3 slashAttackHitboxCenter, slashAttackHitboxSize;

    [Header("Leap Attack Stats")]
    [SerializeField] private int leapDamage;
    [SerializeField] private bool hitboxActive_Leap;
    [SerializeField] private float leapAttackHitboxHeight, leapAttackHitboxRadius;
    [SerializeField] private int numOfLeaps = 3;
    [SerializeField] private float time_lineUpLeap, time_leapReaction, time_calmDown;
    [SerializeField] private bool playerIsVisable;
    [SerializeField] private LayerMask ref_playerLayer;

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
    }

    private void OnDrawGizmosSelected()
    {
        #region slashDebugs
        if (hitboxActive_Slash) Gizmos.color = Color.red;
        else if (canUseSlash) Gizmos.color = Color.green;
        else Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(transform.position + slashAttackHitboxCenter, 0.05f);
        Gizmos.DrawWireCube(slashAttackHitboxCenter, slashAttackHitboxSize * 2f);
        #endregion

        #region Dash Debugs
        if (ref_PlayerObj != null)
        {
            if (currentAIState == seekerAIStates.following)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position + Vector3.up * 1.25f, ref_PlayerObj.transform.position);
            }
            else if (currentAIState == seekerAIStates.dashing && dashPath.Count > 0)
            {
                foreach (Vector3 dashPoint in dashPath)
                {
                    Gizmos.DrawSphere(dashPoint, 0.05f);
                }
            }
        }



        #endregion
    }

    private void Update()
    {
        if (currentAIState == seekerAIStates.following)
        {
            ref_NavMeshAgent.SetDestination(ref_PlayerObj.transform.position);
            //if (Mathf.Abs(ref_NavMeshAgent.velocity.magnitude) > 0) ref_seekerAnimator.Play("SeekerWalk");
            //else ref_seekerAnimator.Play("SeekerIdle");
        }

        if (Vector3.Distance(transform.position + Vector3.up * 1.25f, ref_PlayerObj.transform.position) < distanceToStartDash && canUseDash && ref_NavMeshAgent.isOnNavMesh)
        {
            StartCoroutine(action_Dash());
        }


        if (hitboxActive_Slash && Physics.CheckBox(transform.position + transform.TransformDirection(slashAttackHitboxCenter), slashAttackHitboxCenter, transform.rotation, ref_playerLayer)) StartCoroutine(ref_PlayerStats.takeDamage(slashDamage));
    }

    IEnumerator action_Dash()
    {
        Vector3 playerPosition = ref_PlayerObj.transform.position;
        currentAIState = seekerAIStates.dashing;
        dashPath.Clear();
        ref_NavMeshAgent.isStopped = true;
        ref_NavMeshAgent.velocity *= 0;
        canUseDash = false;


     

       

        currentAIState = seekerAIStates.following;
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
        Vector3 aimingDir = Vector3.zero ;

        ref_seekerAnimator.Play("leapStartPose");
        yield return new WaitForSeconds(ref_seekerAnimator.GetCurrentAnimatorClipInfo(0).Length);

        #region Line Up Leap
        
        //Start lining up leap
        float time = 0;
        while(time < time_lineUpLeap)
        {
            aimingDir = ref_PlayerObj.transform.position - transform.position;
            transform.LookAt(ref_PlayerObj.transform.position, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);



            time += Time.deltaTime;
            yield return null;
        }
        #endregion

        //Launch at the player
        #region launch at player
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
