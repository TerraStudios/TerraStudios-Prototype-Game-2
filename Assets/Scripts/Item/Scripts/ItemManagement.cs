using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemManagement : MonoBehaviour
{
    [HideInInspector] public List<ItemData> db = new List<ItemData>();

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
        db.AddRange(Resources.LoadAll<ItemData>(""));
        Debug.Log("[Item Management] Loaded " + db.Count + " items");
    }
}
