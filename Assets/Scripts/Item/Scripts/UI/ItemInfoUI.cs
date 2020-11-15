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
        startingPrice.text = item.StartingPriceInShop.ToString();
        description.text = item.description;
        itemName.text = item.name;

        this.currentItem = gameObj;
        this.currentData = item;
    }

    public void OnSellItem()
    {
        RemoveSystem.instance.DeleteItem(currentData, currentItem);

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

