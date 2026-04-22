using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour, Iweapon
{
    [SerializeField] GunData gunData;

    [SerializeField] GameObject user;
    [SerializeField] Transform barrel;

    int curAmo;

    bool canShoot = true;

    void Awake()
    {
        curAmo = gunData.maxAmo;
    }

    public void Shoot(EnemyController target)
    {
        if (!canShoot)
        {
            return;
        }

        RaycastHit hit;
        Vector3 origin = barrel.position;
        Vector3 direction = user.transform.forward;
        Vector3 endPoint;

        if(Physics.Raycast(origin, direction, out hit, gunData.range))
        {
            endPoint = hit.point;

            if(hit.transform.GetComponent<EnemyController>())
            {
                hit.transform.GetComponent<EnemyController>().GetDamaged(gunData.damage);
            }
        }
        else
        {
            endPoint = origin + direction * gunData.range;
        }

        --curAmo;

        Debug.Log("Amo: " + curAmo);

        if (curAmo == 0) Reload();
        else StartCoroutine(WaitFireRate());
    }

    public void Reload() 
    {
        canShoot = false;
        StartCoroutine(Reloading());
    }
    public float GetRange() { return gunData.range; }
    public void SwitchWeapon() { }

    IEnumerator WaitFireRate()
    {
        canShoot = false;
        yield return new WaitForSeconds(gunData.fireRate);
        canShoot = true;
    }

    IEnumerator Reloading()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(gunData.reloadTime);
        curAmo = gunData.maxAmo;
        canShoot = true;
        Debug.Log("Reloaded");
    }
}
