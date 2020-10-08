﻿using System;
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
    public int daysBeforeSeriousBankruptcy = 5;
    public int daysBeforeGameOverBankruptcy = 7;

    [Header("Dynamic variables")]
    public bool isInBankruptcy;
    public decimal Balance
    {
        get { return GameSave.current.EconomySaveData.balanace; }
        set 
        {
            GameSave.current.EconomySaveData.balanace = value;
            OnBalanceUpdate();
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
        if (!GameManager.profile.enableBankruptcySystem)
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

        BankruptcyTimers.Add(TimeManager.RegisterTimeWaiter(TimeSpan.FromDays(daysBeforeSeriousBankruptcy), SeriousBankruptcyID));
    }

    public virtual void OnSeriousBankruptcy()
    {
        BankruptcyTimers.Add(TimeManager.RegisterTimeWaiter(TimeSpan.FromDays(daysBeforeGameOverBankruptcy), GameOverID));
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

    public bool IsCreditSufficient(decimal priceToCheck) 
    {
        if (Balance - Math.Abs(priceToCheck) >= 0) // check if the difference is higher or equal to 0
            return true;
        else if (Balance <= 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Updates the balanace in a positive or negative way.
    /// </summary>
    /// <param name="valueToApply">The price to apply to the balance. Can be either positive or negative.</param>
    /// <returns>Returns whether the transaction succeeded.</returns>
    public bool UpdateBalance(decimal valueToApply)
    {
        if (valueToApply == 0)
        {
            Debug.Log("ES: Processed an empty transaction");
            return true;
        }

        if (valueToApply < 0 && !IsCreditSufficient(valueToApply))
        {
            Debug.LogWarning("ES: Failed to process a transaction - insufficient funds! Balanace " + Balance + ", price " + valueToApply);
            return false;
        }

        decimal balanaceChange = Balance + valueToApply;

        if (balanaceChange <= 0 && !GameManager.instance.Profile.enableBankruptcySystem)
        {
            balanaceChange = 0;
        }

        Debug.Log("ES: Transaction succeeded! New Balanace " + balanaceChange + ", price " + valueToApply + ", old balance " + Balance);
        Balance = balanaceChange;
        return true;
    }
}
