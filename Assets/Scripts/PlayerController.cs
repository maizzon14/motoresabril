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

    InputAction[] m_switchWeaponActions = new InputAction[4];

    [SerializeField] GameObject[] Weapons = new GameObject[4];
    Iweapon currentWeapon;

    int currentWeaponNumber;

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

        m_switchWeaponActions = new InputAction[4] { input.Main.Weapon1, input.Main.Weapon2, input.Main.Weapon3, input.Main.Weapon4 };
    }

    void Start()
    {
        for(int i = 0;  i < Weapons.Length; i++)
        {
            if(i == 0)
            {
                currentWeapon = Weapons[i].GetComponent<Iweapon>();
                currentWeaponNumber = i;
            }
            else
            {
                Weapons[i].SetActive(false);
            }
        }



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
                if (target.Hit.transform == null)
                {
                    target = new Target(TargetType.none, new RaycastHit());
                    return;
                }
                agent.destination = target.Hit.transform.position;

                float distance = Vector3.Distance(transform.position, agent.destination);
                if (distance <= currentWeapon.GetRange())
                {
                    agent.isStopped = true;
                    transform.LookAt(agent.destination);
                    currentWeapon.Shoot(target.Hit.transform.GetComponent<EnemyController>());
                }
                break;
            case TargetType.position:
            default:
                break;
        }

        for(int i = 0; i < m_switchWeaponActions.Length; i++)
        {
            if (i >= Weapons.Length) return;

            if (m_switchWeaponActions[i].WasPressedThisFrame())
            {
                if (Weapons[i] && currentWeapon != Weapons[i].GetComponent<Iweapon>())
                {
                    Weapons[currentWeaponNumber].SetActive(false);
                    Weapons[i].SetActive(true);
                    currentWeapon = Weapons[i].GetComponent<Iweapon>();
                    currentWeapon.SwitchWeapon();
                    currentWeaponNumber = i;
                    break;
                }
            }
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
                case "Interactable":
                    break;
                default:
                    Debug.Log("???");
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

        Gizmos.color = Color.red;
        if(currentWeapon != null) Gizmos.DrawWireSphere(transform.position, currentWeapon.GetRange());
    }
    
    /*void Shoot()
    {
        canShoot = false;
        Debug.Log("Bang");
        Debug.DrawLine(transform.position, target.Hit.transform.position,
            Color.yellow, 0.1f);
        StartCoroutine(ShootCooldown());
    }*/ 
    /*IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        canShoot = true;
    }*/
}
