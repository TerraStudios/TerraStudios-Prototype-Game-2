using System;
using System.Collections.Generic;

/// <summary>
/// Contains data from the economy systems.
/// </summary>
[Serializable]
public class EconomySaveData
{
    public decimal balanace;
    public DateTime LastBankruptcyStart;
    public DateTime LastBankruptcyEnd;
    public List<TimeWaitEvent> bankruptcyTimers = new List<TimeWaitEvent>();
}