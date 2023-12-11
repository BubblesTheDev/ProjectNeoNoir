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


    #region Assignables
    private NavMeshAgent ref_NavMeshAgent;
    private GameObject ref_PlayerObj;
    private playerHealth ref_PlayerStats;
    private Animator ref_meleeAnimator;
    private enemyStats ref_EnemyStats;
    public LayerMask ref_playerLayer;
    #endregion

    private void Awake()
    {
        ref_NavMeshAgent = GetComponent<NavMeshAgent>();
        ref_PlayerObj = GameObject.Find("Player");
        ref_PlayerStats = ref_PlayerObj.GetComponent<playerHealth>();
        ref_meleeAnimator = GetComponent<Animator>();
        ref_EnemyStats = GetComponent<enemyStats>();
       // ref_playerLayer = ref_PlayerObj.layer;

        ref_EnemyStats.onDamageTaken.AddListener(startTakeDamage);
    }

    private void Update()
    {
        if (ref_NavMeshAgent.velocity.magnitude > 0 && currentAIState == bruiserAIStates.following) ref_meleeAnimator.Play("BasicMeleeWalk");
        if (currentAIState == bruiserAIStates.following) ref_NavMeshAgent.SetDestination(ref_PlayerObj.transform.position);
        if (canPunch && Vector3.Distance(transform.position, ref_PlayerObj.transform.position) <= distanceToPunchPlayer) StartCoroutine(action_PunchPlayer());

        if (hitboxActive_Punch)
        {
            if(Physics.CheckBox(transform.localPosition + punchAttackHitboxCenter, punchAttackHitboxSize, transform.localRotation, ref_playerLayer))
            {
                StartCoroutine(ref_PlayerStats.takeDamage(1));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        #region punchDebug
        if (hitboxActive_Punch) Gizmos.color = Color.red;
        else if (canPunch) Gizmos.color = Color.green;
        else Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(transform.localPosition + punchAttackHitboxCenter, 0.05f);
        Gizmos.DrawWireCube(transform.localPosition + punchAttackHitboxCenter, punchAttackHitboxSize * 2f);
        #endregion
    }

    IEnumerator action_PunchPlayer()
    {
        currentAIState = bruiserAIStates.punching;
        ref_NavMeshAgent.isStopped = true;
        canPunch = false;

        ref_meleeAnimator.Play("BasicMeleePunch");
        yield return new WaitForSeconds(ref_meleeAnimator.GetCurrentAnimatorClipInfo(0).Length + 0.1f);

        ref_NavMeshAgent.isStopped = false;
        currentAIState = bruiserAIStates.following;
        yield return new WaitForSeconds(punchAttackCooldown);
        canPunch = true;
    }

    void startTakeDamage()
    {
        StartCoroutine(action_TakeDamage());
    }

    IEnumerator action_TakeDamage()
    {
        currentAIState = bruiserAIStates.hitstun;
        //StopCoroutine(hitPlayer());
        canPunch = true;
        ref_NavMeshAgent.isStopped = true;
        ref_meleeAnimator.Play("BasicMeleeHit");
        yield return new WaitForSeconds(ref_meleeAnimator.GetCurrentAnimatorStateInfo(0).length);
        ref_NavMeshAgent.isStopped = false;
        currentAIState = bruiserAIStates.following;
    }

    public void toggle_PunchHitbox()
    {
        hitboxActive_Punch = !hitboxActive_Punch;
    }
}

enum bruiserAIStates
{
    following,
    punching,
    hitstun
}
