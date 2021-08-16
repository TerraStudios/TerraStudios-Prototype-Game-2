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
using CoreManagement;

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
