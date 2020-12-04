﻿/// <summary>
/// Contains all variables that are stored inside a game save.
/// </summary>
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
        set
        {
            if (value != null)
            {
                _current = value;
            }
        }
    }

    public TimeSaveData TimeSaveData = new TimeSaveData();
    public EconomySaveData EconomySaveData = new EconomySaveData();
    public WorldSaveData WorldSaveData = new WorldSaveData();
}