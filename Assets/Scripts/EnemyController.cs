using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Scripting.APIUpdating;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] GameObject target;

    float curHealth;

    [SerializeField] EnemyData enemyData;


    float damage;
    float attackRange;
    float attackSpeed;

    bool canAttack = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        curHealth = enemyData.GetMaxHealth();

        damage = enemyData.GetDamage();
        attackRange = enemyData.GetAttackRange();
        attackSpeed = enemyData.GetAttackSpeed();
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
            Attack();
        }
    }

    void Move()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.destination = target.transform.position;
        }
        else
        {
            agent.isStopped = true;
        }
    }

    void Attack()
    {
        if (!canAttack) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
                StartCoroutine(AttackCooldown());
                break;
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

    public void GetDamaged(float amount)
    {
        curHealth -= amount;
        if (curHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
