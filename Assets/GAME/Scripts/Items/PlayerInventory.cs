using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [SerializeField] private int smallKeys = 0;
    [SerializeField] private int coins = 0;
    public bool HasBossKey { get; private set; }

    public Action OnKeyChange;
    public Action OnCoinChange;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddKey()
    {
        smallKeys++;
        OnKeyChange?.Invoke();
    }

    public void RemoveKey()
    {
        smallKeys--;
        OnKeyChange?.Invoke();
    }

    public void AddCoins(int newCoins)
    {
        coins += newCoins;
        OnKeyChange?.Invoke();
    }

    public void RemoveCoins(int newCoins)
    {
        coins -= newCoins;
        OnKeyChange?.Invoke();
    }

    public void AddBossKey() { HasBossKey = true; }

    public bool HasKey() { return smallKeys > 0; }
    public bool HasEnoughCoins(int requiredCoins) { return coins >= requiredCoins; }
}
