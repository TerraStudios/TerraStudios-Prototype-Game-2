using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class TimeEngine : MonoBehaviour
{
    private Thread thread;

    [Header("Components")]
    public GameManger GameManager;
    public UMTD UMTD;

    [Header("Constant variables")]
    public int defaultTimeMultiplier;

    [Header("Dynamic variables")]
    public int timeMultiplier;
    public bool isPaused;
    public DateTime CurrentTime;

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
                while (!isPaused)
                {
                    CurrentTime = CurrentTime.AddMinutes(1);
                    int msToWait = Mathf.FloorToInt((float) 100 / (defaultTimeMultiplier * timeMultiplier));
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
        thread.Abort();
    }
}
