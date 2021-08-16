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

using UnityEngine;

namespace BuildingModules
{
    /// <summary>
    /// This class acts like a "hub" for all scripts in the Building GameObject.
    /// It is useful to reference this module and access every script available for this Building from here.
    /// </summary>
    public class ModuleConnector : MonoBehaviour
    {
        [Header("Required")]
        [Tooltip("The building the ModuleConnector is attached to")]
        public Building building;

        [Header("Optional")]
        [Tooltip("The BuildingIOManager script of a Building")]
        public BuildingIOManager buildingIOManager;
        [Tooltip("The Conveyor movement script of a Building")]
        public Conveyor conveyor;
        [Tooltip("The APM script of a Building")]
        public APM apm;
    }
}
