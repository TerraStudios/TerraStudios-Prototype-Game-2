//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

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
