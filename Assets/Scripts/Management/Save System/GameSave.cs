using CoreManagement;
using System;

namespace SaveSystem
{
    /// <summary>
    /// Contains all variables that are stored inside a game save.
    /// </summary>
    [Serializable]
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

        public GameProfileData gameProfileData;
        public UserProfileData userProfileData;

        public TimeSaveData timeSaveData = new TimeSaveData();
        public EconomySaveData economySaveData = new EconomySaveData();
        public WorldSaveData worldSaveData = new WorldSaveData();
    }
}
