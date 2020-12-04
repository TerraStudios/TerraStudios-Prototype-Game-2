using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles communications between the UI and the SerializationManager for loading, saving, deleting and overwritting game saves.
/// </summary>
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

    /// <summary>
    /// Loads saves for the load screen and sets up button events
    /// </summary>
    public void ShowLoadScreen()
    {
        GetLoadFiles();

        foreach (GameObject button in LoadGameSaveButtons)
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

    /// <summary>
    /// Loads saves for the save screen and sets up button events
    /// </summary>
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

    /// <summary>
    /// Refreshes the currently open panel (load or save)
    /// </summary>
    private void ReloadSavesList()
    {
        if (SelectSavePanel)
            ShowLoadScreen();
        if (SaveGameSavePanel)
            ShowSaveScreen();
    }

    /// <summary>
    /// Called when a save is selected from the load menu
    /// </summary>
    /// <param name="saveName">The name of the save to load</param>
    public void OnSaveSelected(string saveName)
    {
        saveLoaded = true;
        GameSave.current = (GameSave)SerializationManager.Load(saveName);
        GameManager.instance.ResetGame();
    }

    /// <summary>
    /// Called when a save is selected to be overwritten
    /// </summary>
    /// <param name="saveName">The save's name to overwrite</param>
    public void OnGameSaveSelectedOverwrite(string saveName)
    {
        ConfirmOverwritePanel.gameObject.SetActive(true);
        SaveGameSavePanel.gameObject.SetActive(false);
        ConfirmOverwriteBtn.onClick.RemoveAllListeners();
        ConfirmOverwriteBtn.onClick.AddListener(() => OnConfimOverwirte(saveName));
    }

    /// <summary>
    /// Called when save overwrite is confirmed for that save name
    /// </summary>
    /// <param name="saveName">The name of the save to overwrite</param>
    private void OnConfimOverwirte(string saveName)
    {
        ConfirmOverwritePanel.gameObject.SetActive(false);
        SaveGameSavePanel.gameObject.SetActive(true);
        SerializationManager.Save(saveName, GameSave.current);
    }

    /// <summary>
    /// Called when a save is selected to be removed
    /// </summary>
    /// <param name="saveName">The name of the save</param>
    public void OnGameSaveDelete(string saveName)
    {
        File.Delete(saveName);
        ReloadSavesList();
    }

    /// <summary>
    /// Called from manual save UI button. Used for loading specific save from name that is written in GameSaveName field
    /// </summary>
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

    /// <summary>
    /// Loads and stores all save files present on the computer
    /// </summary>
    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");
    }
}