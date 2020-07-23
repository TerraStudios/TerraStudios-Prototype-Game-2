using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class TimeEngine : MonoBehaviour
{
    private Thread thread = null;

    [Header("Components")]
    public GameManger GameManager;
    public UMTD UMTD;

    [Header("Constant variables")]
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
    public static bool isPaused;
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
                while (!isPaused)
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
