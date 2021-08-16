//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
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
