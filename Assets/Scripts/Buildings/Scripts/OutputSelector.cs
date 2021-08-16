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
        private int outputId;

        [Header("UI Components")]
        public TMP_Text itemNameText;
        public Button button;
        public TMP_Text buttonText;

        public int OutputId
        {
            get => outputId;
            set
            {
                outputId = value;
                buttonText.text = "Output " + value;
            }
        }

        private void Start()
        {
            button.onClick.AddListener(OnChangeOutputIdButtonClicked);
        }

        private void OnChangeOutputIdButtonClicked()
        {
            BuildingManager.Instance.focusedBuilding.mc.apm.OnOutputButtonPressed(this);
        }
    }
}
