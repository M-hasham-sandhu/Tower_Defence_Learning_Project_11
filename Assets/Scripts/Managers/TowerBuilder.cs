using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [Header("All Tower Data (assign in Inspector)")]
    public List<TowerData> allTowers;

    private Dictionary<(TowerType, int), TowerData> towerLookup;

    // For testing
    private GameObject lastBuiltTower;
    public Transform testSpawnParent;
    public Vector3 testSpawnPosition = Vector3.zero;

    private void Awake()
    {
        BuildTowerLookup();
    }

    private void BuildTowerLookup()
    {
        towerLookup = new Dictionary<(TowerType, int), TowerData>();
        foreach (var baseData in allTowers)
        {
            var data = baseData;
            int stage = 1;
            while (data != null)
            {
                var key = (data.towerType, stage);
                if (!towerLookup.ContainsKey(key))
                    towerLookup.Add(key, data);
                data = data.nextUpgrade;
                stage++;
            }
        }
    }

    // Build a tower by type and stage (stage starts at 1)
    public GameObject BuildTower(TowerType type, int stage, Vector3 position, Transform parent = null)
    {
        TowerData data = GetTowerData(type, stage);
        if (data == null)
        {
            Debug.LogWarning($"No TowerData found for {type} at stage {stage}!");
            return null;
        }

        GameObject tower = Instantiate(data.towerPrefab, position, Quaternion.identity, parent);
        // Optionally, assign the TowerData to a Tower script on the prefab
        var towerScript = tower.GetComponent<Tower>();
        if (towerScript != null)
            towerScript.Initialize(data);

        return tower;
    }

    // Fast lookup using the dictionary
    public TowerData GetTowerData(TowerType type, int stage)
    {
        towerLookup.TryGetValue((type, stage), out TowerData data);
        return data;
    }

    // Call this from a UI button to build the first stage of a tower
  public void BuildTestTower()
{
    // Always spawn at (0, 0, 0) for testing
    lastBuiltTower = BuildTower(TowerType.Type_1, 1, Vector3.zero, testSpawnParent);
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
            bool upgraded = tower.Upgrade();
            if (!upgraded)
            {
                Debug.Log("Tower is already at max level.");
            }
            // Optionally, update lastBuiltTower reference if you want to keep track of the new instance:
            // lastBuiltTower = ... (find the new instance if needed)
        }
    }
}