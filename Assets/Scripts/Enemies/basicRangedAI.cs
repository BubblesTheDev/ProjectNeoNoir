using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class basicRangedAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float startupTime = 5f;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float rangeAroundPlayer;
    [SerializeField] private List<GameObject> firePoints;
    [SerializeField] private GameObject enemyBullet;
    [SerializeField] private float bulletVelocity;

    private bool canShoot;

    private GameObject playerObj;
    private enemyStats stats;
    [SerializeField] private Animator currentAnimator;
    private rangeStates currentState;

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
        stats = GetComponent<enemyStats>();
        agent = GetComponent<NavMeshAgent>();
        canShoot = true;

        stats.onDamageTaken.AddListener(startTakeDamage);
        StartCoroutine(startUpAI());
    }

    private void Update()
    {

        if (agent.velocity.magnitude > 0 && currentState == rangeStates.walking) currentAnimator.Play("BasicRangedWalk");
        if (agent.remainingDistance < 1f && canShoot) StartCoroutine(shootPlayer()); 
    }

    void findDestination()
    {
        Vector3 tempDestination = transform.position + (Vector3)Random.insideUnitCircle * rangeAroundPlayer;

        agent.SetDestination(tempDestination);
    }

    IEnumerator startUpAI()
    {
        yield return new WaitForSeconds(startupTime);
        findDestination();
    }

    IEnumerator shootPlayer()
    {
        currentState = rangeStates.shooting;
        agent.isStopped = true;
        canShoot = false;
        currentAnimator.Play("BasicRangedShoot");

        Vector3 targetPos = new Vector3(playerObj.transform.position.x, transform.position.y, playerObj.transform.position.z);
        transform.LookAt(targetPos);
        yield return new WaitForSeconds(0.1f);

        currentAnimator.speed = 0;
        foreach (GameObject firePoint in firePoints)
        {
            for (int i = 0; i < (int)Random.Range(1,3); i++)
            {
                GameObject b = Instantiate(enemyBullet, firePoint.transform.position, Quaternion.LookRotation((playerObj.transform.position - firePoint.transform.position).normalized, Vector3.up), GameObject.Find("Bullet Storage").transform);
                Rigidbody rb = b.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * bulletVelocity, ForceMode.Impulse);
                //Play enemy shot here

                yield return new WaitForSeconds(0.1f);
            }
        }
        currentAnimator.speed = 1;

        yield return new WaitForSeconds(1.5f);
        currentState = rangeStates.walking;
        agent.isStopped = false;

        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
        findDestination();
    }
    
    void startTakeDamage()
    {
        StartCoroutine(takeDamage());
    }

    IEnumerator takeDamage()
    {
        currentState = rangeStates.hitstun;
        StopCoroutine(shootPlayer());
        canShoot = true;
        agent.isStopped = true;
        currentAnimator.Play("BasicRangedHit");
        yield return new WaitForSeconds(1.33f);
        agent.isStopped = false;
        currentState = rangeStates.walking;
    }

}

public enum rangeStates
{
    walking,
    shooting,
    stunned,
    hitstun
}
