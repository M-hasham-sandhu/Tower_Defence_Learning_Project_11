using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [Header("All Tower Data (assign in Inspector)")]
    public List<TowerData> allTowers;

    // For testing
    private GameObject lastBuiltTower;
    public Transform testSpawnParent;
    public Vector3 testSpawnPosition = Vector3.zero;
    private GameObject previewTower;

    private void Awake()
    {
        
    }

    // Build a tower by type and level (level starts at 0)
    public GameObject BuildTower(TowerType type, int level, Vector3 position, Transform parent = null)
    {
        TowerData data = allTowers.Find(t => t.towerType == type);
        if (data == null)
        {
            Debug.LogWarning($"No TowerData found for {type}!");
            return null;
        }
        if (level < 0 || level >= data.levels.Count)
        {
            Debug.LogWarning($"No level {level} for tower type {type}!");
            return null;
        }
        var stats = data.levels[level];
        // Check if player has enough gold
        if (!CurrencyManager.Instance.SpendGold(stats.cost))
        {
            Debug.LogWarning("Not enough gold to build this tower!");
            return null;
        }
        Debug.Log($"[TowerBuilder] Attempting to build {type} at level {level}. Cost: {stats.cost}, Current Gold: {CurrencyManager.Instance.CurrentGold}");
        GameObject tower = Instantiate(stats.towerPrefab, position, Quaternion.identity, parent);
        var towerScript = tower.GetComponent<Tower>();
        if (towerScript != null)
            towerScript.Initialize(data, level);
        return tower;
    }

    // Call this from a UI button to build the first level of a tower
    public void BuildTestTower()
    {
        lastBuiltTower = BuildTower(TowerType.Type_1, 0, Vector3.zero, testSpawnParent);
        if (lastBuiltTower != null)
        {
            Debug.Log("Built tower: " + lastBuiltTower.GetComponent<Tower>().Data.name);
        }
    }

    // Call this from a UI button to upgrade the last built tower
    public void UpgradeTestTower()
    {
        if (lastBuiltTower == null)
        {
            Debug.LogWarning("No tower to upgrade!");
            return;
        }
        var tower = lastBuiltTower.GetComponent<Tower>();
        if (tower != null)
        {
            GameObject upgraded = tower.Upgrade();
            if (upgraded != null)
            {
                lastBuiltTower = upgraded;
            }
            else
            {
                Debug.Log("Tower is already at max level.");
            }
        }
    }

    // Call this to preview a tower's first level prefab
    public void PreviewTower(TowerType type)
    {
        // Destroy existing preview tower if any
        if (previewTower != null)
            Destroy(previewTower);
        previewTower = GetPreviewTower(type);
    }

    public GameObject GetPreviewTower(TowerType type)
    {
        var data = allTowers.Find(t => t.towerType == type);
        if (data != null && data.levels.Count > 0 && data.levels[0].towerPrefab != null)
            return Instantiate(data.levels[0].towerPrefab, Vector3.zero, Quaternion.identity);
        else
            Debug.LogWarning("Preview tower NOT instantiated! Check your prefab and TowerData.");
        return null;
    }
}