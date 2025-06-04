using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    private void Start()
    {
        if (GameManger.Instance != null)
            GameManger.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        if (GameManger.Instance != null)
            GameManger.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        HideAllPanels();

        switch (state)
        {
            case GameState.Paused:
                if (pausePanel != null)
                {
                    Debug.Log("Game is paused, showing pause panel.");
                    pausePanel.SetActive(true);
                }
                else
                {
                    Debug.Log("Pause panel is not assigned in UIManager.");
                }
                break;
            case GameState.GameOver:
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                break;
            case GameState.Victory:
                if (victoryPanel != null) victoryPanel.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }
}
