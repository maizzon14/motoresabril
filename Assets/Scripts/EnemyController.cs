using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Scripting.APIUpdating;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] GameObject target;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (target)
        {
            Move();
        }
    }

    void Move()
    {
        agent.destination = target.transform.position;
    }
}
