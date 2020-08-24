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
    public GameManger GameManager;
    [Tooltip("A reference to the UMTD")]
    public UMTD UMTD;

    [Header("Constant variables")]
    [Tooltip("The default multiplier for time-based operations")]
    public int defaultTimeMultiplier;

    [Header("Dynamic variables")]
    private static int timeMultiplier;
    public static int TimeMultiplier
    {
        get => timeMultiplier;
        set
        {
            Time.timeScale = value;
            if (value <= 0)
            {
                Debug.LogError("Attempting to apply value for TimeMultiplier <= 0. That's not allowed. Use TimeEngine.isPaused instead, if you want to pause time!");
                return;
            } 
            timeMultiplier = value;
        }
    }

    public static bool IsPaused 
    {
        get => isPaused;
        set
        {
            if (value)
            {
                Time.timeScale = 0;
            }
            else
                Time.timeScale = TimeMultiplier;
                
            isPaused = value;
        }
    }

    [Tooltip("Whether the time is currently paused or not")]
    private static bool isPaused;
    [Tooltip("The current DateTime of the game")]
    public DateTime CurrentTime;

    public void StartClock() 
    {
        TimeMultiplier = 1;
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
    }
}
