//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingManagement;
using CoreManagement;
using ItemManagement;
using RecipeManagement;
using UnityEngine;
using Utilities;

namespace BuildingModules
{
    public enum APMStatus { Idle, Blocked, Crafting }

    /// <summary>
    /// This class contains data about the current crafting process.
    /// </summary>
    public class CraftingData
    {
        public MachineRecipe currentRecipe;
        public int inputID;
        public int outputID;
    }

    /// <summary>
    /// APM (Advanced Processing Machine) handles a complex of tasks mostly related to crafting.
    /// </summary>
    public class APM : MonoBehaviour
    {
        public ModuleConnector mc;
        public RecipeFilter recipeFilter;
        public List<MachineRecipe> allowedRecipes = new List<MachineRecipe>();
        private MachineRecipe currentRecipe;
        public float baseTimeMultiplier = 1;
        public int inputSpace;
        public int outputSpace;

        private APMStatus currentStatus;

        // Key is the needed recipe item
        // Value is inputID
        [HideInInspector]
        public Dictionary<MachineRecipe.InputData, int> inputData = new Dictionary<MachineRecipe.InputData, int>();

        // Key is the needed recipe item
        // Value is outputID
        [HideInInspector]
        public Dictionary<MachineRecipe.OutputData, int> outputData = new Dictionary<MachineRecipe.OutputData, int>();

        public Queue<CraftingData> currentlyCrafting = new Queue<CraftingData>();

        public MachineRecipe CurrentRecipe
        {
            get => currentRecipe;
            set
            {
                if (CurrentRecipe == value)
                    return;

                currentRecipe = value;
                InitIOData();

                if (value)
                    Debug.Log("Set recipe: " + value.name, this);
                else
                    Debug.Log("Set recipe null", this);

                if (!value)
                    CurrentStatus = APMStatus.Blocked;
                else
                    CurrentStatus = APMStatus.Idle;
            }
        }

        public APMStatus CurrentStatus
        {
            get => currentStatus;
            set
            {
                /*if (value == APMStatus.Crafting)
                    mc.BuildingIOManager.SetConveyorGroupState(WorkStateEnum.Off);

                else
                    mc.BuildingIOManager.SetConveyorGroupState(WorkStateEnum.On);*/

                currentStatus = value;
            }
        }

        public void Init()
        {
            mc.buildingIOManager.onItemEnterInput.AddListener(OnItemEnterInput);

            if (CurrentRecipe)
                InitIOData();
            else
                CurrentStatus = APMStatus.Blocked;

            allowedRecipes = RecipeManager.GetRecipes(recipeFilter).allowed;
        }

        private void InitIOData()
        {
            inputData.Clear();
            outputData.Clear();

            if (CurrentRecipe)
            {
                int buildingInputs = mc.buildingIOManager.inputs.Length;
                foreach (MachineRecipe.InputBatch data in CurrentRecipe.inputs)
                {
                    for (int i = 0; i < data.inputs.Length; i++)
                    {
                        int inputIDToApply = data.inputs[i].inputID + 1;
                        if (data.inputs[i].inputID == -1)
                        {
                            inputIDToApply = -1;
                        }
                        else
                        {
                            if (inputIDToApply >= buildingInputs)
                                inputIDToApply = buildingInputs;
                        }

                        inputData.Add(data.inputs[i], inputIDToApply);
                    }
                }

                int buildingOutputs = mc.buildingIOManager.outputs.Length;
                foreach (MachineRecipe.OutputBatch data in CurrentRecipe.outputs)
                {
                    for (int i = 0; i < data.outputs.Length; i++)
                    {
                        int outputIDToApply = data.outputs[i].outputID + 1;
                        if (outputIDToApply >= buildingOutputs)
                            outputIDToApply = buildingOutputs;

                        outputData.Add(data.outputs[i], outputIDToApply);
                    }
                }
            }
        }

        public void OnInputButtonPressed(InputSelector caller)
        {
            int newInputID = caller.InputID + 1;

            if (caller.InputID == -1)
                return;
            else if (newInputID <= mc.buildingIOManager.inputs.Length)
                inputData[caller.value] = newInputID;
            else
            {
                newInputID = 1;
                inputData[caller.value] = newInputID;
            }

            caller.InputID = newInputID;
        }

        public void OnOutputButtonPressed(OutputSelector caller)
        {
            int newOutputID = caller.OutputID + 1;

            if (newOutputID <= mc.buildingIOManager.outputs.Length)
                outputData[caller.value] = newOutputID;
            else
            {
                newOutputID = 1;
                outputData[caller.value] = newOutputID;
            }

            caller.OutputID = newOutputID;
        }

