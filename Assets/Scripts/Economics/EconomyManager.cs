//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System;
using System.Collections.Generic;
using CoreManagement;
using SaveSystem;
using TimeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EconomyManagement
{
    /// <summary>
    /// A struct containing information about the result of a transaction.
    /// </summary>
    public struct TransactionResponse
    {
        public float amount;

        public class ResponseType
        {
            public static readonly ResponseType UNKNOWN_ERROR = new ResponseType("An unknown error has occured:\n{0}");
            public static readonly ResponseType INSUFFICIENT_BALANCE = new ResponseType("You don't have enough money!");
            public static readonly ResponseType SUCCESS = new ResponseType();

            private string errorMessage;

            public string GetErrorMessage(params object[] args)
            {
                return string.Format(errorMessage, args);
            }

            private ResponseType(string errorMessage = "")
            {
                this.errorMessage = errorMessage;
            }
        }

        public ResponseType response;

        public bool Succeeded
        {
            get => response.Equals(ResponseType.SUCCESS);
        }
    }

    /// <summary>
    /// Highest level class that handles economy simulations.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance;

        [Header("UI Components")]
        public TMP_Text currentBalanceText;

        [Header("Components")]
        public TimeManager timeManager;

        [Header("Constant variables")]
        public int startBalance = 1456536;
        public int balanceChangeTest;

        [Header("Dynamic variables")]
        public bool isInBankruptcy;
        public decimal Balance
        {
            get { return GameSave.current.economySaveData.balance; }
            set
            {
                if (!GameManager.Instance.CurrentGameProfile.enableGodMode)
                {
                    GameSave.current.economySaveData.balance = value;
                    OnBalanceUpdate();
                }
            }
        }
        public DateTime lastBankruptcyStart { get => GameSave.current.economySaveData.lastBankruptcyStart; set => GameSave.current.economySaveData.lastBankruptcyStart = value; }
        public DateTime lastBankruptcyEnd { get => GameSave.current.economySaveData.lastBankruptcyEnd; set => GameSave.current.economySaveData.lastBankruptcyEnd = value; }

        public List<TimeWaitEvent> bankruptcyTimers { get => GameSave.current.economySaveData.bankruptcyTimers; set => GameSave.current.economySaveData.bankruptcyTimers = value; }

        public int seriousBankruptcyId;
        public int gameOverId;

        private void Awake()
        {
            Instance = this;
            if (GameSave.current.economySaveData.balance == default)
                Balance = startBalance;
            else
                Balance = GameSave.current.economySaveData.balance;

            seriousBankruptcyId = CallbackHandler.Instance.RegisterCallback(EnterSeriousBankruptcy);
            gameOverId = CallbackHandler.Instance.RegisterCallback(GameManager.Instance.GameOver);
        }

        public void TestBalanceChange(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttemptTransaction(balanceChangeTest, true);
            }
        }

        public void OnBalanceUpdate()
        {
            MakeBankruptcyCheck();

            // UI

            currentBalanceText.text = GetReadableBalance();
            if (Balance > 0) { currentBalanceText.color = Color.white; }
        }

        public void EnterBankruptcy()
        {
            Debug.Log("Critical condition! Your balance is empty!");

            isInBankruptcy = true;
            lastBankruptcyStart = timeManager.CurrentTime;

            bankruptcyTimers.Add(timeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.Instance.CurrentGameProfile.daysBeforeSeriousBankruptcy), seriousBankruptcyId));

            // UI

            currentBalanceText.color = Color.red;
        }

        public void EnterSeriousBankruptcy()
        {
            bankruptcyTimers.Add(timeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.Instance.CurrentGameProfile.daysBeforeGameOverBankruptcy), gameOverId));


            Debug.Log("Extremely critical condition! Your're now in Serious Bankruptcy!");
        }

        public void EndBankruptcy()
        {
            Debug.Log("Recovered from bankruptcy!");
            isInBankruptcy = false;
            lastBankruptcyEnd = timeManager.CurrentTime;

            foreach (TimeWaitEvent ev in bankruptcyTimers) { timeManager.UnregisterTimeWaiter(ev); }

            // UI

            currentBalanceText.color = Color.green;
            TimeSpan bankruptcyDuration = lastBankruptcyEnd - lastBankruptcyStart;
            Debug.Log("The bankrupt lasted: " + bankruptcyDuration);
        }

        /// <summary>
        /// Processes a sum of money. Withdrawals or deposits depending if the sum is positive or negative.
        /// </summary>
        /// <param name="sum">Sum to add or remove.</param>
        /// <returns></returns>
        public TransactionResponse AttemptTransaction(float sum, bool bypassBalanceCheck = false)
        {
            try
            {
                if (sum >= 0)
                {
                    Balance += (decimal)sum;

                    return new TransactionResponse
                    {
                        response = TransactionResponse.ResponseType.SUCCESS
                    };
                }
                else
                {
                    float price = -sum;
                    if (CheckForSufficientFunds(price, bypassBalanceCheck))
                    {
                        Balance -= (decimal)price;

                        return new TransactionResponse
                        {
                            response = TransactionResponse.ResponseType.SUCCESS,
                            amount = price
                        };
                    }
                    else
                    {
                        return new TransactionResponse
                        {
                            response = TransactionResponse.ResponseType.INSUFFICIENT_BALANCE,
                            amount = price
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                return new TransactionResponse
                {
                    response = TransactionResponse.ResponseType.UNKNOWN_ERROR,
                };
            }
        }

        /// <summary>
        /// Checks whether there are enough funds in the balance to pay for that price.
        /// </summary>
        /// <param name="price">Price to check against the balance.</param>
        /// <returns></returns>
        public bool CheckForSufficientFunds(double price, bool bypassBalanceCheck = false)
        {
            if (bypassBalanceCheck)
                return true;

            if (Balance >= (decimal)price)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks whether we're in bankruptcy and takes the appropriate actions.
        /// </summary>
        private void MakeBankruptcyCheck()
        {
            if (!GameManager.Instance.CurrentGameProfile.enableBankruptcy)
                return;

            if (Balance < 0 && !isInBankruptcy)
            {
                EnterBankruptcy();
                return;
            }
            else if (Balance >= 0 && isInBankruptcy)
            {
                EndBankruptcy();
                return;
            }
        }

        /// <summary>
        /// Returns the current balance in a format decided by the current culture.
        /// </summary>
        /// <returns>Readable balance text.</returns>
        public string GetReadableBalance()
        {
            return "Balance: " + Balance.ToString("C", GameManager.Instance.currentCultureCurrency);
        }
    }
}
