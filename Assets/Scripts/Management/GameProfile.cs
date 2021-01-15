//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using UnityEngine;

namespace CoreManagement
{
    public enum IncomeTaxType { Progressive, Flat }

    /// <summary>
    /// This class contains properties that we use to enforce different game experiences by changing values of various systems.
    /// </summary>
    [CreateAssetMenu(fileName = "New Game Profile", menuName = "Settings/New Game Profile")]
    public class GameProfile : ScriptableObject
    {
        public GameProfileData data = new GameProfileData();
    }

    [Serializable]
    public class GameProfileData
    {
        [Header("[WIP] Taxes - income tax")]
        public bool enableIncomeTax = true;
        public IncomeTaxType incomeTaxType;

        [Header("[WIP] Taxes - sales tax")]
        public bool enableSalesTax = true;

        [Header("[WIP] Taxes - eco tax")]
        public bool enableEcoTax = true;

        [Header("[WIP] Taxes - electricity tax")]
        public bool enableElectricityTax = true;

        [Header("Building Settings")]
        public bool enableBuildingDamage = true;
        public float monthsLifespanMultiplier = 1;
        public float timeToFixMultiplier = 1;

        public float buildingPenaltyForFixMultiplier = 1;

        public bool enableElectricityCalculations = true;
        public float wattsPerHourIdleMultiplier = 1;
        public float wattsPerHourWorkMultiplier = 1;

        [Header("APM Settings")]
        public float globalBaseTimeMultiplier = 1;

        [Header("Remove System Multipliers")]
        public float removePenaltyMultiplier = 0.4f;
        public float garbageRemoveMultiplier = 2;

        [Header("Economy System - Global Multipliers")]
        public float globalPriceMultiplierItems = 1;
        public float globalPriceMultiplierBuildings = 1;

        [Header("Economy System - Currency Visualization")]
        public bool forceManualCurrencyCC;
        public string currencyCC;
        [Tooltip("Freezes the balance to the start balance")]
        public bool enableGodMode;

        [Header("Bankruptcy System")]

        [Tooltip("Can the player build if *price of the building* > *amount of cash*")]
        public bool allowBuildingIfBalanceInsufficient = true;

        [Tooltip("Should the bankruptcy system turn on if cash below 0")]
        public bool enableBankruptcy = true;

        public int daysBeforeSeriousBankruptcy = 5;
        public int daysBeforeGameOverBankruptcy = 7;
    }
}
