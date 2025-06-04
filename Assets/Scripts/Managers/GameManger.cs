using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
    Victory
}

public class GameManger : MonoBehaviour
{
    public static GameManger Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log($"Changing game state from {CurrentState} to {newState}");
        if (CurrentState == newState) return;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(CurrentState);
    }
}
