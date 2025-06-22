using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    protected float currentHealth;
    private float damage = 5f;

    protected Transform[] waypoints;
    protected int currentWaypoint = 0;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        waypoints = WaypointManager.Instance.waypoints;
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
        CurrencyManager.Instance.AddGold(10); // Reward player for kill
        Destroy(gameObject);
    }

    protected virtual void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypoint < waypoints.Length)
        {
            Vector3 target = waypoints[currentWaypoint].position;
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target) < 0.1f)
                currentWaypoint++;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual float Attack()
    {
       return damage;
    }
}