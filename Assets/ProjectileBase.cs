using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float damage;

    public void Initialize(Transform target, float speed, float damage)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 dir = (target.position - transform.position).normalized;
        float step = speed * Time.deltaTime;
        transform.position += dir * step;
        // Optional: face the projectile towards the target
        // transform.LookAt(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null && other.transform == target)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
