//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
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
