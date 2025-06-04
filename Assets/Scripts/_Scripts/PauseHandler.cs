using UnityEngine;

public class PauseHandler : MonoBehaviour
{
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
        if (state == GameState.Paused)
            Time.timeScale = 0f;
        else if (state == GameState.Playing)
            Time.timeScale = 1f;

        Debug.Log("Time scale set to: " + Time.timeScale);
    }

    public void OnResumeButtonClicked()
    {
        Debug.Log("Resume button clicked, changing game state to Playing.");
        GameManger.Instance.ChangeState(GameState.Playing);
    }

    public void OnPauseButtonClicked()
    {
        Debug.Log("Pause button clicked, changing game state to Paused.");
        GameManger.Instance.ChangeState(GameState.Paused);
    }
}
