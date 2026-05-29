using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float MaxHealth;
    float CurrentHealth;

    public GameObject tryAgain;
    [SerializeField] Slider healthBar;
    private void Start()
    {
        CurrentHealth = MaxHealth;
        tryAgain.SetActive(false);

        healthBar.maxValue = MaxHealth;
        healthBar.value = CurrentHealth;
    }
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
       
        healthBar.value = CurrentHealth;
        Debug.Log("HP: " + CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("MORITE");
        Time.timeScale = 0;
        tryAgain.SetActive(true);
    }
}
