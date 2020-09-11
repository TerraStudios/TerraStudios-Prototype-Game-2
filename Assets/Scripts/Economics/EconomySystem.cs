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
    public int daysBeforeSeriousBankruptcy = 5;
    public int daysBeforeGameOverBankruptcy = 7;

    [Header("Dynamic variables")]
    public bool isInBankruptcy;
    public decimal _balance { get; private set; } // writable only inside this class
    public decimal Balance
    {
        get { return _balance; }
        set 
        {
            _balance = value;
            OnBalanceUpdate();
        }
    }

    public DateTime LastBankruptcyStart;
    public DateTime LastBankruptcyEnd;

    [HideInInspector] public CultureInfo CurrentCulture { get { return GameManager.currentCulture; } }
    [HideInInspector] private List<TimeWaitEvent> bankruptcyTimers = new List<TimeWaitEvent>();

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

        UnityEvent callback = new UnityEvent();
        callback.AddListener(OnSeriousBankruptcy);

        bankruptcyTimers.Add(TimeManager.RegisterTimeWaiter(TimeSpan.FromDays(daysBeforeSeriousBankruptcy), callback));
    }

    public virtual void OnSeriousBankruptcy()
    {
        UnityEvent callback = new UnityEvent();
        callback.AddListener(GameManager.GameOver);

        bankruptcyTimers.Add(TimeManager.RegisterTimeWaiter(TimeSpan.FromDays(daysBeforeGameOverBankruptcy), callback));
    }

    public virtual void OnEndBankruptcy()
    {
        isInBankruptcy = false;
        LastBankruptcyEnd = TimeManager.CurrentTime;

        foreach (TimeWaitEvent ev in bankruptcyTimers) { TimeManager.UnregisterTimeWaiter(ev); }
    }

    public string GetReadableBalance()
    {
        string after = "Balance: " + Balance.ToString("C", CurrentCulture);
        return after;
    }

    public bool IsCreditSufficient(decimal priceToCheck) 
    {
        if (Balance - priceToCheck <= 0)
            return false;
        else
            return true;
    }

    public void MakePurchase(decimal price) 
    {
        decimal balanceToApply = Balance - price;

        if (balanceToApply <= 0 && !GameManager.instance.Profile.enableBankruptcySystem)
        {
            balanceToApply = 0;
        }

        Balance = balanceToApply;
    }

    public void AddToBalance(decimal toAdd) 
    {
        Balance += toAdd;
    }
}
