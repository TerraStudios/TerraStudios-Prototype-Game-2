//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using RecipeManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingManagement
{
    /// <summary>
    /// This script handles input selection for Buildings through the UI.
    /// </summary>
    public class InputSelector : MonoBehaviour
    {
        // Dictionary values
        [HideInInspector] public MachineRecipe.InputData value;
        private int inputId;

        [Header("UI Components")]
        public TMP_Text itemNameText;
        public Button button;
        public TMP_Text buttonText;

        public int InputId
        {
            get => inputId;
            set
            {
                inputId = value;
                if (value == -1)
                    buttonText.text = "Input Any";
                else
                    buttonText.text = "Input " + value;
            }
        }

        private void Start()
        {
            button.onClick.AddListener(OnChangeInputIdButtonClicked);
        }

        private void OnChangeInputIdButtonClicked()
        {
            BuildingManager.Instance.focusedBuilding.mc.apm.OnInputButtonPressed(this);
        }
    }
}
