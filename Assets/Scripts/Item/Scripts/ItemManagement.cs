//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace ItemManagement
{
    /// <summary>
    /// Manages all items in the game. 
    /// Loads and retrives items from the items list.
    /// </summary>
    public class ItemManagement : MonoBehaviour
    {
        [Tooltip("Determines the initial pool size for each item")]
        public int initialPoolSize = 100;

        [HideInInspector] public static List<ItemData> db = new List<ItemData>();

        private void Awake()
        {
            LoadItemDB();
        }

        public ItemData GetItemFromDB(int ID)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (db[i].ID == ID)
                    return db[i];
            }

            return null;
        }

        private void LoadItemDB()
        {
            db.Clear();
            ItemData[] itemDB = Resources.LoadAll<ItemData>("");
            for (int i = 0; i < itemDB.Count(); i++)
            {
                ItemData data = itemDB[i];
                data.ID = i;
                db.Insert(i, data);
            }

            Debug.Log("[Item Management] Loaded " + db.Count + " items");

            foreach (ItemData data in db)
            {
                ObjectPoolManager.Instance.CreatePool(data.obj.gameObject, initialPoolSize);
            }
        }
    }
}
