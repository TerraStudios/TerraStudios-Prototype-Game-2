﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategories { Food, Medecine, Resources }

[CreateAssetMenu(fileName = "New Item Data", menuName = "Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Properties")]
    public int ID;
    public new string name;
    [TextArea] public string description;
    public Transform obj;
    public ItemCategories ItemCategory;

    [Header("Economic Simulation Properties")]
    public int startingPriceInShop;
    public bool isBuyable;
    public int ecoTax;
    public AnimationCurve demandCurve;
    public int maxProductPrice;
    public int maxConsumersAmount;
    public float[] seasonalPopularityOfProduct;
    public ItemData[] Dependencies;

    //[Header("Health System Properties")]

    [Header("Basic Processing Machine Properties")]
    public float materialHardness;
    public float meltTemperature;
    public float secondsToMelt;
    public float solidFormTemperature;
    public float secondsToFreeze;
}
