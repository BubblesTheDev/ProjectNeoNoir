using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerProjectileBase : MonoBehaviour
{
    public float projectileDamage;
    public float projectileSpeed;
    public float maxShotDistance;
    public int projectilePeirce;

    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool projectileDead;
    public LayerMask layersToIgnore;

    public virtual void loadStats(float damage, float speed, float distance, int peirce) { }
}
