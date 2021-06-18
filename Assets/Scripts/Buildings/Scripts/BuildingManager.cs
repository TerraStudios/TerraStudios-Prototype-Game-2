//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using BuildingModules;
using ItemManagement;
using RecipeManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utilities;

namespace BuildingManagement
{
    /// <summary>
    /// This class is the highest level of the <c>BuildingSystem</c>.
    /// It handles connections with the UI along with references to GameObjects.
    /// </summary>
    public class BuildingManager : BuildingSystem
    {
        public static ItemData testItemToSpawn;
        public bool enableDebugSpawn;
        public void EnableDebugSpawn(bool value) => enableDebugSpawn = value;
        [Header("BuildingInfo UI Components")]
        public GameObject buildingInfo;
        public TMP_Text buildHealth;
        public TMP_Text itemInsideName;

        [Header("IO and Recipe Setup UI Components")]
        public TMP_Dropdown recipeSelection;
        public GameObject ioSelectionPanel;
        public Transform inputsParent;
        public Transform inputsSelector;
        public Transform outputsParent;
        public Transform outputsSelector;
        private List<Transform> inputSelectorFields = new List<Transform>();
        private List<Transform> outputSelectorFields = new List<Transform>();

        [Header("Building Indicators")]
        public Transform directionIndicator;
        public Transform arrowIndicator;
        public Transform brokenIndicator;
        public Transform errorIndicator;
        public Transform fixingIndicator;

        [Header("Color Indicators")]
        public Material blueArrow;
        public Material greenArrow;
        public Material redArrow;

        public static BuildingManager Instance;

        //! Probably has to be moved to BuildingSystem since this script should only handle UI
        public void Awake()
        {
            Instance = this;

            //Create pools for each indicator
            ObjectPoolManager.Instance.CreatePool(directionIndicator.gameObject, 80);
            ObjectPoolManager.Instance.CreatePool(arrowIndicator.gameObject, 8);
            ObjectPoolManager.Instance.CreatePool(brokenIndicator.gameObject, 50);
            ObjectPoolManager.Instance.CreatePool(errorIndicator.gameObject, 50);
            ObjectPoolManager.Instance.CreatePool(fixingIndicator.gameObject, 50);

            ClearRegisteredBuildings();
            PoolAllBuildingMeshes();
            LoadAllBuildingsFromSave();

            buildingScriptParent = buildingScriptParent ? buildingScriptParent : (buildingScriptParent = new GameObject("Building GO Scripts"));
            buildingMeshParent = buildingMeshParent ? buildingMeshParent : (buildingMeshParent = new GameObject("Building GO Meshes"));
        }

        //Static because the building manager doesn't have access to BuildingManager, and it doesn't make sense to put it in BuildingIOManager (multiple instances)

        /// <summary>
        /// Event called when the building is selected, sets the UI to active and calls the OnBuildingSelected event
        /// </summary>
        public override void OnBuildingSelected(Building b)
        {
            base.OnBuildingSelected(b);

            b.mc.buildingIOManager.UpdateArrows();

            buildingInfo.SetActive(true);

            RefreshRecipeList();
            RefreshIOUI();

            if (b.mc.buildingIOManager.isConveyor && enableDebugSpawn)
                b.mc.buildingIOManager.AttemptItemEnter(testItemToSpawn, 0, null, null);
        }

        /// <summary>
        /// Event called when the building is deselected, sets the UI to inactive and calls the OnBuildingDeselected event
        /// </summary>
        public override void OnBuildingDeselected()
        {
            if (!focusedBuilding)
                return;
            focusedBuilding.mc.buildingIOManager.DestroyArrows();

            foreach (List<KeyValuePair<Building, GameObject>> kvp in PlacedBuildings.Values)
                foreach (KeyValuePair<Building, GameObject> buildingKVP in kvp)
                {
                    buildingKVP.Key.mc.buildingIOManager.DestroyArrows();
                }

            base.OnBuildingDeselected();
            buildingInfo.SetActive(false);
            ioSelectionPanel.SetActive(false);
        }

        /// <summary>
        /// Updates the Building UI with all of the correct information
        /// </summary>
        public override void OnBuildingUpdateUI()
        {
            base.OnBuildingUpdateUI();
            if (focusedBuilding)
            {
                buildHealth.text = focusedBuilding.bBase.healthPercent.ToString();
                //onTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.On).Duration().ToString();
                //idleTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.Idle).Duration().ToString();
                //offTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.Off).Duration().ToString();
                //itemInsideName.text = FocusedBuilding.BuildingIOManager.GetItemInsideName();
            }
        }

