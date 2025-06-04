using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/WaveData")]
public class WaveData : ScriptableObject
{
    public List<EnemySpawnInfo> enemiesToSpawn;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnDelay;
}
