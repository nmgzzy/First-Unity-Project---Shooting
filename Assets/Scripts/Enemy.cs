using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent pathfinder;
    Transform target;

    void Start()
    {
        pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        // StartCoroutine(UpdatePath());
        InvokeRepeating("UpdatePath", 0, 0.25f);

    }

    // void Update()
    // {
    // }
    void UpdatePath()
    {
        Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
        pathfinder.SetDestination(target.position);
    }
    // IEnumerator UpdatePath()
    // {
    //     float refreshTime = 0.25f;
    //     while (target != null)
    //     {
    //         Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
    //         pathfinder.SetDestination(target.position);
    //         yield return new WaitForSeconds(refreshTime);

    //     }
    // }
}


