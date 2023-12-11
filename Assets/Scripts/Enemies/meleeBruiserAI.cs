using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.iOS;

public class meleeBruiserAI : MonoBehaviour
{
    [Header("Debug Things")]
    [SerializeField] private bruiserAIStates currentAIState;
    [SerializeField] private bool canPunch;
    [SerializeField] private float punchAttackCooldown, hitStunDuration;

    [Header("Punch Attack Stats")]
    [SerializeField] private bool hitboxActive_Punch;
    [SerializeField] private float timeBeforePunchAttack;
    [SerializeField] private float distanceToPunchPlayer;
    [SerializeField] private Vector3 punchAttackHitboxCenter, punchAttackHitboxSize;
    [SerializeField] private LayerMask ref_playerLayer;
    private bool hasHitPlayerThisAttack = false;

    #region Assignables
    private NavMeshAgent ref_NavMeshAgent;
    private GameObject ref_PlayerObj;
    private playerHealth ref_PlayerStats;
    private Animator ref_meleeAnimator;
    private enemyStats ref_EnemyStats;
    #endregion

    private void Awake()
    {
        ref_NavMeshAgent = GetComponent<NavMeshAgent>();
        ref_PlayerObj = GameObject.Find("Player");
        ref_PlayerStats = ref_PlayerObj.GetComponent<playerHealth>();
        ref_meleeAnimator = GetComponent<Animator>();
        ref_EnemyStats = GetComponent<enemyStats>();

        ref_EnemyStats.onDamageTaken.AddListener(startTakeDamage);
    }

    private void Update()
    {
        if (currentAIState == bruiserAIStates.following)
        {
            if(Mathf.Abs(ref_NavMeshAgent.velocity.magnitude) > 0) ref_meleeAnimator.Play("BasicMeleeWalk");
            else ref_meleeAnimator.Play("BasicMeleeAngy");
            ref_NavMeshAgent.SetDestination(ref_PlayerObj.transform.position);
        }
        if (canPunch && Vector3.Distance(transform.position + Vector3.up * 1.5f, ref_PlayerObj.transform.position) <= distanceToPunchPlayer) StartCoroutine(action_PunchPlayer());

        if (hitboxActive_Punch)
        {
            if(Physics.CheckBox(transform.position + transform.TransformDirection(punchAttackHitboxCenter), punchAttackHitboxSize, transform.rotation, ref_playerLayer) && !hasHitPlayerThisAttack)
            {
                StartCoroutine(ref_PlayerStats.takeDamage(1));
                hasHitPlayerThisAttack = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(ref_PlayerObj != null) 
        {
            if (canPunch && Vector3.Distance(transform.position, ref_PlayerObj.transform.position) < 15f)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, ref_PlayerObj.transform.position);
            }
        }

        #region punchDebug
        if (hitboxActive_Punch) Gizmos.color = Color.red;
        else if (canPunch) Gizmos.color = Color.green;
        else Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(transform.localPosition + transform.TransformDirection(punchAttackHitboxCenter), 0.05f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(punchAttackHitboxCenter, punchAttackHitboxSize * 2f);
        #endregion
    }

    void startTakeDamage()
    {
        StartCoroutine(action_TakeDamage());
    }

    public void toggle_PunchHitbox()
    {
        hitboxActive_Punch = !hitboxActive_Punch;
    }

    IEnumerator action_PunchPlayer()
    {
        currentAIState = bruiserAIStates.punching;
        ref_NavMeshAgent.isStopped = true;
        ref_NavMeshAgent.velocity = Vector3.zero;
        canPunch = false;

        ref_meleeAnimator.Play("BasicMeleePunch");
        yield return new WaitForSeconds(ref_meleeAnimator.GetCurrentAnimatorClipInfo(0).Length + 0.1f);

        ref_NavMeshAgent.isStopped = false;
        currentAIState = bruiserAIStates.following;
        yield return new WaitForSeconds(punchAttackCooldown);
        canPunch = true;
        hasHitPlayerThisAttack = false;
    }

    IEnumerator action_TakeDamage()
    {
        currentAIState = bruiserAIStates.hitstun;
        StopCoroutine(action_PunchPlayer());
        canPunch = true;
        ref_NavMeshAgent.isStopped = true;
        ref_meleeAnimator.Play("BasicMeleeHit");
        yield return new WaitForSeconds(ref_meleeAnimator.GetCurrentAnimatorStateInfo(0).length);
        ref_NavMeshAgent.isStopped = false;
        currentAIState = bruiserAIStates.following;
    }
}

enum bruiserAIStates
{
    following,
    punching,
    hitstun
}
