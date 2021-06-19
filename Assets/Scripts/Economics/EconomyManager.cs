//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using CoreManagement;
using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EconomyManagement
{
    /// <summary>
    /// Highest level class that handles economy simulations.
    /// </summary>
    public class EconomyManager : EconomySystem
    {
        [Header("UI Components")]
        public TMP_Text currentBalanceText;

        public static EconomyManager Instance;

        private void Awake()
        {
            Instance = this;
            if (GameSave.current.economySaveData.balance == default)
                Balance = startBalance;
            else
                Balance = GameSave.current.economySaveData.balance;

            seriousBankruptcyID = CallbackHandler.Instance.RegisterCallback(OnSeriousBankruptcy);
            gameOverID = CallbackHandler.Instance.RegisterCallback(GameManager.Instance.GameOver);
        }

        public void TestBalanceChange(InputAction.CallbackContext context)
        {
            if (context.performed)
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
            TimeSpan howMuchBankruptcyLasted = lastBankruptcyEnd - lastBankruptcyStart;
            Debug.Log("The bankrupt lasted: " + howMuchBankruptcyLasted);
        }
    }
}
