﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : TimeSystem
{
    [Header("UI Components")]
    public TMP_Text CurrentDateText;
    public TMP_Text CurrentTimeText;

    public Button PauseButton;
    public Button x1Button;
    public Button x4Button;
    public Button x8Button;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (GameSave.current.TimeSaveData.currentTime == default)
            StartCounting(DateTime.Now);
        else
            StartCounting(GameSave.current.TimeSaveData.currentTime);

        OnCounterTick();

        PauseButton.onClick.AddListener(() => OnSpeedButtonClicked(PauseButton));
        x1Button.onClick.AddListener(() => OnSpeedButtonClicked(x1Button));
        x4Button.onClick.AddListener(() => OnSpeedButtonClicked(x4Button));
        x8Button.onClick.AddListener(() => OnSpeedButtonClicked(x8Button));
    }

    private void OnSpeedButtonClicked(Button clicked)
    {
        clicked.interactable = false;

        if (clicked == PauseButton)
        {
            x1Button.interactable = true;
            x4Button.interactable = true;
            x8Button.interactable = true;

            IsPaused = true;
        }

        else if (clicked == x1Button)
        {
            PauseButton.interactable = true;
            x4Button.interactable = true;
            x8Button.interactable = true;

            IsPaused = false;
            TimeMultiplier = 1;
        }

        else if (clicked == x4Button)
        {
            x1Button.interactable = true;
            PauseButton.interactable = true;
            x8Button.interactable = true;

            IsPaused = false;
            TimeMultiplier = 4;
        }

        else if (clicked == x8Button)
        {
            x1Button.interactable = true;
            PauseButton.interactable = true;
            x4Button.interactable = true;

            IsPaused = false;
            TimeMultiplier = 8;
        }
    }

    public override void OnCounterTick()
    {
        base.OnCounterTick();
        CurrentTimeText.text = GetReadableHourTime();
        CurrentDateText.text = GetReadableDateTime();
    }
}
