using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public struct TimeWaitEvent 
{
    public DateTime currentTime;
    public TimeSpan waitTime;
    public UnityEvent ev;
}

public class TimeCountEvent
{
    public Guid hash;
    public bool isPaused;
    public TimeSpan timePassed;
}

public class TimeSystem : TimeEngine
{
    [HideInInspector] public List<TimeWaitEvent> timeWaiters = new List<TimeWaitEvent>();
    [HideInInspector] public List<TimeCountEvent> timeCounters = new List<TimeCountEvent>();
    [HideInInspector] public CultureInfo CurrentCulture { get { return GameManager.currentCulture; } }

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
                ev.ev.Invoke();
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

    public TimeWaitEvent RegisterTimeWaiter(TimeSpan waitTime, UnityEvent methodToCall)
    {
        TimeWaitEvent waitEvent = new TimeWaitEvent()
        {
            waitTime = waitTime,
            currentTime = CurrentTime,
            ev = methodToCall
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

    public string GetReadableTime()
    {
        CultureInfo EuropeStandard = CurrentCulture;
        return CurrentTime.ToString("g", EuropeStandard);
    }
}
