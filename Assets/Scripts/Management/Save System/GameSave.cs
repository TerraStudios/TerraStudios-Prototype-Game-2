using UnityEngine.Serialization;

/// <summary>
/// Contains all variables that are stored inside a game save.
/// </summary>
[System.Serializable]
public class GameSave
{
    private static GameSave Current;

    public static GameSave current
    {
        get
        {
            if (Current == null)
                Current = new GameSave();

            return Current;
        }
        set
        {
            if (value != null)
            {
                Current = value;
            }
        }
    }

    public TimeSaveData timeSaveData = new TimeSaveData();
    public EconomySaveData economySaveData = new EconomySaveData();
    public WorldSaveData worldSaveData = new WorldSaveData();
}