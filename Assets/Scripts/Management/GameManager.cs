using DebugTools;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Collections;
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
        public static GameManager Instance;

        public bool DebugMode; //May be moved to Super Secret Settings later on

        [HideInInspector] public CultureInfo currentCultureTimeDate;
        [HideInInspector] public CultureInfo currentCultureCurrency;

        [Header("Game Settings")]
        private GameProfileData debugLoadedGameProfile;
        public GameProfile editorGameProfile;
        public List<GameProfilePlayData> gameProfiles;

        public GameProfileData CurrentGameProfile
        {
            get => GameSave.current.gameProfileData;
            set
            {
                GameSave.current.gameProfileData = value;
                debugLoadedGameProfile = value;
            }
        }

        private UserProfileData debugLoadedUserProfile;
        public UserProfile defaultUserProfile;
        public UserProfileData CurrentUserProfile
        {
            get => GameSave.current.userProfileData;
            set
            {
                GameSave.current.userProfileData = value;
                debugLoadedUserProfile = value;
            }
        }

        private void Update()
        {
            Debug.Log(CurrentGameProfile.enableBankruptcy);
            Debug.Log(CurrentUserProfile.currencyCC);
        }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(this);

            if (Application.isEditor && CurrentGameProfile == null)
            {
                CurrentGameProfile = editorGameProfile.data;
                Debug.Log("Applied default editor gprofile");
            }
            else
                Debug.Log("Skipping default editor gprofile");

            if (CurrentUserProfile == null)
                CurrentUserProfile = defaultUserProfile.data;

            GenerateCultures();

            Log.DEBUG_MODE = DebugMode; //Set the debug mode for logging
        }

        private void GenerateCultures()
        {
            // Game Profile always has higher priority than User Profile

            if (CurrentUserProfile.manualTimeDateCC)
                currentCultureTimeDate = CultureInfo.CreateSpecificCulture(CurrentUserProfile.timeDateCC);
            else
                currentCultureTimeDate = CultureInfo.CurrentCulture;

            if (CurrentGameProfile.forceManualCurrencyCC)
                currentCultureCurrency = CultureInfo.CreateSpecificCulture(CurrentGameProfile.currencyCC);
            else if (CurrentUserProfile.manualCurrencyCC)
                currentCultureCurrency = CultureInfo.CreateSpecificCulture(CurrentUserProfile.currencyCC);
            else
                currentCultureTimeDate = CultureInfo.CurrentCulture;

            Debug.Log("Current time culture: " + currentCultureCurrency.Name);
        }

        public void ResetGame()
        {
            GenerateCultures();
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
