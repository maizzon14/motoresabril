using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] float MaxHealth;
    [SerializeField] float Damage;
    [SerializeField] float AttackRange;
    [SerializeField] float AttackSpeed;


    public float GetMaxHealth() { return MaxHealth; }
    public float GetDamage() { return Damage; }
    public float GetAttackRange() { return AttackRange; }
    public float GetAttackSpeed() { return AttackSpeed; }
}
