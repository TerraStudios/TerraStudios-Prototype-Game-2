using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

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

public class TimeSystem : TimeEngine
{
    [HideInInspector] public CultureInfo CurrentCulture { get { return GameManager.instance.currentCultureTimeDate; } }

    public List<TimeWaitEvent> TimeWaiters { get => GameSave.current.TimeSaveData.timeWaiters; set => GameSave.current.TimeSaveData.timeWaiters = value; }
    public List<TimeCountEvent> TimeCounters { get => GameSave.current.TimeSaveData.timeCounters; set => GameSave.current.TimeSaveData.timeCounters = value; }

    public void StartCounting(DateTime? startTime)
    {
        if (startTime.Value != null)
            CurrentTime = startTime.Value;
        StartClock();
    }

    public override void OnCounterTick()
    {
        for (int i = 0; i < TimeWaiters.Count; i++)
        {
            TimeWaitEvent ev = TimeWaiters[i];
            if (CurrentTime - ev.currentTime >= ev.waitTime)
            {
                CallbackHandler.instance.GetEvent(ev.methodID).Invoke();
                TimeWaiters.Remove(ev);
                break;
            }
        }

        for (int i = 0; i < TimeCounters.Count; i++)
        {
            TimeCountEvent ev = TimeCounters[i];
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
        TimeWaiters.Add(waitEvent);

        return waitEvent;
    }

    public void UnregisterTimeWaiter(TimeWaitEvent ev) => TimeWaiters.Remove(ev);

    public TimeCountEvent StartTimeCounter()
    {
        TimeCountEvent waitEvent = new TimeCountEvent()
        {
            hash = Guid.NewGuid(),
        };
        TimeCounters.Add(waitEvent);

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
        TimeCounters.Remove(ev);
        return ev.timePassed;
    }

    private TimeCountEvent GetTCEFromGUID(Guid hash)
    {
        foreach (TimeCountEvent ev in TimeCounters)
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
