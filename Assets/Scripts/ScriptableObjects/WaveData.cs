using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/WaveSetData")]
public class WaveSetData : ScriptableObject
{
    public List<WaveData> waves;
}

[System.Serializable]
public class WaveData
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
