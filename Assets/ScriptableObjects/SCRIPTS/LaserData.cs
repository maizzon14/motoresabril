using UnityEngine;

[CreateAssetMenu(fileName = "LaserData", menuName = "Scriptable Objects/LaserData")]
public class LaserData : ScriptableObject
{
    public int Damage;
    public float ReloadTime;
    public float Range;
    public float Radius;
    public float FireRating;
    public int MaxAmo;
    public int AmoCost;
}
