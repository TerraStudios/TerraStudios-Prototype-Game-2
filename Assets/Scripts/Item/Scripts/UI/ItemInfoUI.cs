//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ItemManagement
{
    /// <summary>
    /// Used to manage the UI of the Item Selection panel.
    /// </summary>
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

            currentItem = gameObj;
            currentData = item;
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
}
