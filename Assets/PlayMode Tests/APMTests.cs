using BuildingModules;
using ItemManagement;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tests
{
    public class APMTests
    {
        private static readonly System.Random random = new System.Random();

        // A Test behaves as an ordinary method
        [Test]
        public void TestRecipeInAPM_1Input1Output_ItemsShouldEnterSuccessfully()
        {
            GameObject apmGO = CreateAPM();
            BuildingIOManager manager = apmGO.GetComponent<BuildingIOManager>();
            APM apm = apmGO.GetComponent<APM>();
            apm.mc = manager.mc;

            manager.inputs = new BuildingIO[0];
            manager.outputs = new BuildingIO[] { GetFakeBuildingIOOutput() };

            ItemData input1 = GetFakeItem();

            // Create fake recipe for testing
            /*MachineRecipe recipe = ScriptableObject.CreateInstance<MachineRecipe>();

            recipe.inputs = new List<MachineRecipe.InputsData>
                {
                    new MachineRecipe.InputsData
                    {
                        new MachineRecipe.InputData
                        {
                            item = input1, amount = 1, inputID = 0
                        }
                    }
                };
            recipe.outputs = new List<MachineRecipe.OutputsData>
                {
                     new MachineRecipe.OutputsData
                     {
                         new MachineRecipe.OutputData
                         {
                             item = GetFakeItemData(), amount = random.Next(1, 3)
                         }
                     }
                };*/
            //recipe.baseTime = random.Next(1, 10);

            //Set current recipe for apm
            //apm.CurrentRecipe = recipe;

            //Initialize APM, which listens in to the OnItemEnterEvent
            apm.Init();

            GameObject sceneInstance = new GameObject("Fake scene instance");

            Dictionary<ItemData, int> proposedItems = new Dictionary<ItemData, int>();

            ItemData toInput = GetFakeItemData(input1);

            proposedItems[toInput] = 1;

            //Invoke OnItemEnterEvent
            OnItemEnterEvent args = new OnItemEnterEvent()
            {
                inputID = 0,
                item = toInput,
                sceneInstance = sceneInstance,
                proposedItems = proposedItems
            };

            //

            //Assert.AreNotEqual(proposedItems, itemsInside, "Item was rejected from APM.");

            Assert.IsTrue(manager.mc.apm.IsAllowedToEnter(args), "Item was rejected from APM.");

            //Currently throws a NPE because of the fake BuildingIO not containing proper information, to be revamped later 
            //Assert.IsTrue(manager.mc.APM.IsAllowedToStartCrafting(args), "APM not allowed to start crafting when it should be able to.");
        }

        private GameObject CreateAPM()
        {
            GameObject apmObject = new GameObject("APM_GO");
            BuildingIOManager manager = apmObject.AddComponent<BuildingIOManager>();
            //Add APM Component
            APM apm = apmObject.AddComponent<APM>();

            ModuleConnector mc = apmObject.AddComponent<ModuleConnector>();

            // Attach modules
            mc.apm = apm;
            mc.buildingIOManager = manager;

            //Add ModuleConnector
            apm.mc = mc;
            manager.mc = mc;

            //Allow OnItemEnterEvent listening
            manager.OnItemEnterInput = new OnItemEnterEvent();

            return apmObject;
        }

        private ItemData GetFakeItem()
        {
            //Randomize whether the ItemOrCategory will be of type ItemData or ItemCategory
            return GetFakeItemData();
        }

        private ItemData GetFakeItemData(ItemData item)
        {
            //ItemOrCategory is an ItemData, just return a copy of it
            return item;
        }

        private ItemData GetFakeItemData()
        {
            ItemData data = ScriptableObject.CreateInstance<ItemData>();
            data.name = GetRandomString();
            data.ID = random.Next(3000);
            data.isBuyable = random.Next(1) == 0;
            return data;
        }

        private BuildingIO GetFakeBuildingIOOutput()
        {
            GameObject obj = new GameObject("Fake IO Output");

            try
            {
                BuildingIO io = obj.AddComponent<BuildingIO>();
                io.arrow = new GameObject("BuildingIO Output Arrow").transform;

                return io;
            }
            catch (NullReferenceException)
            { }

            throw new Exception("Couldn't add BuildingIO component");
        }

        private string GetRandomString()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(8).ToArray());
        }
    }
}
