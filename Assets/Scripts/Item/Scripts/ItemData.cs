using System;
using UnityEngine;

/// <summary>
/// Properties for Item ScriptableObject.
/// </summary>
[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[CreateAssetMenu(fileName = "New Item Data", menuName = "Item/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Properties")]
    [HideInInspector] public int ID;
    public new string name;
    [TextArea] public string description;
    public ItemBehaviour obj;

    [Header("Economic Simulation Properties")]
    public float startingPriceInShop;

    public float StartingPriceInShop { get => startingPriceInShop * GameManager.Instance.profile.globalPriceMultiplierItems; set => startingPriceInShop = value; }

    public bool isBuyable;
    public int ecoTax;
    public AnimationCurve demandCurve;
    public int maxProductPrice;
    public int maxConsumersAmount;
    public float[] seasonalPopularityOfProduct;
    public ItemData[] Dependencies;

    //[Header("Health System Properties")]

    [Header("Basic Processing Machine Properties")]
    public bool isGarbage;
    public float materialHardness;
    public float meltTemperature;
    public float secondsToMelt;
    public float solidFormTemperature;
    public float secondsToFreeze;
}
