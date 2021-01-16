//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using TimeSystem;

namespace SaveSystem
{
    /// <summary>
    /// Contains data about the time simulation.
    /// </summary>
    [Serializable]
    public class TimeSaveData
    {
        public bool isPaused;
        public int timeMultiplier = 1;
        public DateTime currentTime;
        public List<TimeWaitEvent> timeWaiters = new List<TimeWaitEvent>();
        public List<TimeCountEvent> timeCounters = new List<TimeCountEvent>();
    }
}
