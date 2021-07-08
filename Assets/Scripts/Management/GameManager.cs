//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DebugTools;
using SaveSystem;
using Unity.Collections;
using Unity.Jobs.LowLevel.Unsafe;
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

        [Header("Editor - Performance")]
        public NativeLeakDetectionMode leakDetectionMode = NativeLeakDetectionMode.Enabled;

        [Header("Job System - Performance")]
        [Tooltip("Worker threads available to the Job Queue. Recommended: Job Worker Count = number of available CPU cores - 1")]
        public int jobWorkerCount = 5;
        [Tooltip("Desired Job Count used for scheduling a conveyor item movement job.")]
        public int conveyorDesiredJobCount = -1;

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
                CurrentGameProfile = editorGameProfile.data;

            if (CurrentUserProfile == null)
                CurrentUserProfile = defaultUserProfile.data;

            GenerateCultures();

            Log.DEBUG_MODE = DebugMode; //Set the debug mode for logging

            JobsUtility.JobWorkerCount = jobWorkerCount;
        }

        private void Update()
        {
            if (Application.isEditor)
                NativeLeakDetection.Mode = leakDetectionMode;
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
            PauseMenu.isOpen = false;
        }

        public void GameOver()
        {
            Debug.Log("Game Over!");
        }
    }
}
