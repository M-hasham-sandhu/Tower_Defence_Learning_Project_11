using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    protected float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Play death effects, add score, etc.
        Destroy(gameObject);
    }

    // Example: Move along a path (override for custom movement)
    protected virtual void Update()
    {
        // Implement movement logic here or in a derived class
    }
}