using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[CreateAssetMenu(fileName = "New Item Data", menuName = "Item/Item")]
public class ItemData : ItemTag
{
    [Header("Basic Properties")]
    [HideInInspector] public int ID;
    public new string name;
    [TextArea] public string description;
    public ItemBehaviour obj;
    public ItemCategory ItemCategory;

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

    public override bool Matches(ItemData item)
    {
        return item.ID == this.ID;
    }

}
