using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int StartingGold = 500;
    private int currentGold;

    public int CurrentGold => currentGold;

    public delegate void OnGoldChanged(int newGold);
    public event OnGoldChanged GoldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentGold = StartingGold;
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            GoldChanged?.Invoke(currentGold);
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        GoldChanged?.Invoke(currentGold);
    }
}
