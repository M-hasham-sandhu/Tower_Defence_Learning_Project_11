using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Tower UI")]
    public GameObject towerPanel; 
    public TextMeshProUGUI towerCostText; 
    private Tower selectedTower;

    [Header("Currency UI")]
    public TextMeshProUGUI goldText; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        towerPanel?.SetActive(false);
    }

    private void Start()
    {
        if (GameManger.Instance != null)
            GameManger.Instance.OnGameStateChanged += HandleGameStateChanged;

        // Subscribe to currency changes
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.GoldChanged += UpdateGoldUI;
            UpdateGoldUI(CurrencyManager.Instance.CurrentGold); // Initial update
        }
    }

    private void OnDisable()
    {
        if (GameManger.Instance != null)
            GameManger.Instance.OnGameStateChanged -= HandleGameStateChanged;

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.GoldChanged -= UpdateGoldUI;
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
                gameOverPanel?.SetActive(true);
                break;
            case GameState.Victory:
                victoryPanel?.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        pausePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        victoryPanel?.SetActive(false);
        towerPanel?.SetActive(false);
    }

    public void ShowTowerPanel(Tower tower)
    {
        selectedTower = tower;
        towerPanel?.SetActive(true);

        if (towerCostText != null && tower.Data != null)
            towerCostText.text = "Cost: " + tower.Data.cost.ToString();
    }

    public void OnUpgradeButton()
    {
        selectedTower?.Upgrade();
        towerPanel?.SetActive(false);
        selectedTower = null;
    }

    public void OnCloseButton()
    {
        towerPanel?.SetActive(false);
        selectedTower = null;
    }

    // --- Currency UI update ---
    private void UpdateGoldUI(int newGold)
    {
        if (goldText != null)
            goldText.text = $"{newGold}";
    }

    public void ShowNotEnoughGoldMessage()
    {
        // You can implement a popup, flash the gold text, etc.
        Debug.LogWarning("Not enough gold!");
        // Example: flash the goldText red
        if (goldText != null)
            StartCoroutine(FlashGoldTextRed());
    }

    private IEnumerator FlashGoldTextRed()
    {
        Color originalColor = goldText.color;
        goldText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        goldText.color = originalColor;
    }
}
