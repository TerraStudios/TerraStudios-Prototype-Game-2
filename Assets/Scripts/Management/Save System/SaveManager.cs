using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public Transform GameSaveButtonPrefab;
    public Transform SelectSavePanel;
    public Transform SaveGameSavePanel;
    public Transform ConfirmOverwritePanel;
    public Button ConfirmOverwriteBtn;
    private List<GameObject> LoadGameSaveButtons = new List<GameObject>();
    private List<GameObject> SaveGameButtons = new List<GameObject>();
    public Transform LoadSaveButtonsRoot;
    public Transform SaveGameButtonsRoot;
    public TMP_InputField GameSaveName;

    public string[] saveFiles;
    public static bool saveLoaded;

    private void Awake()
    {
        instance = this;
    }

    public void ShowLoadScreen()
    {
        GetLoadFiles();

        foreach(GameObject button in LoadGameSaveButtons)
        {
            Destroy(button);
        }

        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject btnGO = Instantiate(GameSaveButtonPrefab, LoadSaveButtonsRoot).gameObject;
            LoadGameSaveButtons.Add(btnGO);

            GameSaveBtn btn = btnGO.GetComponent<GameSaveBtn>();

            int index = i;
            btn.SaveSelectedBtn.onClick.AddListener(() =>
            {
                OnSaveSelected(saveFiles[index]);
            });
            btn.TrashBtn.onClick.AddListener(() =>
            {
                OnGameSaveDelete(saveFiles[index]);
            });
            btn.SaveName.text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
        }
    }

    public void ShowSaveScreen() 
    {
        GetLoadFiles();

        foreach (GameObject button in SaveGameButtons)
        {
            Destroy(button);
        }

        Debug.Log("Found " + saveFiles.Length + " saves");

        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject btnGO = Instantiate(GameSaveButtonPrefab, SaveGameButtonsRoot).gameObject;
            SaveGameButtons.Add(btnGO);

            GameSaveBtn btn = btnGO.GetComponent<GameSaveBtn>();

            int index = i;
            btn.SaveSelectedBtn.onClick.AddListener(() =>
            {
                OnGameSaveSelectedOverwrite(saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "").Replace(".pbag", ""));
            });
            btn.TrashBtn.onClick.AddListener(() =>
            {
                OnGameSaveDelete(saveFiles[index]);
            });
            btn.SaveName.text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
        }
    }

    private void ReloadSavesList() 
    {
        if (SelectSavePanel)
            ShowLoadScreen();
        if (SaveGameSavePanel)
            ShowSaveScreen();
    }

    public void OnSaveSelected(string saveName)
    {
        saveLoaded = true;
        GameSave.current = (GameSave)SerializationManager.Load(saveName);
        GameManager.instance.ResetGame();
    }

    public void OnGameSaveSelectedOverwrite(string saveName)
    {
        ConfirmOverwritePanel.gameObject.SetActive(true);
        SaveGameSavePanel.gameObject.SetActive(false);
        ConfirmOverwriteBtn.onClick.RemoveAllListeners();
        ConfirmOverwriteBtn.onClick.AddListener( () => OnConfimOverwirte(saveName) );
    }

    private void OnConfimOverwirte(string saveName) 
    {
        ConfirmOverwritePanel.gameObject.SetActive(false);
        SaveGameSavePanel.gameObject.SetActive(true);
        SerializationManager.Save(saveName, GameSave.current);
    }

    public void OnGameSaveDelete(string saveName) 
    {
        File.Delete(saveName);
        ReloadSavesList();
    }

    public void OnSaveGameManualBtn()
    {
        if (File.Exists(Application.persistentDataPath + "/saves/" + GameSaveName.text + ".pbag"))
        {
            Debug.LogError("Save already exists");
            return;
        }

        SerializationManager.Save(GameSaveName.text, GameSave.current);
        ReloadSavesList();
        GameSaveName.text = "";
    }

    public void SaveAlreadyExistsDialog() 
    {
        
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
