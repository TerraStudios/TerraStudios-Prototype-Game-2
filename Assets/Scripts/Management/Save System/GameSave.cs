using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSave
{
    private static GameSave _current;
    public static GameSave current
    {
        get
        {
            if (_current == null)
                _current = new GameSave();

            return _current;
        }
    }

    public TimeSaveData TimeSaveData;
}
