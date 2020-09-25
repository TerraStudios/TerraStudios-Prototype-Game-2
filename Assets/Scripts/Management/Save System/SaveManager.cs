using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public void OnLoadGame()
    {
        //TODO: Specify path to file with input field
        GameSave.current = (GameSave)SerializationManager.Load(Application.persistentDataPath + "/saves/Save.save");

    }
    public void OnSaveGame()
    {
        //TODO: Specify save name with input field
        SerializationManager.Save("Save", GameSave.current);
    }
}