        private void OnItemEnterInput(OnItemEnterEvent ItemEnterInfo)
        {
            if (IsAllowedToEnter(ItemEnterInfo))
                AcceptItemInside(ItemEnterInfo);
            else
                return;

            (bool, int, int) recipeInputInfo = IsAllowedToStartCrafting(ItemEnterInfo);

            if (recipeInputInfo.Item1)
                StartCrafting(recipeInputInfo.Item2, recipeInputInfo.Item3);
        }

        public bool IsAllowedToEnter(OnItemEnterEvent ItemEnterInfo)
        {
            if (!CurrentRecipe) // check if we have any recipe to work with
            {
                ItemLog(ItemEnterInfo.item.name, "Item attempts to enter but there's no recipe!!!", this);
                return false;
            }

            // Get the InputsData that expect this item to come in
            List<MachineRecipe.InputBatch> recipeData = CurrentRecipe.inputs.FindAll(data =>
            {
                foreach (MachineRecipe.InputData inputData in data.inputs)
                {
                    if (inputData.item.id == ItemEnterInfo.item.id)
                    {
                        if (inputData.inputID != -1)
                        {
                            if (inputData.inputID != ItemEnterInfo.inputID)
                                return false;
                            else
                                return true;
                        }
                        else if (inputData.inputID == -1)
                        {
                            return true;
                        }
                    }
                }

                return false;
            });

            // Check if there's any allowed InputsData, if none, the item shouldn't be allowed to enter
            if (recipeData.Count == 0)
            {
                Debug.LogWarning("Didn't find any item that fits the InputData of this recipe!");
                return false;
            }

            // storage check here
            if (!IsInputStorageSufficient(ItemEnterInfo))
            {
                Debug.LogWarning("Input storage insufficient!");
                return false;
            }

            return true;
        }

        private (bool, int, int) IsAllowedToStartCrafting(OnItemEnterEvent ItemEnterInfo)
        {
            int inputID = 0;
            int outputID = 0;

            /*// check if the outputs' queues have enough space to fit the output items
            foreach (KeyValuePair<MachineRecipe.OutputData, int> kvp in outputData)
            {
                //BuildingIO io = mc.buildingIOManager.outputs[kvp.Value - 1];
                if (mc.buildingIOManager.itemsToSpawn.Count + kvp.Key.amount > outputSpace)
                {
                    ItemLog(ItemEnterInfo.item.name, "Not enough space to one or more of the output/s", this);
                    mc.building.SetIndicator(BuildingManager.Instance.errorIndicator);
                    return (false, inputID, outputID);
                }
            }*/

            mc.building.RemoveIndicator();

            List<MachineRecipe.InputBatch> recipeData = CurrentRecipe.inputs.FindAll(data =>
            {
                foreach (MachineRecipe.InputData inputData in data.inputs)
                {
                    ItemData recipeItem = inputData.item;

                    // check if itemsInside contains the item needed from the recipe
                    if (!mc.buildingIOManager.itemsInside.ContainsKey(recipeItem))
                    {
                        //A required item type is missing from itemsInside!
                        return false;
                    }

                    // check if we have the enough quantity of it available to start crafting
                    if (mc.buildingIOManager.itemsInside[recipeItem] < inputData.amount)
                    {
                        //Still, not all items are present inside
                        return false;
                    }

                    inputID = CurrentRecipe.inputs.FindIndex(id => id == data);
                    outputID = data.outputListID;
                }

                return true;
            });

            if (recipeData.Count == 1)
            {
                Debug.Log("Starting crafting with inputID: " + inputID + " outputID: " + outputID);
                return (true, inputID, outputID);
            }
            else
            {
                return (false, inputID, outputID);
            }
        }

        private bool IsInputStorageSufficient(OnItemEnterEvent ItemEnterInfo)
        {
            // If the items that attempts to enter has a quantity larger that the allowed
            if (mc.buildingIOManager.itemsInside.FirstOrDefault(kvp => kvp.Key == ItemEnterInfo.item).Value == inputSpace)
            {
                ItemLog(ItemEnterInfo.item.name, "There's not enough input space for this item!", this);
                //mc.Building.SetIndicator(BuildingManager.instance.ErrorIndicator);
                return false;
            }

            mc.building.RemoveIndicator();
            return true;
        }

