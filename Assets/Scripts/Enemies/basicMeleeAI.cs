using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class basicMeleeAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float distanceToStop;
    [SerializeField] private GameObject hurtBox;
    private bool canHitPlayer;

    private GameObject playerObj;
    private enemyStats stats;
    [SerializeField] private Animator meleeAnimator;
    private meleeStates currentMeleeState;

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
        stats = GetComponent<enemyStats>();
        agent = GetComponent<NavMeshAgent>();
        canHitPlayer = true;

        stats.onDamageTaken.AddListener(startTakeDamage);
    }

    private void Update()
    {

        if (agent.velocity.magnitude > 0 && currentMeleeState == meleeStates.walking) meleeAnimator.Play("BasicMeleeWalk");
        if(Vector3.Distance(playerObj.transform.position, transform.position) < distanceToStop && canHitPlayer)
        {
            StartCoroutine(hitPlayer());   

        }

        agent.SetDestination(playerObj.transform.position);
    }

    

    IEnumerator hitPlayer()
    {
        currentMeleeState = meleeStates.punching;
        canHitPlayer = false;
        agent.isStopped = true;
        meleeAnimator.Play("BasicMeleePunch");
        yield return new WaitForSeconds(.3f);
        if (Vector3.Distance(playerObj.transform.position, transform.position) < distanceToStop) StartCoroutine(playerObj.GetComponent<playerHealth>().takeDamage(1));
        yield return new WaitForSeconds(.9f);
        agent.isStopped = false;
        currentMeleeState = meleeStates.walking;

        yield return new WaitForSeconds(.5f);
        canHitPlayer = true;

    }

    void startTakeDamage()
    {
        StartCoroutine(takeDamage());
    }

    IEnumerator takeDamage()
    {
        currentMeleeState = meleeStates.stunned;
        StopCoroutine(hitPlayer());
        canHitPlayer = true;
        agent.isStopped = true;
        meleeAnimator.Play("BasicMeleeHit");
        yield return new WaitForSeconds(meleeAnimator.GetCurrentAnimatorStateInfo(0).length);
        agent.isStopped = false;
        currentMeleeState = meleeStates.walking;

    }

}

public enum meleeStates
{
    walking,
    angy,
    punching,
    stunned,
    hitstun
}
