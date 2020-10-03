using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimeSaveData
{
    public bool isPaused;
    public int timeMultiplier = 1;
    public DateTime currentTime;
}
