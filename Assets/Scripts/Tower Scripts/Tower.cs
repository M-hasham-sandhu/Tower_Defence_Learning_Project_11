using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData Data { get; private set; }
    public int Level { get; private set; } = 0;
    private List<BaseEnemy> enemiesInRange = new List<BaseEnemy>();
    private Dictionary<BaseEnemy, Coroutine> attackCoroutines = new Dictionary<BaseEnemy, Coroutine>();

    public void Initialize(TowerData towerData, int level = 0)
    {
        Data = towerData;
        Level = level;
        var sphere = GetComponent<SphereCollider>();
        if (sphere != null)
            sphere.radius = Data.levels[Level].range;
    }

    public bool CanUpgrade => Level + 1 < Data.levels.Count;

    public GameObject Upgrade()
    {
        if (!CanUpgrade)
        {
            Debug.Log("Tower is already at max level.");
            return null;
        }

        TowerLevelStats nextStats = Data.levels[Level + 1];
        // Check if player has enough gold for upgrade
        if (!CurrencyManager.Instance.SpendGold(nextStats.cost))
        {
            Debug.Log("Not enough gold to upgrade this tower!");
            UIManager.Instance.ShowNotEnoughGoldMessage();
            return null;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Transform parent = transform.parent;

        Destroy(gameObject);
        GameObject upgradedTower = Instantiate(nextStats.towerPrefab, pos, rot, parent);
        var towerScript = upgradedTower.GetComponent<Tower>();
        if (towerScript != null)
            towerScript.Initialize(Data, Level + 1);

        Debug.Log($"Tower upgraded! New stats: Damage={nextStats.damage}, Range={nextStats.range}, AttackRate={nextStats.attackRate}");
        return upgradedTower;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only allow enemies to be tracked if they are not already dead/destroyed
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy) && enemy.gameObject.activeInHierarchy)
        {
            enemiesInRange.Add(enemy);
            // Prevent duplicate coroutines for the same enemy
            if (!attackCoroutines.ContainsKey(enemy))
            {
                Coroutine attackRoutine = StartCoroutine(AttackEnemy(enemy));
                attackCoroutines[enemy] = attackRoutine;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null && enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
            if (attackCoroutines.ContainsKey(enemy))
            {
                StopCoroutine(attackCoroutines[enemy]);
                attackCoroutines.Remove(enemy);
            }
        }
    }

    private IEnumerator AttackEnemy(BaseEnemy enemy)
    {
        while (enemy != null && enemiesInRange.Contains(enemy) && enemy.gameObject.activeInHierarchy)
        {
            var stats = Data.levels[Level];
            if (stats.usesProjectile && stats.projectilePrefab != null)
            {
                GameObject projObj = Instantiate(stats.projectilePrefab, transform.position, Quaternion.identity);
                ProjectileBase proj = projObj.GetComponent<ProjectileBase>();
                if (proj != null)
                {
                    proj.Initialize(enemy.transform, stats.projectileSpeed, stats.damage);
                }
            }
            else
            {
                enemy.TakeDamage(stats.damage);
                Debug.Log($"Attacking enemy: {enemy.name}, Damage dealt: {stats.damage}");
            }
            yield return new WaitForSeconds(1f / stats.attackRate);
        }
        // Clean up coroutine reference if enemy is gone
        if (attackCoroutines.ContainsKey(enemy))
            attackCoroutines.Remove(enemy);
    }

    public void OnSelected()
    {
        UIManager.Instance.ShowTowerPanel(this);
    }
}