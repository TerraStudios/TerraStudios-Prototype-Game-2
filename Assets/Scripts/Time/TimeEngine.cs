using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class TimeEngine : MonoBehaviour
{
    private Thread thread = null;

    [Header("Components")]
    [Tooltip("A reference to the GameManager")]
    public GameManager GameManager;
    [Tooltip("A reference to the UMTD")]
    public UMTD UMTD;

    [Header("Constant variables")]
    [Tooltip("The default multiplier for time-based operations")]
    public int defaultTimeMultiplier;

    public static int TimeMultiplier
    {
        get => GameSave.current.TimeSaveData.timeMultiplier;
        set
        {
            Time.timeScale = value;
            if (value <= 0)
            {
                Debug.LogError("Attempting to apply value for TimeMultiplier <= 0. That's not allowed. Use TimeEngine.isPaused instead, if you want to pause time!");
                return;
            }

            GameSave.current.TimeSaveData.timeMultiplier = value;
        }
    }

    // Used for saving ONLY
    public static bool IsPaused_Save
    {
        set
        {
            if (PauseMenu.isOpen)
                GameSave.current.TimeSaveData.isPaused = PauseMenu.wasPaused;
            else
                GameSave.current.TimeSaveData.isPaused = value;
        }
    }

    private static bool _isPaused;

    // Used for ingame processes
    public static bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (value)
            {
                Time.timeScale = 0;
            }
            else
                Time.timeScale = TimeMultiplier;

            IsPaused_Save = value;
            _isPaused = value;
        }
    }

    public DateTime CurrentTime 
    {
        get => GameSave.current.TimeSaveData.currentTime;
        set => GameSave.current.TimeSaveData.currentTime = value;
    }

    public void StartClock() 
    {
        thread = new Thread(new ThreadStart(CounterWork));
        thread.Start();
    }

    public void CounterWork()
    {
        try
        {
            while (true)
            {
                while (!IsPaused)
                {
                    CurrentTime = CurrentTime.AddMinutes(1);
                    int msToWait = Mathf.FloorToInt((float) 100 / (defaultTimeMultiplier * TimeMultiplier));
                    Thread.Sleep(msToWait);
                    UMTD.Enqueue(() => OnCounterTick());
                }
            }
        }
        catch (Exception ex)
        {
            // log errors
            Debug.Log("Error in TimeEngine Thread, this is normal however. -> " + ex.ToString());
        }
    }

    public virtual void OnCounterTick() { }

    private void OnDisable()
    {
        if (thread != null)
            thread.Abort();

        _isPaused = false;
    }
}
