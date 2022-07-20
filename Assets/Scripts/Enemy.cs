using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Enemy : LivingEntity
{
    UnityEngine.AI.NavMeshAgent pathfinder;
    public enum State { Idle, Chasing, Attacking };
    State currentState;
    Transform target;
    Material skinMaterial;
    Color originalColour;
    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        skinMaterial = GetComponent<Renderer>().material;
        originalColour = skinMaterial.color;
        currentState = State.Chasing;
        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        InvokeRepeating("UpdatePath", 0, 0.25f);
    }

    void Update()
    {
        if (Time.time > nextAttackTime)
        {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            }

        }
    }

    IEnumerator Attack()
    {

        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;

        while (percent <= 1)
        {

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColour;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    void UpdatePath()
    {
        if (currentState == State.Chasing)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
            if (!dead)
            {
                pathfinder.SetDestination(targetPosition);
            }
        }
    }

}


