using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Tests
{
    internal class BuildingTests
    {

        [Test]
        public void CheckRenderingData()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Building building = go.GetComponent<Building>();

                if (building)
                {
                    Assert.IsFalse(go.GetComponent<MeshRenderer>().receiveShadows, $"Receive Shadows needs to be disabled on GameObject {go.name}");
                    Assert.IsTrue(go.GetComponent<MeshRenderer>().shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off, $"Cast Shadows needs to be disabled on GameObject {go.name}");
                }
            }

        }


        [Test]
        public void CheckBuildingData()
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

        [Test]
        public void CheckConveyorData()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Conveyor conveyor = go.GetComponent<Conveyor>();

                if (conveyor)
                {
                    Assert.AreNotEqual(conveyor.speed, 0, $"{go.name} found with a speed of 0");
                    Assert.IsNotNull(conveyor.rb, $"{go.name} found with a null rb field");
                }
            }

        }

        [Test]
        public void CheckModuleConnectorData()
        {
            foreach (GameObject go in Resources.LoadAll<GameObject>("Buildings"))
            {
                Building building = go.GetComponent<Building>();

                if (building)
                {
                    Assert.IsNotNull(building.mc, $"{go.name} is missing a ModuleConnector");

                    ModuleConnector mc = building.mc;

                    Assert.IsNotNull(mc.buildingIOManager, $"{go.name}'s ModuleConnector is missing a BuildingIOManager");
                    Assert.IsNotNull(mc.building, $"{go.name}'s ModuleConnector is missing a Building");
                }
            }

        }

        [Test]
        public void TestBuildingIOData()
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

        [Test]
        public void CheckValidAPMSpace()
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
