using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveController : MonoBehaviour
{
    [Header("Wave Set Data")]
    public WaveSetData waveSetData; // Assign the WaveSetData ScriptableObject in the Inspector

    [Header("UI")]
    public TextMeshProUGUI waveTimerText;
    public TextMeshProUGUI waveInfoText;

    public int CurrentWaveIndex { get; private set; } = -1;
    public bool IsSpawning { get; private set; } = false;

    public event System.Action<int, WaveData> OnWaveStarted;
    public event System.Action<int, WaveData> OnWaveCompleted;

    private float waveDelay = 120f;
    private Coroutine timerCoroutine;

    private void Start()
    {
        UpdateWaveInfoUI();
        timerCoroutine = StartCoroutine(StartNextWaveWithDelay());
    }

    public void StartNextWave()
    {
        Debug.Log($"Starting wave {CurrentWaveIndex + 1}");
        if (IsSpawning) return;
        if (waveSetData == null || waveSetData.waves == null || CurrentWaveIndex + 1 >= waveSetData.waves.Count)
        {
            //GameManger.Instance.ChangeState(GameState.Victory);
            return;
        }

        CurrentWaveIndex++;
        IsSpawning = true;
        UpdateWaveInfoUI();
        WaveData currentWave = waveSetData.waves[CurrentWaveIndex];
        OnWaveStarted?.Invoke(CurrentWaveIndex, currentWave);

        StartCoroutine(SpawnWaveCoroutine(currentWave));
    }

    private IEnumerator StartNextWaveWithDelay()
    {
        float timer = waveDelay;
        while (timer > 0)
        {
            UpdateWaveTimerUI(timer);
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }
        UpdateWaveTimerUI(0);
        StartNextWave();
    }

    private void UpdateWaveTimerUI(float timeLeft)
    {
        if (waveTimerText != null)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            waveTimerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void UpdateWaveInfoUI()
    {
        if (waveInfoText != null && waveSetData != null && waveSetData.waves != null)
        {
            int total = waveSetData.waves.Count;
            int current = Mathf.Clamp(CurrentWaveIndex + 1, 1, total);
            waveInfoText.text = $"Wave {current}/{total}";
        }
    }

    private IEnumerator SpawnWaveCoroutine(WaveData wave)
    {
        // For each wave, randomly select a path for all enemies in this wave
        Transform[] selectedPath = WaypointManager.Instance.GetRandomPath();
        Vector3 spawnPos = Vector3.zero;
        if (selectedPath != null && selectedPath.Length > 0)
            spawnPos = selectedPath[0].position;
        else
            Debug.LogWarning("No waypoints assigned in WaypointManager! Enemies will spawn at (0,0,0).");

        int totalEnemies = 0;
        foreach (var enemyInfo in wave.enemiesToSpawn)
            totalEnemies += enemyInfo.count;
        int enemiesRemaining = totalEnemies;

        void OnEnemyDestroyed()
        {
            enemiesRemaining--;
            if (enemiesRemaining <= 0)
            {
                CompleteCurrentWave();
            }
        }

        List<GameObject> spawnedEnemies = new List<GameObject>();
        foreach (var enemyInfo in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                GameObject enemy = Instantiate(enemyInfo.enemyPrefab, spawnPos, Quaternion.identity, this.transform);
                spawnedEnemies.Add(enemy);
                BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
                if (baseEnemy != null)
                {
                    baseEnemy.SetWaypoints(selectedPath);
                    // Subscribe to OnDestroy event
                    baseEnemy.gameObject.AddComponent<EnemyDeathNotifier>().OnDeath = OnEnemyDestroyed;
                }
                yield return new WaitForSeconds(enemyInfo.spawnDelay);
            }
        }
    }

    // Call this when the wave is finished (all enemies defeated)
    public void CompleteCurrentWave()
    {
        if (!IsSpawning) return;
        WaveData currentWave = waveSetData.waves[CurrentWaveIndex];
        OnWaveCompleted?.Invoke(CurrentWaveIndex, currentWave);
        IsSpawning = false;
        // Optionally, automatically start the next wave or wait for player input
        // Restart timer for next wave if needed
        if (CurrentWaveIndex + 1 < waveSetData.waves.Count)
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(StartNextWaveWithDelay());
        }
    }

    // Optional: Reset for replay or restart
    public void ResetWaves()
    {
        CurrentWaveIndex = -1;
        IsSpawning = false;
        UpdateWaveInfoUI();
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(StartNextWaveWithDelay());
    }

    public class EnemyDeathNotifier : MonoBehaviour
    {
        public System.Action OnDeath;
        private void OnDestroy()
        {
            if (OnDeath != null) OnDeath();
        }
    }
}

