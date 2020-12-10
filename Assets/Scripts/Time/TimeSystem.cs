using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace TimeSystem
{
    [Serializable]
    public struct TimeWaitEvent
    {
        public DateTime currentTime;
        public TimeSpan waitTime;
        public int methodID;
    }

    [Serializable]
    public class TimeCountEvent
    {
        public Guid hash;
        public bool isPaused;
        public TimeSpan timePassed;
    }

    /// <summary>
    /// Mid-level time management script for handling the TimeEngine thread.
    /// Also used for handling Time Waiters and Time Counters.
    /// </summary>
    public class TimeSystem : TimeEngine
    {
        [HideInInspector] public CultureInfo CurrentCulture { get { return GameManager.Instance.currentCultureTimeDate; } }

        public List<TimeWaitEvent> timeWaiters { get => GameSave.current.timeSaveData.timeWaiters; set => GameSave.current.timeSaveData.timeWaiters = value; }
        public List<TimeCountEvent> timeCounters { get => GameSave.current.timeSaveData.timeCounters; set => GameSave.current.timeSaveData.timeCounters = value; }

        public void StartCounting(DateTime? startTime)
        {
            if (startTime.Value != null)
                CurrentTime = startTime.Value;
            StartClock();
        }

        public override void OnCounterTick()
        {
            for (int i = 0; i < timeWaiters.Count; i++)
            {
                TimeWaitEvent ev = timeWaiters[i];
                if (CurrentTime - ev.currentTime >= ev.waitTime)
                {
                    CallbackHandler.Instance.GetEvent(ev.methodID).Invoke();
                    timeWaiters.Remove(ev);
                    break;
                }
            }

            for (int i = 0; i < timeCounters.Count; i++)
            {
                TimeCountEvent ev = timeCounters[i];
                if (!ev.isPaused)
                {
                    ev.timePassed += TimeSpan.FromMinutes(1);
                }
            }
        }

        public TimeWaitEvent RegisterTimeWaiter(TimeSpan waitTime, int methodID)
        {
            TimeWaitEvent waitEvent = new TimeWaitEvent()
            {
                waitTime = waitTime,
                currentTime = CurrentTime,
                methodID = methodID
            };
            timeWaiters.Add(waitEvent);

            return waitEvent;
        }

        public void UnregisterTimeWaiter(TimeWaitEvent ev) => timeWaiters.Remove(ev);

        public TimeCountEvent StartTimeCounter()
        {
            TimeCountEvent waitEvent = new TimeCountEvent()
            {
                hash = Guid.NewGuid(),
            };
            timeCounters.Add(waitEvent);

            return waitEvent;
        }

        public TimeSpan ContinueTimeCounter(Guid hash)
        {
            TimeCountEvent ev = GetTCEFromGUID(hash);
            ev.isPaused = false;
            return ev.timePassed;
        }

        public TimeSpan GetTCETimeSpan(Guid hash)
        {
            TimeCountEvent ev = GetTCEFromGUID(hash);
            return ev.timePassed;
        }

        public TimeSpan PauseTimeCounter(Guid hash)
        {
            TimeCountEvent ev = GetTCEFromGUID(hash);
            ev.isPaused = true;
            return ev.timePassed;
        }

        public TimeSpan StopTimeCounter(Guid hash)
        {
            TimeCountEvent ev = GetTCEFromGUID(hash);
            timeCounters.Remove(ev);
            return ev.timePassed;
        }

        private TimeCountEvent GetTCEFromGUID(Guid hash)
        {
            foreach (TimeCountEvent ev in timeCounters)
            {
                if (ev.hash == hash)
                    return ev;
            }
            return null;
        }

        public string GetReadableHourTime()
        {
            return CurrentTime.ToString("t", CurrentCulture);
        }

        public string GetReadableDateTime()
        {
            return CurrentTime.ToString("d MMM yyyy", CurrentCulture);
        }
    }
}
