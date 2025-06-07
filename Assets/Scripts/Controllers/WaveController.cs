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
        float xPos = 0f;
        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                Vector3 spawnPos = new Vector3(xPos, 2, 0);
                GameObject enemy = Instantiate(enemyInfo.enemyPrefab, spawnPos, Quaternion.identity, this.transform);
                enemy.transform.localScale = new Vector3(50f, 50f, 50f);
                xPos += 5f;
                yield return new WaitForSeconds(enemyInfo.spawnDelay);
            }
        }
        // Optionally, call CompleteCurrentWave() here if you want to auto-complete after spawning
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

