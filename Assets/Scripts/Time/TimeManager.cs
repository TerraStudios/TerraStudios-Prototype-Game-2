﻿//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TimeSystem
{
    /// <summary>
    /// Highest-level time management script used for connecting the whole system with the UI.
    /// </summary>
    public class TimeManager : TimeSystem
    {
        [Header("UI Components")]
        public TMP_Text currentDateText;
        public TMP_Text currentTimeText;

        public Button pauseButton;
        public Button x1Button;
        public Button x4Button;
        public Button x8Button;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (GameSave.current.timeSaveData.currentTime == default)
                StartCounting(DateTime.Now);
            else
                StartCounting(GameSave.current.timeSaveData.currentTime);

            OnCounterTick();

            pauseButton.onClick.AddListener(() => OnSpeedButtonClicked(pauseButton));
            x1Button.onClick.AddListener(() => OnSpeedButtonClicked(x1Button));
            x4Button.onClick.AddListener(() => OnSpeedButtonClicked(x4Button));
            x8Button.onClick.AddListener(() => OnSpeedButtonClicked(x8Button));
        }

        private void OnSpeedButtonClicked(Button clicked)
        {
            clicked.interactable = false;

            if (clicked == pauseButton)
            {
                x1Button.interactable = true;
                x4Button.interactable = true;
                x8Button.interactable = true;

                IsPaused = true;
            }

            else if (clicked == x1Button)
            {
                pauseButton.interactable = true;
                x4Button.interactable = true;
                x8Button.interactable = true;

                IsPaused = false;
                TimeMultiplier = 1;
            }

            else if (clicked == x4Button)
            {
                x1Button.interactable = true;
                pauseButton.interactable = true;
                x8Button.interactable = true;

                IsPaused = false;
                TimeMultiplier = 4;
            }

            else if (clicked == x8Button)
            {
                x1Button.interactable = true;
                pauseButton.interactable = true;
                x4Button.interactable = true;

                IsPaused = false;
                TimeMultiplier = 8;
            }
        }

        public override void OnCounterTick()
        {
            base.OnCounterTick();
            currentTimeText.text = GetReadableHourTime();
            currentDateText.text = GetReadableDateTime();
        }
    }
}
