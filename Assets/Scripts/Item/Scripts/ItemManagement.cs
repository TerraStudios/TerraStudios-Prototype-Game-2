using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
            ObjectPoolManager.instance.CreatePool(data.obj.gameObject, initialPoolSize);
        }
    }
}
