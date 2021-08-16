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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace ItemManagement
{
    /// <summary>
    /// Manages all items in the game. 
    /// Loads and retrieves items from the items list.
    /// </summary>
    public class ItemManagement : MonoBehaviour
    {
        [Tooltip("Determines the initial pool size for each item")]
        public int initialPoolSize = 100;

        public static List<ItemData> Db = new List<ItemData>();

        private void Awake()
        {
            LoadItemDB();
        }

        public ItemData GetItemFromDB(int ID)
        {
            for (int i = 0; i < Db.Count; i++)
            {
                if (Db[i].id == ID)
                    return Db[i];
            }

            return null;
        }

        private void LoadItemDB()
        {
            Db.Clear();
            ItemData[] itemDB = Resources.LoadAll<ItemData>("");
            for (int i = 0; i < itemDB.Count(); i++)
            {
                ItemData data = itemDB[i];
                data.id = i;
                Db.Insert(i, data);
            }

            Debug.Log("[Item Management] Loaded " + Db.Count + " items");

            foreach (ItemData data in Db)
            {
                ObjectPoolManager.Instance.CreatePool(data.obj.gameObject, initialPoolSize);
            }
        }
    }
}
