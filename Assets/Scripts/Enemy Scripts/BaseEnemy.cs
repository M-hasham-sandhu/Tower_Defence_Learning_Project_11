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

    private Animator animator;

    public float CurrentHealth => currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        // Waypoints are now always assigned via SetWaypoints by the spawner (WaveController).
        // No need to assign or warn here.
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (animator != null)
        {
            animator.SetTrigger("damage");
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("die");
        }
        CurrencyManager.Instance.AddGold(10);
        Destroy(gameObject);
    }

    protected virtual void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"[BaseEnemy] No waypoints in Update for {gameObject.name}.");
            return;
        }

        if (currentWaypoint < waypoints.Length)
        {
            if (waypoints[currentWaypoint] == null)
            {
                Debug.LogWarning($"[BaseEnemy] Waypoint {currentWaypoint} is null for {gameObject.name}.");
                currentWaypoint++;
                return;
            }
            Vector3 target = waypoints[currentWaypoint].position;
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            {
                animator.SetTrigger("move");
            }

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                Debug.Log($"[BaseEnemy] {gameObject.name} reached waypoint {currentWaypoint}.");
                currentWaypoint++;
            }
        }
        else
        {
            Debug.Log($"[BaseEnemy] {gameObject.name} finished all waypoints and will be destroyed.");
            if (animator != null)
            {
                animator.SetTrigger("die");
            }
            Destroy(gameObject);
        }
    }

    public virtual float Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }
        return damage;
    }

    public void SetHealth(float value)
    {
        currentHealth = value;
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypoint = 0;
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"[BaseEnemy] SetWaypoints called with null or empty array for {gameObject.name}.");
        }
        else
        {
            Debug.Log($"[BaseEnemy] {gameObject.name} received {waypoints.Length} waypoints via SetWaypoints. First: {(waypoints[0] != null ? waypoints[0].name : "null")}");
        }
    }
}