using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

public class EconomySystem : MonoBehaviour
{
    [Header("Components")]
    public TimeManager TimeManager;
    public GameManager GameManager;

    [Header("Constant variables")]
    public int startBalance = 1456536;
    public int balanceChangeTest;

    [Header("Dynamic variables")]
    public bool isInBankruptcy;
    public decimal Balance
    {
        get { return GameSave.current.EconomySaveData.balanace; }
        set 
        {
            if (!GameManager.profile.enableGodMode)
            {
                GameSave.current.EconomySaveData.balanace = value;
                OnBalanceUpdate();
            }
            value = startBalance;
        }
    }
    public DateTime LastBankruptcyStart { get => GameSave.current.EconomySaveData.LastBankruptcyStart; set => GameSave.current.EconomySaveData.LastBankruptcyStart = value; }
    public DateTime LastBankruptcyEnd { get => GameSave.current.EconomySaveData.LastBankruptcyEnd; set => GameSave.current.EconomySaveData.LastBankruptcyEnd = value; }

    public List<TimeWaitEvent> BankruptcyTimers { get => GameSave.current.EconomySaveData.bankruptcyTimers; set => GameSave.current.EconomySaveData.bankruptcyTimers = value; }

    public int SeriousBankruptcyID;
    public int GameOverID;

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
        if (!GameManager.profile.enableBankruptcy)
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
        LastBankruptcyStart = TimeManager.CurrentTime;

        BankruptcyTimers.Add(TimeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.profile.daysBeforeSeriousBankruptcy), SeriousBankruptcyID));
    }

    public virtual void OnSeriousBankruptcy()
    {
        BankruptcyTimers.Add(TimeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.profile.daysBeforeGameOverBankruptcy), GameOverID));
    }

    public virtual void OnEndBankruptcy()
    {
        isInBankruptcy = false;
        LastBankruptcyEnd = TimeManager.CurrentTime;

        foreach (TimeWaitEvent ev in BankruptcyTimers) { TimeManager.UnregisterTimeWaiter(ev); }
    }

    public string GetReadableBalance()
    {
        string after = "Balance: " + Balance.ToString("C", GameManager.currentCultureCurrency);
        return after;
    }
}
