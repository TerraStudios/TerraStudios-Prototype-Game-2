using System;
using System.Collections.Generic;
using TimeSystem;

namespace SaveSystem
{
    /// <summary>
    /// Contains data from the economy systems.
    /// </summary>
    [Serializable]
    public class EconomySaveData
    {
        public decimal balance;
        public DateTime lastBankruptcyStart;
        public DateTime lastBankruptcyEnd;
        public List<TimeWaitEvent> bankruptcyTimers = new List<TimeWaitEvent>();
    }
}
