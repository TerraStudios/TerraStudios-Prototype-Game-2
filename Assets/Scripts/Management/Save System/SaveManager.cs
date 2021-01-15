//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using CoreManagement;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaveSystem
{
    /// <summary>
    /// Handles communications between the UI and the SerializationManager for loading, saving, deleting and overwritting game saves.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance;

        public Transform gameSaveButtonPrefab;
        public Transform selectSavePanel;
        public Transform saveGameSavePanel;
        public Transform confirmOverwritePanel;
        public Button confirmOverwriteBtn;
        private readonly List<GameObject> LoadGameSaveButtons = new List<GameObject>();
        private readonly List<GameObject> SaveGameButtons = new List<GameObject>();
        public Transform loadSaveButtonsRoot;
        public Transform saveGameButtonsRoot;
        public TMP_InputField gameSaveName;

        public string[] saveFiles;
        public static bool saveLoaded;

        private void Awake()
        {
            Instance = this;
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
                GameObject btnGO = Instantiate(gameSaveButtonPrefab, loadSaveButtonsRoot).gameObject;
                LoadGameSaveButtons.Add(btnGO);

                GameSaveBtn btn = btnGO.GetComponent<GameSaveBtn>();

                int index = i;
                btn.saveSelectedBtn.onClick.AddListener(() =>
                {
                    OnSaveSelected(saveFiles[index]);
                });
                btn.trashBtn.onClick.AddListener(() =>
                {
                    OnGameSaveDelete(saveFiles[index]);
                });
                btn.saveName.text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
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
                GameObject btnGO = Instantiate(gameSaveButtonPrefab, saveGameButtonsRoot).gameObject;
                SaveGameButtons.Add(btnGO);

                GameSaveBtn btn = btnGO.GetComponent<GameSaveBtn>();

                int index = i;
                btn.saveSelectedBtn.onClick.AddListener(() =>
                {
                    OnGameSaveSelectedOverwrite(saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "").Replace(".pbag", ""));
                });
                btn.trashBtn.onClick.AddListener(() =>
                {
                    OnGameSaveDelete(saveFiles[index]);
                });
                btn.saveName.text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
            }
        }

        /// <summary>
        /// Refreshes the currently open panel (load or save)
        /// </summary>
        private void ReloadSavesList()
        {
            if (selectSavePanel)
                ShowLoadScreen();
            if (saveGameSavePanel)
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
            GameManager.Instance.ResetGame();
        }

        /// <summary>
        /// Called when a save is selected to be overwritten
        /// </summary>
        /// <param name="saveName">The save's name to overwrite</param>
        public void OnGameSaveSelectedOverwrite(string saveName)
        {
            confirmOverwritePanel.gameObject.SetActive(true);
            saveGameSavePanel.gameObject.SetActive(false);
            confirmOverwriteBtn.onClick.RemoveAllListeners();
            confirmOverwriteBtn.onClick.AddListener(() => OnConfimOverwirte(saveName));
        }

        /// <summary>
        /// Called when save overwrite is confirmed for that save name
        /// </summary>
        /// <param name="saveName">The name of the save to overwrite</param>
        private void OnConfimOverwirte(string saveName)
        {
            confirmOverwritePanel.gameObject.SetActive(false);
            saveGameSavePanel.gameObject.SetActive(true);
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
            if (File.Exists(Application.persistentDataPath + "/saves/" + gameSaveName.text + ".pbag"))
            {
                Debug.LogError("Save already exists");
                return;
            }

            SerializationManager.Save(gameSaveName.text, GameSave.current);
            ReloadSavesList();
            gameSaveName.text = "";
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
}
