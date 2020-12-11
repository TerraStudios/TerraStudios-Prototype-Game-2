﻿using RecipeManagement;
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
        private int inputID;

        [Header("UI Components")]
        public TMP_Text itemNameText;
        public Button button;
        public TMP_Text buttonText;

        public int InputID
        {
            get => inputID;
            set
            {
                inputID = value;
                if (value == -1)
                    buttonText.text = "Input Any";
                else
                    buttonText.text = "Input " + value;
            }
        }

        private void Awake()
        {
            button.onClick.AddListener(OnChangeInputIDButtonClicked);
        }

        private void OnChangeInputIDButtonClicked()
        {
            BuildingManager.Instance.focusedBuilding.mc.apm.OnInputButtonPressed(this);
        }
    }
}
