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
using CoreManagement;
using UnityEngine;

namespace ItemManagement
{
    /// <summary>
    /// Properties for Item ScriptableObject.
    /// </summary>
    [Serializable]
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Item/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Properties")]
        [HideInInspector] public int id;
        public new string name;
        [TextArea] public string description;
        public ItemBehaviour obj;

        [Header("Economic Simulation Properties")]
        public float startingPriceInShop;

        public float StartingPriceInShop { get => startingPriceInShop * GameManager.Instance.CurrentGameProfile.globalPriceMultiplierItems; set => startingPriceInShop = value; }

        public bool isBuyable;
        public int ecoTax;
        public AnimationCurve demandCurve;
        public int maxProductPrice;
        public int maxConsumersAmount;
        public float[] seasonalPopularityOfProduct;
        public ItemData[] dependencies;

        //[Header("Health System Properties")]

        [Header("Basic Processing Machine Properties")]
        public bool isGarbage;
        public float materialHardness;
        public float meltTemperature;
        public float secondsToMelt;
        public float solidFormTemperature;
        public float secondsToFreeze;
    }
}
