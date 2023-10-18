using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicMeleeAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timeToAttack;
    [SerializeField] private float cooldownTime;

    private GameObject playerObj;

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
    }

}
