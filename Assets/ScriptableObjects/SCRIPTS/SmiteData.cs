using UnityEngine;

[CreateAssetMenu(fileName = "SmiteData", menuName = "Scriptable Objects/SmiteData")]
public class SmiteData : ScriptableObject
{
    public int Damage;
    public float ReloadingTime;
    public float Range;
    public float AreaRadius;
}
