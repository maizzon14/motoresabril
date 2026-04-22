using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;
using UnityEngine.Scripting.APIUpdating;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;

    InputReal input;
    InputAction m_interactAction;

    Vector3 MousePosition = new Vector3();

    public enum TargetType
    { none, enemy, position }

    public struct Target
    {
        public Target ( TargetType type, RaycastHit hit )
        {
            Type = type;
            Hit = hit;
        }
        public TargetType Type;
        public RaycastHit Hit;
    }

    [SerializeField] LayerMask mask;

    Target target = new Target(TargetType.none, new RaycastHit());

    bool canShoot = true;

    [SerializeField] float AttackRange = 5f;
    [SerializeField] float Cooldown = 0.5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        input = new InputReal();
        input.Main.Enable();
        m_interactAction = input.Main.Interact;
    }

    void Update()
    {
        if(m_interactAction.WasPressedThisFrame())
        {
            Move();
        }

        switch(target.Type)
        {
            case TargetType.enemy:
                agent.destination = target.Hit.transform.position;
                float distance = Vector3.Distance(transform.position, agent.destination);
                if (distance <= AttackRange & canShoot)
                {
                    canShoot = false;
                    agent.isStopped = true;
                    transform.LookAt(agent.destination);
                    Shoot();
                }
                break;
            case TargetType.position:
            default:
                break;
        }
    }

    void Move()
    {
        MousePosition = Mouse.current.position.value;

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition),
            out hit, 100, mask))
        {
            string LayerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
            switch(LayerName)
            {
                case "Enemy":
                    target = new Target(TargetType.enemy, hit);
                    break;
                case "Walkable":
                    target = new Target(TargetType.position, hit);
                    break;
                default:
                    break;
            }

            agent.destination = target.Hit.point;
            agent.isStopped = false;
        }
    }

    void OnDrawGizmos()
    {
        if(target.Type != TargetType.none)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 1f);
            Gizmos.DrawWireSphere(agent.destination, 0.5f);
        }

        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
    
    void Shoot()
    {
        canShoot = false;
        Debug.Log("Bang");
        Debug.DrawLine(transform.position, target.Hit.transform.position,
            Color.yellow, 0.1f);
        StartCoroutine(ShootCooldown());
    }
    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        canShoot = true;
    }
}
