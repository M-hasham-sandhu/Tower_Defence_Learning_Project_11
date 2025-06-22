using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData Data { get; private set; } 
    private List<BaseEnemy> enemiesInRange = new List<BaseEnemy>();
    private Dictionary<BaseEnemy, Coroutine> attackCoroutines = new Dictionary<BaseEnemy, Coroutine>();

    public void Initialize(TowerData towerData)
    {
        Data = towerData;
        var sphere = GetComponent<SphereCollider>();
        if (sphere != null)
            sphere.radius = Data.range;
    }

    public GameObject Upgrade()
    {
        if (Data.nextUpgrade == null)
        {
            Debug.Log("Tower is already at max level.");
            return null;
        }

        TowerData nextData = Data.nextUpgrade;

        // Check if player has enough gold for upgrade
        if (!CurrencyManager.Instance.SpendGold(nextData.cost))
        {
            Debug.Log("Not enough gold to upgrade this tower!");
            UIManager.Instance.ShowNotEnoughGoldMessage(); // Optional: show a warning in UI
            return null;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Transform parent = transform.parent;

        Destroy(gameObject);
        GameObject upgradedTower = Instantiate(nextData.towerPrefab, pos, rot, parent);
        var towerScript = upgradedTower.GetComponent<Tower>();
        if (towerScript != null)
            towerScript.Initialize(nextData);

        Debug.Log("Tower upgraded! New stats: Damage=" + nextData.damage + ", Range=" + nextData.range + ", AttackRate=" + nextData.attackRate);
        return upgradedTower;
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
            Coroutine attackRoutine = StartCoroutine(AttackEnemy(enemy));
            attackCoroutines[enemy] = attackRoutine;
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
        while (enemy != null && enemiesInRange.Contains(enemy))
        {
            enemy.TakeDamage(Data.damage);
            Debug.Log("Attacking enemy: " + enemy.name + ", Damage dealt: " + Data.damage);
            yield return new WaitForSeconds(1f / Data.attackRate);
        }
    }

    public void OnSelected()
    {
        UIManager.Instance.ShowTowerPanel(this);
    }
}