        /// <summary>
        /// Event called when the fix button is pressed, checks if the health is below optimal standards and tries to fix it
        /// </summary>
        public void OnFixButtonPressed()
        {
            if (focusedBuilding.bBase.healthPercent != 100)
            {
                focusedBuilding.Fix();
            }
        }

        /// <summary>
        /// Sets the appropriate WorkState of the machine based off of button input
        /// </summary>
        /// <param name="buttonID">The button pressed</param>
        public void OnStateButtonPressed(int buttonID)
        {
            switch (buttonID)
            {
                case 1:
                    focusedBuilding.WorkState = WorkStateEnum.On;
                    break;
                case 2:
                    focusedBuilding.WorkState = WorkStateEnum.Idle;
                    break;
                case 3:
                    focusedBuilding.WorkState = WorkStateEnum.Off;
                    break;
            }
        }

        #region IO Management

        private void RefreshRecipeList()
        {
            recipeSelection.ClearOptions();

            recipeSelection.options.Add(new TMP_Dropdown.OptionData() { text = "None" });

            if (focusedBuilding.mc.buildingIOManager.isConveyor)
            {
                recipeSelection.value = 0;
                recipeSelection.RefreshShownValue();
                return;
            }

            if (!focusedBuilding.mc.apm.CurrentRecipe)
            {
                recipeSelection.value = 0;
            }

            for (int i = 0; i < focusedBuilding.mc.apm.allowedRecipes.Count; i++)
            {
                MachineRecipe recipe = focusedBuilding.mc.apm.allowedRecipes[i];
                recipeSelection.options.Add(new TMP_Dropdown.OptionData() { text = recipe.name });
                if (focusedBuilding.mc.apm.CurrentRecipe == recipe)
                    recipeSelection.value = i + 1;
            }

            recipeSelection.RefreshShownValue();

            recipeSelection.onValueChanged.AddListener(delegate { OnRecipeSelected(recipeSelection); });
        }

        private void OnRecipeSelected(TMP_Dropdown changed)
        {
            if (changed.value == 0)
                focusedBuilding.mc.apm.CurrentRecipe = null;
            else
                focusedBuilding.mc.apm.CurrentRecipe = focusedBuilding.mc.apm.allowedRecipes[changed.value - 1];

            RefreshIOUI();
        }

        public void ShowIOSelectionUI() => RefreshIOUI(true);

        public void HideIOSelectionUI()
        {
            ioSelectionPanel.SetActive(false);
        }

        private void RefreshIOUI(bool showPanel = false)
        {
            if (!IsOutputSetupSupported())
                return;

            if (showPanel)
                ioSelectionPanel.SetActive(true);

            // visualize the inputData from APM to the UI

            foreach (Transform field in inputSelectorFields)
            {
                Destroy(field.gameObject);
            }

            inputSelectorFields.Clear();

            for (int i = 0; i < focusedBuilding.mc.apm.inputData.Count; i++)
            {
                KeyValuePair<MachineRecipe.InputData, int> entry = focusedBuilding.mc.apm.inputData.ElementAt(i);

                Transform fieldToAdd = Instantiate(inputsSelector, inputsParent);
                InputSelector os = fieldToAdd.GetComponent<InputSelector>();
                os.value = entry.Key;
                os.InputID = entry.Value;
                os.itemNameText.text = entry.Key.item.name;
                inputSelectorFields.Add(fieldToAdd);

                if (!focusedBuilding.mc.apm.CurrentRecipe.allowPlayerInputsConfiguration)
                    os.button.interactable = false;
            }

            // visualize the outputsData from APM in the UI

            foreach (Transform field in outputSelectorFields)
            {
                Destroy(field.gameObject);
            }

            outputSelectorFields.Clear();

            for (int i = 0; i < focusedBuilding.mc.apm.outputData.Count; i++)
            {
                KeyValuePair<MachineRecipe.OutputData, int> entry = focusedBuilding.mc.apm.outputData.ElementAt(i);

                Transform fieldToAdd = Instantiate(outputsSelector, outputsParent);
                OutputSelector os = fieldToAdd.GetComponent<OutputSelector>();
                os.value = entry.Key;
                os.OutputID = entry.Value;
                os.itemNameText.text = entry.Key.item.name;
                outputSelectorFields.Add(fieldToAdd);

                if (!focusedBuilding.mc.apm.CurrentRecipe.allowPlayerOutputsConfiguration)
                    os.button.interactable = false;
            }
        }

        private bool IsOutputSetupSupported()
        {
            return focusedBuilding && focusedBuilding.mc.apm && !focusedBuilding.mc.conveyor;
        }
        #endregion
    }
}
