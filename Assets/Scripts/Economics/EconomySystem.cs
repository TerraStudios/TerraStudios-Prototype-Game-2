//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using CoreManagement;
using SaveSystem;
using TimeSystem;
using UnityEngine;

namespace EconomyManagement
{
    /// <summary>
    /// A struct containing information about the result of a transaction.
    /// </summary>
    public struct TransactionResponse
    {
        public enum ResponseType { UNKNOWN_ERROR, INSUFFICIENT_BALANCE, SUCCESS }

        public ResponseType response;

        public bool Succeeded
        {
            get => response.Equals(ResponseType.SUCCESS);
        }
    }

    /// <summary>
    /// Handles the economy calculations.
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

        public int seriousBankruptcyID;
        public int gameOverID;

        public virtual void OnBalanceUpdate() { MakeBankruptcyCheck(); }

        /// <summary>
        /// Processes a sum of money. Withdrawals or deposits depending if the sum is positive or negative.
        /// </summary>
        /// <param name="sum">Sum to add or remove.</param>
        /// <returns></returns>
        public TransactionResponse ProcessSum(float sum)
        {
            if (sum >= 0)
                return Deposit(sum);
            else
                return AttemptWithdrawal(-sum); // We flip the sign of the sum here to ensure it doesn't get flipped inside AttemptWithdrawal
        }

        /// <summary>
        /// Deposits a sum to the balance.
        /// </summary>
        /// <param name="sum">Sum to add to the balance.</param>
        /// <returns></returns>
        public TransactionResponse Deposit(float sum)
        {
            try
            {
                if (sum <= 0)
                    throw new UnityException("Attempted to deposit with a negative sum! " + sum);

                Balance += (decimal)sum;

                return new TransactionResponse
                {
                    response = TransactionResponse.ResponseType.SUCCESS
                };
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());

                return new TransactionResponse
                {
                    response = TransactionResponse.ResponseType.UNKNOWN_ERROR
                };
            }
        }

        /// <summary>
        /// Attempts to withdraw an amount of money from the balance.
        /// </summary>
        /// <param name="price">Amount to remove.</param>
        /// <returns></returns>
        public TransactionResponse AttemptWithdrawal(float price)
        {
            TransactionResponse response;
            try
            {
                if (price <= 0)
                    throw new UnityException("Attempted to withdraw with a negative price! " + price);

                response = CheckForSufficientFunds(price);

                if (response.Succeeded)
                {
                    Balance -= (decimal)price;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());

                return new TransactionResponse
                {
                    response = TransactionResponse.ResponseType.UNKNOWN_ERROR
                };
            }

            return response;
        }

        /// <summary>
        /// Checks whether there are enough funds in the balance to pay for that price.
        /// </summary>
        /// <param name="price">Price to check against the balance.</param>
        /// <returns></returns>
        public TransactionResponse CheckForSufficientFunds(double price)
        {
            if (Balance >= (decimal) price)
            {
                return new TransactionResponse
                {
                    response = TransactionResponse.ResponseType.SUCCESS
                };
            }
            else
            {
                return new TransactionResponse
                {
                    response = TransactionResponse.ResponseType.INSUFFICIENT_BALANCE
                };
            }
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
                OnEnterBankruptcy();
                return;
            }
            else if (Balance >= 0 && isInBankruptcy)
            {
                OnEndBankruptcy();
                return;
            }
        }

        /// <summary>
        /// Called when the Economy System enter a state of bankruptcy.
        /// </summary>
        public virtual void OnEnterBankruptcy()
        {
            isInBankruptcy = true;
            lastBankruptcyStart = timeManager.CurrentTime;

            bankruptcyTimers.Add(timeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.Instance.CurrentGameProfile.daysBeforeSeriousBankruptcy), seriousBankruptcyID));
        }

        /// <summary>
        /// Called when the Economy System enter a serious state of bankruptcy.
        /// </summary>
        public virtual void OnSeriousBankruptcy()
        {
            bankruptcyTimers.Add(timeManager.RegisterTimeWaiter(TimeSpan.FromDays(GameManager.Instance.CurrentGameProfile.daysBeforeGameOverBankruptcy), gameOverID));
        }

        /// <summary>
        /// Called when the Economy System exists from state of bankruptcy.
        /// </summary>
        public virtual void OnEndBankruptcy()
        {
            isInBankruptcy = false;
            lastBankruptcyEnd = timeManager.CurrentTime;

            foreach (TimeWaitEvent ev in bankruptcyTimers) { timeManager.UnregisterTimeWaiter(ev); }
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
