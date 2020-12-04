using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Handles the calculations for the economy calculations.
/// </summary>
public class EconomySystem : MonoBehaviour
{
    [Header("Components")]
    public TimeManager timeManager;

    [Header("Constant variables")]
    public int startBalance = 1456536;
    public int balanceChangeTest;

    [Header("Dynamic variables")]
    public bool isInBankruptcy;
    public decimal Balance
    {
        get { return GameSave.current.economySaveData.balanace; }
        set
        {
            if (!GameManager.Profile.enableGodMode)
            {
                GameSave.current.economySaveData.balanace = value;
                OnBalanceUpdate();
            }
        }
    }
    public DateTime lastBankruptcyStart { get => GameSave.current.economySaveData.lastBankruptcyStart; set => GameSave.current.economySaveData.lastBankruptcyStart = value; }
    public DateTime lastBankruptcyEnd { get => GameSave.current.economySaveData.lastBankruptcyEnd; set => GameSave.current.economySaveData.lastBankruptcyEnd = value; }

    public List<TimeWaitEvent> bankruptcyTimers { get => GameSave.current.economySaveData.bankruptcyTimers; set => GameSave.current.economySaveData.bankruptcyTimers = value; }

    public int seriousBankruptcyID;
    public int gameOverID;

    public virtual void OnBalanceUpdate() { MakeBankruptcyCheck(); }

    public bool CheckForSufficientFunds(int price)
    {
        if (Balance >= price)
            return true;
        else
            return false;
    }

    private void MakeBankruptcyCheck()
    {
        if (!GameManager.Profile.enableBankruptcy)
            return;

        if (Balance < 0 && !isInBankruptcy)
        {
            OnEnterBankruptcy();
            return;
        }
        else if (Balance >= 0 && isInBankruptcy)
        {
            OnEndBankruptcy();
            return;
        }
    }

    public virtual void OnEnterBankruptcy()
    {
        isInBankruptcy = true;
        lastBankruptcyStart = timeManager.CurrentTime;

        bankruptcyTimers.Add(timeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.Profile.daysBeforeSeriousBankruptcy), seriousBankruptcyID));
    }

    public virtual void OnSeriousBankruptcy()
    {
        bankruptcyTimers.Add(timeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.Profile.daysBeforeGameOverBankruptcy), gameOverID));
    }

    public virtual void OnEndBankruptcy()
    {
        isInBankruptcy = false;
        lastBankruptcyEnd = timeManager.CurrentTime;

        foreach (TimeWaitEvent ev in bankruptcyTimers) { timeManager.UnregisterTimeWaiter(ev); }
    }

    public string GetReadableBalance()
    {
        return "Balance: " + Balance.ToString("C", GameManager.Instance.currentCultureCurrency);
    }
}