        private bool IsOutputStorageSufficient()
        {
            // Loop all recipe outputs
            // Check if the BuildingIO corresponding to the Recipe output has enough space

            CraftingData currentlyCrafting = this.currentlyCrafting.Peek();

            foreach (MachineRecipe.OutputData data in currentlyCrafting.currentRecipe.outputs[currentlyCrafting.outputID].outputs) // get items needed to be ejected
            {
                // find their corresponding outputID
                KeyValuePair<MachineRecipe.OutputData, int> kvp = new KeyValuePair<MachineRecipe.OutputData, int>(data, outputData[data]);

                for (int t = 0; t < kvp.Key.amount; t++)
                {
                    int outputIDToCheck = kvp.Value - 1;

                    // Check the output space based on the items inside
                    int total = mc.buildingIOManager.outputs[outputIDToCheck].itemsToSpawn.GroupBy(_ => _.item).Where(_ => _.Count() > 1).Sum(_ => _.Count());
                    if (total >= outputSpace)
                    {
                        Debug.LogWarning("Output " + (kvp.Value - 1) + " is full!");
                        return false;
                    }
                }
            }

            return true;
        }

        #region Crafting procedure

        private void StartCrafting(int inputID, int outputID)
        {
            Debug.Log("Start crafting!", this);

            CraftingData data = new CraftingData()
            {
                inputID = inputID,
                outputID = outputID,
                currentRecipe = currentRecipe
            };

            currentlyCrafting.Enqueue(data);

            Debug.Log("Enqueued new crafting.");

            if (currentlyCrafting.Count > 1)
            {
                Debug.Log("Looks like APM is currently crafting something... Will wait my turn.");
                return;
            }

            Debug.Log("APM is ready to craft this item! Proceeding...");
            CurrentStatus = APMStatus.Crafting;
            ProcessToCraft();
        }

        private void ProcessToCraft()
        {
            CraftingData data = currentlyCrafting.Peek();
            foreach (MachineRecipe.InputData toRemove in data.currentRecipe.inputs[data.inputID].inputs)
            {
                mc.buildingIOManager.RemoveItem(toRemove.item);
            }

            StartCoroutine(RunCraftingTimer());
        }

        private IEnumerator RunCraftingTimer()
        {
            yield return new WaitForSeconds(currentlyCrafting.Peek().currentRecipe.baseTime * baseTimeMultiplier * GameManager.Instance.CurrentGameProfile.globalBaseTimeMultiplier);
            yield return new WaitUntil(IsOutputStorageSufficient);
            ExecuteCrafting();
        }

        private void ExecuteCrafting()
        {
            CraftingData currentlyCrafting = this.currentlyCrafting.Peek();

            foreach (MachineRecipe.OutputData data in currentlyCrafting.currentRecipe.outputs[currentlyCrafting.outputID].outputs) // get items needed to be ejected
            {
                // find their corresponding outputID
                KeyValuePair<MachineRecipe.OutputData, int> kvp = new KeyValuePair<MachineRecipe.OutputData, int>(data, outputData[data]);

                for (int t = 0; t < kvp.Key.amount; t++)
                {
                    Debug.Log("Spawning on outputID" + (kvp.Value - 1));
                    mc.buildingIOManager.EjectItem(kvp.Key.item, kvp.Value - 1, false);
                }
            }

            mc.building.RemoveIndicator();
            CurrentStatus = APMStatus.Idle;

            this.currentlyCrafting.Dequeue();

            if (this.currentlyCrafting.Count > 0)
            {
                Debug.Log("Finished crafting! Processing the next crafting data in the queue...");
                ProcessToCraft();
            }
        }

        private void AcceptItemInside(OnItemEnterEvent ItemEnterInfo)
        {
            if (ItemEnterInfo.sceneInstance)
                ObjectPoolManager.Instance.DestroyObject(ItemEnterInfo.sceneInstance);

            if (ItemEnterInfo.caller.mc.conveyor)
                ItemEnterInfo.caller.mc.conveyor.RemoveItemFromBelt(ItemEnterInfo.sceneInstance);

            mc.buildingIOManager.AddItem(ItemEnterInfo.item);
        }

        #endregion

        private void ItemLog(string itemName, string message, Object highlight = null)
        {
            if (CurrentRecipe)
                Debug.Log($"[Recipe: {CurrentRecipe.name}] [Item: {itemName}] {message}", highlight);
            else
                Debug.Log($"[Recipe: None] [Item: {itemName}] {message}", highlight);
        }
    }
}
