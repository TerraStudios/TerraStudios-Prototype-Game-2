#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//Currently not used, not deleting until we know for sure what we want
public class InventoryItemEditor : EditorWindow
{
    /*

    public class MachineRecipeList : Object
    {
        public List<MachineRecipe> recipesList;

    }

    public MachineRecipeList recipes = new MachineRecipeList();
    private int viewIndex = 1;

    [MenuItem("Window/Inventory Item Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(InventoryItemEditor));
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            recipes = (AssetDatabase.LoadAssetAtPath(objectPath, typeof(MachineRecipeList))) as MachineRecipeList;
        }

    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Inventory Item Editor", EditorStyles.boldLabel);
        if (recipes != null)
        {
            if (GUILayout.Button("Show Item List"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = recipes;
            }
        }
        if (GUILayout.Button("Open Item List"))
        {
            OpenItemList();
        }
        if (GUILayout.Button("New Item List"))
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = recipes;
        }
        GUILayout.EndHorizontal();

        if (recipes == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Item List", GUILayout.ExpandWidth(false)))
            {
                CreateNewItemList();
            }
            if (GUILayout.Button("Open Existing Item List", GUILayout.ExpandWidth(false)))
            {
                OpenItemList();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        if (recipes != null)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (viewIndex > 1)
                    viewIndex--;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (viewIndex < recipes.itemList.Count)
                {
                    viewIndex++;
                }
            }

            GUILayout.Space(60);

            if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
            {
                AddItem();
            }
            if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
            {
                DeleteItem(viewIndex - 1);
            }

            GUILayout.EndHorizontal();
            if (recipes.itemList == null)
                Debug.Log("Inventory is empty");
            if (recipes.itemList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 1, inventoryItemList.itemList.Count);
                //Mathf.Clamp (viewIndex, 1, inventoryItemList.itemList.Count);
                EditorGUILayout.LabelField("of   " + recipes.itemList.Count.ToString() + "  items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                recipes.itemList[viewIndex - 1].itemName = EditorGUILayout.TextField("Item Name", recipes.itemList[viewIndex - 1].itemName as string);
                recipes.itemList[viewIndex - 1].itemIcon = EditorGUILayout.ObjectField("Item Icon", recipes.itemList[viewIndex - 1].itemIcon, typeof(Texture2D), false) as Texture2D;
                recipes.itemList[viewIndex - 1].itemObject = EditorGUILayout.ObjectField("Item Object", recipes.itemList[viewIndex - 1].itemObject, typeof(Rigidbody), false) as Rigidbody;

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                recipes.itemList[viewIndex - 1].isUnique = (bool)EditorGUILayout.Toggle("Unique", recipes.itemList[viewIndex - 1].isUnique, GUILayout.ExpandWidth(false));
                recipes.itemList[viewIndex - 1].isIndestructible = (bool)EditorGUILayout.Toggle("Indestructable", recipes.itemList[viewIndex - 1].isIndestructible, GUILayout.ExpandWidth(false));
                recipes.itemList[viewIndex - 1].isQuestItem = (bool)EditorGUILayout.Toggle("QuestItem", recipes.itemList[viewIndex - 1].isQuestItem, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                recipes.itemList[viewIndex - 1].isStackable = (bool)EditorGUILayout.Toggle("Stackable ", recipes.itemList[viewIndex - 1].isStackable, GUILayout.ExpandWidth(false));
                recipes.itemList[viewIndex - 1].destroyOnUse = (bool)EditorGUILayout.Toggle("Destroy On Use", recipes.itemList[viewIndex - 1].destroyOnUse, GUILayout.ExpandWidth(false));
                recipes.itemList[viewIndex - 1].encumbranceValue = EditorGUILayout.FloatField("Encumberance", recipes.itemList[viewIndex - 1].encumbranceValue, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

            }
            else
            {
                GUILayout.Label("This Inventory List is Empty.");
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventoryItemList);
        }
    }

    void CreateNewItemList()
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        viewIndex = 1;
        inventoryItemList = CreateInventoryItemList.Create();
        if (inventoryItemList)
        {
            inventoryItemList.itemList = new List<InventoryItem>();
            string relPath = AssetDatabase.GetAssetPath(inventoryItemList);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }

    void OpenItemList()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Inventory Item List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            inventoryItemList = AssetDatabase.LoadAssetAtPath(relPath, typeof(InventoryItemList)) as InventoryItemList;
            if (inventoryItemList.itemList == null)
                inventoryItemList.itemList = new List<InventoryItem>();
            if (inventoryItemList)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    void AddItem()
    {
        InventoryItem newItem = new InventoryItem();
        newItem.itemName = "New Item";
        inventoryItemList.itemList.Add(newItem);
        viewIndex = inventoryItemList.itemList.Count;
    }

    void DeleteItem(int index)
    {
        inventoryItemList.itemList.RemoveAt(index);
    }
    */
}
#endif