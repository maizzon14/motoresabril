using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour, Iweapon
{
    [SerializeField] GunData gunData;
    [SerializeField] TrailData trail;
    [SerializeField] GameObject user;
    [SerializeField] Transform barrel;

    float Damage;
    float ReloadTime;
    float Range;
    float FireRating;

    int MaxAmo;
    int curAmo;

    bool canShoot = true;

    void Awake()
    {
        Damage = gunData.damage;
        ReloadTime = gunData.reloadTime;
        Range = gunData.range;
        FireRating = gunData.fireRate;
        MaxAmo = gunData.maxAmo;
        
        curAmo = MaxAmo;
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

        if(Physics.Raycast(origin, direction, out hit, Range))
        {
            endPoint = hit.point;

            if(hit.transform.GetComponent<EnemyController>())
            {
                hit.transform.GetComponent<EnemyController>().GetDamaged(Damage);
            }
        }
        else
        {
            endPoint = origin + direction * Range;
        }

        StartCoroutine(trail.PlayTrail(origin, endPoint));

        --curAmo;

        Debug.Log("Amo: " + curAmo);

        if (curAmo == 0) Reload();
        else StartCoroutine(WaitFireRate());
    }

    public void Reload() 
    {
        StartCoroutine(Reloading());
    }
    public float GetRange() { return Range; }
    public void SwitchWeapon() 
    {
        Reload();
    }

    IEnumerator WaitFireRate()
    {
        canShoot = false;
        yield return new WaitForSeconds(FireRating);
        canShoot = true;
    }

    IEnumerator Reloading()
    {
        Debug.Log("Reloading...");
        canShoot = false;
        yield return new WaitForSeconds(ReloadTime);
        curAmo = MaxAmo;
        canShoot = true;
        Debug.Log("Reloaded");
    }
}
