using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCastle : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float damagePerSecond = 5f;

    // Track enemies and their coroutines
    private Dictionary<BaseEnemy, Coroutine> damagingEnemies = new Dictionary<BaseEnemy, Coroutine>();

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null && !damagingEnemies.ContainsKey(enemy))
        {
            Coroutine routine = StartCoroutine(DamageEnemyOverTime(enemy));
            damagingEnemies.Add(enemy, routine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null && damagingEnemies.ContainsKey(enemy))
        {
            StopCoroutine(damagingEnemies[enemy]);
            damagingEnemies.Remove(enemy);
        }
    }

    private IEnumerator DamageEnemyOverTime(BaseEnemy enemy)
    {
        while (enemy != null)
        {
            enemy.TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(1f);
        }
        // Clean up if enemy is destroyed
        if (damagingEnemies.ContainsKey(enemy))
            damagingEnemies.Remove(enemy);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Castle took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Castle destroyed! Game Over.");
        Destroy(gameObject);
    }
}