using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    /// <summary>
    /// Determines the discount sell when manually removing an item through the ItemInfoUI.
    /// </summary>
    private const decimal REMOVE_PRICE_MULTIPLIER = 0.8M;


    [Header("Components")]
    public GameObject itemInfoUI;
    public Toggle buyableToggle;
    public TMP_Text category;
    public TMP_Text startingPrice;
    public TMP_Text description;
   
    public TMP_Text itemName;
    public ItemSelector selector;

    private GameObject currentItem;
    private ItemData currentData;


    public void SetData(GameObject gameObj, ItemData item)
    {
        buyableToggle.isOn = item.isBuyable;
        startingPrice.text = item.startingPriceInShop.ToString();
        category.text = item.ItemCategory.formattedName;
        description.text = item.description;
        itemName.text = item.name;

        this.currentItem = gameObj;
        this.currentData = item;
    }

    public void OnSellItem()
    {
        //NOTE: If there is a different price variable, change currentData.startingPriceInShop to whatever is needed.
        Debug.Log($"Adding {currentData.startingPriceInShop * REMOVE_PRICE_MULTIPLIER} to the balance.");
        EconomyManager.instance.Balance += currentData.startingPriceInShop * REMOVE_PRICE_MULTIPLIER;
        ObjectPoolManager.instance.DestroyObject(currentItem); //destroy object 

        //clean up 
        currentItem = null;
        currentData = null;
        OnUIExit(); //close UI after item has been deleted

    }

    public void OnUIOpen()
    {
        itemInfoUI.SetActive(true);
    }
    
    public void OnUIExit()
    {
        itemInfoUI.SetActive(false);
    }

}

