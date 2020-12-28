using System;
using System.Diagnostics;

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
