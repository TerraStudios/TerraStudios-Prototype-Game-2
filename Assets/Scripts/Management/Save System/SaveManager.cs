using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public Transform GameSaveButtonPrefab;
    private List<GameObject> LoadedGameSaveButtons = new List<GameObject>();
    public Transform GameSaveButtonsRoot;
    public TMP_InputField GameSaveName;

    public string[] saveFiles;

    public void ShowLoadScreen()
    {
        GetLoadFiles();

        foreach(GameObject button in LoadedGameSaveButtons)
        {
            Destroy(button);
        }

        Debug.Log("Found " + saveFiles.Length + " saves");

        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject btn = Instantiate(GameSaveButtonPrefab, GameSaveButtonsRoot).gameObject;
            LoadedGameSaveButtons.Add(btn);

            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnSaveSelected(saveFiles[index]);
            });
            btn.GetComponentInChildren<TextMeshProUGUI>().text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
        }
    }

    public void OnSaveSelected(string saveName)
    {
        GameSave.current = (GameSave)SerializationManager.Load(saveName);
        GameManager.instance.ResetGame();
    }

    public void OnSaveGame()
    {
        SerializationManager.Save(GameSaveName.text, GameSave.current);
    }

    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");
    }
}
