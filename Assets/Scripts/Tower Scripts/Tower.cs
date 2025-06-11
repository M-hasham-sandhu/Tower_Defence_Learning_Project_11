using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerData Data { get; private set; }

    public void Initialize(TowerData towerData)
    {
        Data = towerData;
        // Set up visuals, range indicators, etc. based on Data
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
}