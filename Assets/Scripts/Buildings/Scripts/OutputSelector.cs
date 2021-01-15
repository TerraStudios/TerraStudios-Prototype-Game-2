//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using RecipeManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingManagement
{
    /// <summary>
    /// This script handles output selection for Buildings through the UI.
    /// </summary>
    public class OutputSelector : MonoBehaviour
    {
        // Dictionary values
        [HideInInspector] public MachineRecipe.OutputData value;
        private int outputID;

        [Header("UI Components")]
        public TMP_Text itemNameText;
        public Button button;
        public TMP_Text buttonText;

        public int OutputID
        {
            get => outputID;
            set
            {
                outputID = value;
                buttonText.text = "Output " + value;
            }
        }

        private void Awake()
        {
            button.onClick.AddListener(OnChangeOutputIDButtonClicked);
        }

        private void OnChangeOutputIDButtonClicked()
        {
            BuildingManager.Instance.focusedBuilding.mc.apm.OnOutputButtonPressed(this);
        }
    }
}
