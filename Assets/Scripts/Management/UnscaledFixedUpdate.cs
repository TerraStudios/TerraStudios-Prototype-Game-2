//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Diagnostics;

namespace CoreManagement
{
    public class UnscaledFixedUpdate
    {
        private float referenceTime = 0;
        private float fixedTime = 0;
        private Action fixedUpdate;
        private Stopwatch timeout = new Stopwatch();

        public UnscaledFixedUpdate(float aFixedDeltaTime, Action aFixecUpdateCallback)
        {
            FixedDeltaTime = aFixedDeltaTime;
            fixedUpdate = aFixecUpdateCallback;
        }

        public bool Update(float aDeltaTime)
        {
            timeout.Reset();
            timeout.Start();

            referenceTime += aDeltaTime;
            while (fixedTime < referenceTime)
            {
                fixedTime += FixedDeltaTime;
                fixedUpdate?.Invoke();
                if ((timeout.ElapsedMilliseconds / 1000.0f) > MaxAllowedTimestep)
                    return false;
            }
            return true;
        }

        public float FixedDeltaTime { get; set; }
        public float MaxAllowedTimestep { get; set; } = 0.3f;
        public float ReferenceTime
        {
            get { return referenceTime; }
        }
        public float FixedTime
        {
            get { return fixedTime; }
        }
    }
}
