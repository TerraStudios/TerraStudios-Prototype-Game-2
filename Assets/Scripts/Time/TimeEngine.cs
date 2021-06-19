//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Threading;
using CoreManagement;
using SaveSystem;
using UnityEngine;
using Utilities;

namespace TimeSystem
{
    /// <summary>
    /// Lowest level time management script for handling time counting on a different thread.
    /// </summary>
    public class TimeEngine : MonoBehaviour
    {
        private Thread thread;

        [Header("Components")]
        [Tooltip("A reference to the UMTD")]
        public UMTD umtd;

        [Header("Constant variables")]
        [Tooltip("The default multiplier for time-based operations")]
        public int defaultTimeMultiplier;

        public static int TimeMultiplier
        {
            get => GameSave.current.timeSaveData.timeMultiplier;
            set
            {
                Time.timeScale = value;
                if (value <= 0)
                {
                    Debug.LogError("Attempting to apply value for TimeMultiplier <= 0. That's not allowed. Use TimeEngine.isPaused instead, if you want to pause time!");
                    return;
                }

                GameSave.current.timeSaveData.timeMultiplier = value;
            }
        }

        // Used for saving ONLY
        public static bool IsPaused_Save
        {
            set
            {
                if (PauseMenu.isOpen)
                    GameSave.current.timeSaveData.isPaused = PauseMenu.wasPaused;
                else
                    GameSave.current.timeSaveData.isPaused = value;
            }
        }

        private static bool isPaused;

        // Used for ingame processes
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

                IsPaused_Save = value;
                isPaused = value;
            }
        }

        public DateTime CurrentTime
        {
            get => GameSave.current.timeSaveData.currentTime;
            set => GameSave.current.timeSaveData.currentTime = value;
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
                        int msToWait = Mathf.FloorToInt((float)100 / (defaultTimeMultiplier * TimeMultiplier));
                        Thread.Sleep(msToWait);
                        umtd.Enqueue(() => OnCounterTick());
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

            isPaused = false;
        }
    }
}
