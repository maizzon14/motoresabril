using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
    public float damage;
    public float range;
    public int maxAmo;
    public float reloadTime;
    public float fireRate;
}
