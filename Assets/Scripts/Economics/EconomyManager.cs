using System;
using TMPro;
using UnityEngine;

public class EconomyManager : EconomySystem
{
    [Header("UI Components")]
    public TMP_Text currentBalanceText;

    public static EconomyManager instance;

    private void Awake()
    {
        instance = this;
        Balance = startBalance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            if (!UpdateBalance(Mathf.Abs(balanceChangeTest)))
                Debug.LogWarning("Unable to apply test balance update.");
        }
    }

    public override void OnBalanceUpdate()
    {
        base.OnBalanceUpdate();
        Debug.Log("New balance: " + Balance);
        currentBalanceText.text = GetReadableBalance();

        if (Balance > 0) { currentBalanceText.color = Color.white; }
    }

    public override void OnEnterBankruptcy()
    {
        base.OnEnterBankruptcy();
        Debug.Log("Critical condition! Your balance is empty!");

        currentBalanceText.color = Color.red;
    }

    public override void OnSeriousBankruptcy()
    {
        base.OnSeriousBankruptcy();
        Debug.Log("Extremely critical condition! Your're now in Serious Bankruptcy!");
    }

    public override void OnEndBankruptcy()
    {
        base.OnEndBankruptcy();
        Debug.Log("Recovered from bankruptcy!");

        currentBalanceText.color = Color.green;
        TimeSpan howMuchBankruptcyLasted = LastBankruptcyEnd - LastBankruptcyStart;
        Debug.Log("The bankrupt lasted: " + howMuchBankruptcyLasted);
    }
}
