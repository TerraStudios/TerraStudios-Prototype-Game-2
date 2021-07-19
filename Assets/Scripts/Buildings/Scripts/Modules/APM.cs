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
        public int inputId;
        public int outputId;
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
        // Value is inputId
        [HideInInspector]
        public Dictionary<MachineRecipe.InputData, int> inputData = new Dictionary<MachineRecipe.InputData, int>();

        // Key is the needed recipe item
        // Value is outputId
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
                        int inputIdToApply = data.inputs[i].inputId + 1;
                        if (data.inputs[i].inputId == -1)
                        {
                            inputIdToApply = -1;
                        }
                        else
                        {
                            if (inputIdToApply >= buildingInputs)
                                inputIdToApply = buildingInputs;
                        }

                        inputData.Add(data.inputs[i], inputIdToApply);
                    }
                }

                int buildingOutputs = mc.buildingIOManager.outputs.Length;
                foreach (MachineRecipe.OutputBatch data in CurrentRecipe.outputs)
                {
                    for (int i = 0; i < data.outputs.Length; i++)
                    {
                        int outputIdToApply = data.outputs[i].outputId + 1;
                        if (outputIdToApply >= buildingOutputs)
                            outputIdToApply = buildingOutputs;

                        outputData.Add(data.outputs[i], outputIdToApply);
                    }
                }
            }
        }

        public void OnInputButtonPressed(InputSelector caller)
        {
            int newInputId = caller.InputId + 1;

            if (caller.InputId == -1)
                return;
            else if (newInputId <= mc.buildingIOManager.inputs.Length)
                inputData[caller.value] = newInputId;
            else
            {
                newInputId = 1;
                inputData[caller.value] = newInputId;
            }

            caller.InputId = newInputId;
        }

        public void OnOutputButtonPressed(OutputSelector caller)
        {
            int newOutputId = caller.OutputId + 1;

            if (newOutputId <= mc.buildingIOManager.outputs.Length)
                outputData[caller.value] = newOutputId;
            else
            {
                newOutputId = 1;
                outputData[caller.value] = newOutputId;
            }

            caller.OutputId = newOutputId;
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
                        if (inputData.inputId != -1)
                        {
                            if (inputData.inputId != ItemEnterInfo.inputId)
                                return false;
                            else
                                return true;
                        }
                        else if (inputData.inputId == -1)
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
            int inputId = 0;
            int outputId = 0;

            /*// check if the outputs' queues have enough space to fit the output items
            foreach (KeyValuePair<MachineRecipe.OutputData, int> kvp in outputData)
            {
                //BuildingIO io = mc.buildingIOManager.outputs[kvp.Value - 1];
                if (mc.buildingIOManager.itemsToSpawn.Count + kvp.Key.amount > outputSpace)
                {
                    ItemLog(ItemEnterInfo.item.name, "Not enough space to one or more of the output/s", this);
                    mc.building.SetIndicator(BuildingManager.Instance.errorIndicator);
                    return (false, inputId, outputId);
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

                    inputId = CurrentRecipe.inputs.FindIndex(id => id == data);
                    outputId = data.outputListId;
                }

                return true;
            });

            if (recipeData.Count == 1)
            {
                Debug.Log("Starting crafting with inputId: " + inputId + " outputId: " + outputId);
                return (true, inputId, outputId);
            }
            else
            {
                return (false, inputId, outputId);
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

            foreach (MachineRecipe.OutputData data in currentlyCrafting.currentRecipe.outputs[currentlyCrafting.outputId].outputs) // get items needed to be ejected
            {
                // find their corresponding outputIds
                KeyValuePair<MachineRecipe.OutputData, int> kvp = new KeyValuePair<MachineRecipe.OutputData, int>(data, outputData[data]);

                for (int t = 0; t < kvp.Key.amount; t++)
                {
                    int outputIdToCheck = kvp.Value - 1;

                    // Check the output space based on the items inside
                    int total = mc.buildingIOManager.outputs[outputIdToCheck].itemsToSpawn.GroupBy(_ => _.item).Where(_ => _.Count() > 1).Sum(_ => _.Count());
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

        private void StartCrafting(int inputId, int outputId)
        {
            Debug.Log("Start crafting!", this);

            CraftingData data = new CraftingData()
            {
                inputId = inputId,
                outputId = outputId,
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
            foreach (MachineRecipe.InputData toRemove in data.currentRecipe.inputs[data.inputId].inputs)
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

            foreach (MachineRecipe.OutputData data in currentlyCrafting.currentRecipe.outputs[currentlyCrafting.outputId].outputs) // get items needed to be ejected
            {
                // find their corresponding outputId
                KeyValuePair<MachineRecipe.OutputData, int> kvp = new KeyValuePair<MachineRecipe.OutputData, int>(data, outputData[data]);

                for (int t = 0; t < kvp.Key.amount; t++)
                {
                    Debug.Log("Spawning on outputId" + (kvp.Value - 1));
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
