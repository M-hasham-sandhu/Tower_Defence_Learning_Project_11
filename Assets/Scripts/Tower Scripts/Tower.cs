using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData Data { get; private set; }
    private List<BaseEnemy> enemiesInRange = new List<BaseEnemy>();

    public void Initialize(TowerData towerData)
    {
        Data = towerData;
        // Set up visuals, range indicators, etc. based on Data

        // Set the range of the SphereCollider to match the tower's range
        var sphere = GetComponent<SphereCollider>();
        if (sphere != null)
            sphere.radius = Data.range;
    }

    // Handles upgrade logic and visual swap
    public GameObject Upgrade()
    {
        if (Data.nextUpgrade == null)
        {
            Debug.Log("Tower is already at max level.");
            return null;
        }

        // Save position and parent
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Transform parent = transform.parent;
        TowerData nextData = Data.nextUpgrade;

        // Destroy current tower and instantiate upgraded one
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
            enemiesInRange.Add(enemy);

        enemy.TakeDamage(Data.damage);
        Debug.Log("Enemy in range: " + enemy.name + ", Damage dealt: " + Data.damage);
    }

    private void OnTriggerExit(Collider other)
    {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null)
            enemiesInRange.Remove(enemy);
    }

}