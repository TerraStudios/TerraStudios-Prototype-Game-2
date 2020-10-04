﻿using System;
using System.Collections.Generic;

[Serializable]
public class TimeSaveData
{
    public bool isPaused;
    public int timeMultiplier = 1;
    public DateTime currentTime;
    public List<TimeWaitEvent> timeWaiters = new List<TimeWaitEvent>();
    public List<TimeCountEvent> timeCounters = new List<TimeCountEvent>();
}
