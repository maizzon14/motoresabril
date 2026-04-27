using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Scripting.APIUpdating;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] GameObject target;

    float curHealth;

    [SerializeField] EnemyData enemyData;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        curHealth = enemyData.GetMaxHealth();
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

    public void GetDamaged(float amount)
    {
        curHealth -= amount;
        if(curHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
