using UnityEngine;

public enum IncomeTaxType { Progressive, Flat }

[CreateAssetMenu(fileName = "New Game Profile", menuName = "Settings/New Game Profile")]
public class GameProfile : ScriptableObject
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

    [Header("Remove System Multipliers")]
    public float removePenaltyMultiplier = 0.4f;
    public float garbageRemoveMultiplier = 2;

    [Header("Economy System - Global Multipliers")]
    public float globalPriceMultiplierItems = 1;
    public float globalPriceMultiplierBuildings = 1;
    [Tooltip("If disabled, the player can't go bellow 0 balance")]
    public bool enableBankruptcySystem = true;
}
