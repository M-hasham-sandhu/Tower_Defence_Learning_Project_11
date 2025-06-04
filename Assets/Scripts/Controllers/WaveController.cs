using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Wave Data")]
    public List<WaveData> waves; // Assign ScriptableObjects for each wave in the Inspector

    public int CurrentWaveIndex { get; private set; } = -1;
    public bool IsSpawning { get; private set; } = false;

    public event Action<int, WaveData> OnWaveStarted;
    public event Action<int, WaveData> OnWaveCompleted;

    private void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
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

        // Example: Iterate through enemiesToSpawn for this wave
        // (Actual spawning logic will be implemented elsewhere)
        foreach (var enemyInfo in currentWave.enemiesToSpawn)
        {
            // You can use enemyInfo.enemyPrefab, enemyInfo.count, enemyInfo.spawnDelay
            // to spawn enemies as needed
        }
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

