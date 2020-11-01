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
        if (GameSave.current.EconomySaveData.balanace == default)
            Balance = startBalance;
        else
            Balance = GameSave.current.EconomySaveData.balanace;

        SeriousBankruptcyID = CallbackHandler.instance.RegisterCallback(OnSeriousBankruptcy);
        GameOverID = CallbackHandler.instance.RegisterCallback(GameManager.GameOver);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            Balance += balanceChangeTest;
        }
    }

    public override void OnBalanceUpdate()
    {
        base.OnBalanceUpdate();
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
