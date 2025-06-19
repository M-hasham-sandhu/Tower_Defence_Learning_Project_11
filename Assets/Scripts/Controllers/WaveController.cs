using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Wave Data")]
    public List<WaveData> waves; // Assign ScriptableObjects for each wave in the Inspector

    public int CurrentWaveIndex { get; private set; } = -1;
    public bool IsSpawning { get; private set; } = false;

    public event System.Action<int, WaveData> OnWaveStarted;
    public event System.Action<int, WaveData> OnWaveCompleted;

    private void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        Debug.Log($"Starting wave {CurrentWaveIndex + 1}");
        if (IsSpawning) return;
        if (CurrentWaveIndex + 1 >= waves.Count)
        {
            //GameManger.Instance.ChangeState(GameState.Victory);
            return;
        }

        CurrentWaveIndex++;
        IsSpawning = true;
        WaveData currentWave = waves[CurrentWaveIndex];
        OnWaveStarted?.Invoke(CurrentWaveIndex, currentWave);

        StartCoroutine(SpawnWaveCoroutine(currentWave));
    }

    private IEnumerator SpawnWaveCoroutine(WaveData wave)
    {
        // Get the first waypoint position from WaypointManager
        Transform[] waypoints = WaypointManager.Instance.waypoints;
        Vector3 spawnPos = Vector3.zero;
        if (waypoints != null && waypoints.Length > 0)
            spawnPos = waypoints[0].position;
        else
            Debug.LogWarning("No waypoints assigned in WaypointManager! Enemies will spawn at (0,0,0).");

        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                GameObject enemy = Instantiate(enemyInfo.enemyPrefab, spawnPos, Quaternion.identity, this.transform);
                // Optionally set scale or other properties here
                yield return new WaitForSeconds(enemyInfo.spawnDelay);
            }
        }
        // CompleteCurrentWave();
    }

    // Call this when the wave is finished (all enemies defeated)
    public void CompleteCurrentWave()
    {
        if (!IsSpawning) return;
        WaveData currentWave = waves[CurrentWaveIndex];
        OnWaveCompleted?.Invoke(CurrentWaveIndex, currentWave);
        IsSpawning = false;
        // Optionally, automatically start the next wave or wait for player input
    }

    // Optional: Reset for replay or restart
    public void ResetWaves()
    {
        CurrentWaveIndex = -1;
        IsSpawning = false;
    }
}

