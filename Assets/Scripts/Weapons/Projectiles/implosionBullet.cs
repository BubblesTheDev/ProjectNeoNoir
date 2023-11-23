using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.VFX;

public class implosionBullet : MonoBehaviour
{
    [Header("Bullet Variables")]
    public float velocity = 250f;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private float collisionForwardOffset;
    [SerializeField] private float collisionDistance;
    private bool isDead = false;

    [Space, Header("Implosion Variables")]
    public float implosionRange;
    public float damage;
    public float implosionForce;

    [Space, Header("Player Pull Settings")]
    [SerializeField] private float pullForce;
    [SerializeField] private float maxDistance;
    [SerializeField] private float stoppingDistance;


    [Space, Header("VFX Variables")]
    public VisualEffect bulletEffect;
    [SerializeField] private LineRenderer gravityChain;


    InteractionInputActions inputActions;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InteractionInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void FixedUpdate()
    {
        if (!isDead) rb.velocity = transform.forward * velocity * Time.deltaTime;
        else rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (inputActions.Combat.Fire2.WasPressedThisFrame() && !isDead && Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < maxDistance)
        {
            StartCoroutine(pullPlayer());
            implosion();
        }
    }

    private void LateUpdate()
    {
        if (isDead) Destroy(gameObject, 5f);
    }

    void implosion()
    {
        isDead = true;

        List<Collider> enemiesHit = Physics.OverlapSphere(transform.position, implosionRange, layersToHit).ToList();

        for (int i = 0; i < enemiesHit.Count-1; i++)
        {
            if (enemiesHit[i].CompareTag("Enemy"))
            {
                enemiesHit[i].GetComponent<enemyStats>().takeDamage(damage);
                //enemiesHit[i].GetComponent<enemyStats>().ragdollEnemy();
                if (enemiesHit[i].GetComponent<Rigidbody>()) enemiesHit[i].GetComponent<Rigidbody>().AddExplosionForce(implosionForce, transform.position, implosionRange, 0.25f, ForceMode.Impulse);
            }

        }

        
    }

    IEnumerator pullPlayer()
    {

        GameObject player = GameObject.Find("Player");
        GameObject orientation = GameObject.Find("Orientation");
        while (Vector3.Distance(player.transform.position, transform.position) > stoppingDistance)
        {
            player.GetComponent<Rigidbody>().AddForce((transform.position - player.transform.position).normalized * pullForce, ForceMode.Force);
            gravityChain.positionCount = 2;
            gravityChain.SetPosition(0, transform.position);
            gravityChain.SetPosition(1, orientation.transform.position + (-Vector3.up * 0.5f));
            gravity.gravityReference.useGravity = false;
            yield return null;
        }
        gravity.gravityReference.useGravity = true;
        isDead = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(!isDead) implosion();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position + transform.forward * collisionForwardOffset, transform.position + -transform.forward * collisionDistance);

    }
}
