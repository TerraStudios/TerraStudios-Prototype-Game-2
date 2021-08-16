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
            RemoveSystem.Instance.DeleteItem(currentData, currentItem);

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
