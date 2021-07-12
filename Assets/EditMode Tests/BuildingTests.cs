//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using BuildingModules;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests
{
    internal class BuildingTests
    {
        /// <summary>
        /// Tests for valid MeshRenderer values in all Building GameObjects.
        /// </summary>
        [Test]
        public void TestBuildings_MeshRenderer_CheckValidValues()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Building building = go.GetComponent<Building>();

                if (building)
                {
                    MeshRenderer renderer = go.GetComponent<MeshRenderer>();
                    if (!renderer)
                        continue;

                    Assert.IsFalse(renderer.receiveShadows, $"Receive Shadows needs to be disabled on GameObject {go.name}");
                    Assert.IsTrue(renderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off, $"Cast Shadows needs to be disabled on GameObject {go.name}");
                }
            }
        }

        /// <summary>
        /// Tests for valid values in the Building component of all Building GameObjects.
        /// </summary>
        [Test]
        public void TestBuildings_BuildingValues_HasValidBuildingValues()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Building building = go.GetComponent<Building>();

                if (building)
                {
                    Assert.IsNotEmpty(building.name, $"A Building ({go.name}) was found with an empty name");
                    //Assert.AreNotEqual(building.EconomyManager.)
                    Assert.AreNotEqual(building.bBase.price, 0, $"{building.name} found with a price of 0");
                    Assert.AreNotEqual(building.bBase.healthPercent, 0, $"{building.name} found with a health percent of 0");
                    Assert.AreNotEqual(building.bBase.monthsLifespanMax, 0, $"{building.name} found with a months lifespan max of 0");
                    Assert.AreNotEqual(building.bBase.monthsLifespanMin, 0, $"{building.name} found with a months lifespan min of 0");
                    Assert.AreNotEqual(building.bBase.penaltyForFix, 0, $"{building.name} found with a penalty for fix of 0");
                    Assert.AreNotEqual(building.bBase.timeToFixMultiplier, 0, $"{building.name} found with a time to fix multiplier of 0");
                    Assert.AreNotEqual(building.bBase.wattsPerHourIdle, 0, $"{building.name} found with a watts per hour idle of 0");
                    Assert.AreNotEqual(building.bBase.wattsPerHourWork, 0, $"{building.name} found with a watts per hour work of 0");
                }
            }
        }

        /// <summary>
        /// Tests if the Conveyor component in the Conveyor GameObjects has correct values.
        /// </summary>
        [Test]
        public void TestBuildings_ConveyorValues_HasValidValues()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Conveyor conveyor = go.GetComponent<Conveyor>();

                if (conveyor)
                {
                    Assert.AreNotEqual(conveyor.speed, 0, $"{go.name} found with a speed of 0");
                    //Assert.IsNotNull(conveyor.rb, $"{go.name} found with a null rb field"); //TODO: Evaluate whether a replacement is needed.
                }
            }
        }

        /// <summary>
        /// Tests for attached ModuleConnector to the Building component.
        /// Tests for missing essential components in the Module Connector.
        /// </summary>
        [Test]
        public void TestBuildings_BuildingValues_HasAttachedModuleConnectorWithEssentialValues()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Building building = go.GetComponent<Building>();

                if (building)
                {
                    Assert.IsNotNull(building.mc, $"{go.name} is missing a ModuleConnector");

                    ModuleConnector mc = building.mc;

                    Assert.IsNotNull(mc.building, $"{go.name}'s ModuleConnector is missing a Building");
                }
            }
        }

        /// <summary>
        /// Tests to see if the BuildingIOManager of all Buildings has duplicates IOs registered.
        /// </summary>
        [Test]
        public void TestBuildings_BuildingValues_HasDuplicateIOsAssignedToBuildingManager()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Building building = go.GetComponent<Building>();

                if (building && building.mc)
                {
                    if (building.mc.buildingIOManager)
                    {
                        List<BuildingIO> allIOs = building.mc.buildingIOManager.inputs.Concat(building.mc.buildingIOManager.outputs).ToList();

                        HashSet<BuildingIO> set = new HashSet<BuildingIO>();

                        foreach (BuildingIO io in allIOs)
                        {
                            if (!set.Add(io))
                            {
                                throw new Exception($"Duplicate BuildingIO found in building {building.name}");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests to see if the APMs have correct input and output space values.
        /// </summary>
        [Test]
        public void TestBuildings_APMValues_HasValidAPMSpace()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                APM apm = go.GetComponent<APM>();
                if (!apm)
                    return;

                Assert.IsFalse(apm.inputSpace <= 0);
                Assert.IsFalse(apm.outputSpace <= 0);
            }
        }
    }
}
