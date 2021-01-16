//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

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
