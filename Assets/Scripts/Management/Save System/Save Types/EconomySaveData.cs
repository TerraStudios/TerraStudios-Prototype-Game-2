using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// Contains data from the economy systems.
/// </summary>
[Serializable]
public class EconomySaveData
{
    public decimal balanace;
    public DateTime lastBankruptcyStart;
    public DateTime lastBankruptcyEnd;
    public List<TimeWaitEvent> bankruptcyTimers = new List<TimeWaitEvent>();
}