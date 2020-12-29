using DebugTools;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CoreManagement
{
    [Serializable]
    public class GameProfilePlayData
    {
        public string name;
        public GameProfile profile;
        public bool isHidden;
    }

    /// <summary>
    /// Top-level script of the game.
    /// Holds properties like CultureInfo, GameProfile and UserProfile.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public bool DebugMode; //May be moved to Super Secret Settings later on

        [HideInInspector] public CultureInfo currentCultureTimeDate;
        [HideInInspector] public CultureInfo currentCultureCurrency;

        [Header("Game Settings")]
        public GameProfile editorGameProfile;
        public List<GameProfilePlayData> gameProfiles;
        public static GameProfile currentGameProfile;

        public UserProfile uProfile;

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(this);

            if (Application.isEditor)
                currentGameProfile = editorGameProfile;

            // Game Profile always has higher priority than User Profile

            if (uProfile.manualTimeDateCC)
                currentCultureTimeDate = CultureInfo.CreateSpecificCulture(uProfile.timeDateCC);
            else
                currentCultureTimeDate = CultureInfo.CurrentCulture;

            if (currentGameProfile.forceManualCurrencyCC)
                currentCultureCurrency = CultureInfo.CreateSpecificCulture(currentGameProfile.currencyCC);
            else if (uProfile.manualCurrencyCC)
                currentCultureCurrency = CultureInfo.CreateSpecificCulture(uProfile.currencyCC);
            else
                currentCultureTimeDate = CultureInfo.CurrentCulture;

            Log.DEBUG_MODE = DebugMode; //Set the debug mode for logging
        }

        public void ResetGame()
        {
            StartCoroutine(ResetGameAction());
        }

        private IEnumerator ResetGameAction()
        {
            yield return StartCoroutine(SceneHandler.instance.ReloadLevelAction());
            InitGame();
        }

        public void InitGame()
        {
            Time.timeScale = GameSave.current.timeSaveData.timeMultiplier;
        }

        public void GameOver()
        {
            Debug.Log("Game Over!");
        }
    }
}
