//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
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
