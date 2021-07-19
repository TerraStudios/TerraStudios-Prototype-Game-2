//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingManagement;
using CoreManagement;
using EconomyManagement;
using TimeSystem;
using UnityEngine;
using Utilities;

namespace BuildingModules
{
    public enum WorkStateEnum { On, Idle, Off }

    /// <summary>
    /// This class contains properties for class <see cref="Building"/> that can be changed
    /// in runtime by the Save and Load System.
    /// </summary>
    [Serializable]
    public class BuildingBase
    {
        public string name;

        [Header("Economics")]
        public float price;
        public float Price { get => price * GameManager.Instance.CurrentGameProfile.globalPriceMultiplierBuildings; set => price = value; }

        [Header("Work States")]
        public WorkStateEnum workState;

        public Dictionary<WorkStateEnum, int> workStateTimes = new Dictionary<WorkStateEnum, int>();

        [Tooltip("The minimum TimeSpan the machine will last in months")]
        public int monthsLifespanMin;
        [Tooltip("The maximum TimeSpan the machine will last in months")]
        public int monthsLifespanMax;
        [Tooltip("The cost (penalty) of fixing the building")]
        public float penaltyForFix;
        [Tooltip("The amount of time to fix the building")]
        public float timeToFixMultiplier;

        [Header("Health")]
        public int healthPercent = 100;
        [HideInInspector] public int monthsLifespan;
        [HideInInspector] public List<TimeWaitEvent> healthWaitEvents = new List<TimeWaitEvent>();
        [HideInInspector] public TimeSpan timeToDrainHealth;
        [HideInInspector] public bool isFixRunning;

        [Header("Electricity")]
        [Tooltip("Determines the energy usage per hour of the machine being idle")]
        public double wattsPerHourIdle;

        [Tooltip("Determines the energy usage per hour of the machine being active")]
        public double wattsPerHourWork;
    }

    /// <summary>
    /// Persistent data class about the Mesh GameObject.
    /// </summary>
    [Serializable]
    public class MeshData
    {
        // We save GO here so when we reload chunks, we don't do Resources.Load
        //public Transform go;

        // Save pos and rot since there isn't any other way to persistently save them.
        public Vector3 pos;
        public Quaternion rot;
        public Vector3Int size;
    }

    /// <summary>
    /// General Building class, contained on all GameObjects that function as a Building.
    /// Handles Building Initialization, work states, building health, electricity and visualizations.
    /// </summary>
    [RequireComponent(typeof(ModuleConnector))]
    [RequireComponent(typeof(BuildingIO))]
    public class Building : MonoBehaviour
    {
        public BuildingBase bBase;
        public MeshData meshData;
        public ModuleConnector mc;
        [HideInInspector] public GameObject correspondingMeshPrefab;
        [HideInInspector] public Transform correspondingMesh;
        [HideInInspector] public bool isSetUp;
        [HideInInspector] public bool isVisible = true;

        [Header("Grid Building Properties")]
        public Transform prefab;

        [Header("Input / Outputs")]
        public bool showDirectionOnVisualize = true;

        private GameObject currentIndicator;

        public WorkStateEnum WorkState
        {
            get => bBase.workState;
            set
            {
                bBase.workState = value;
                OnWorkStateChanged(value);
            }
        }

        // Required Components (Systems)
        [HideInInspector] public TimeManager timeManager;
        [HideInInspector] public EconomyManager economyManager;

        private int healthUpdateId;

        [HideInInspector] public string scriptPrefabLocation;

        /// <summary>
        /// Prepares the building for visualization work.
        /// </summary>
        public void PreInit()
        {
            mc.buildingIOManager.PreInit();
        }

        /// <summary>
        /// Initializes the building's properties and work state.
        /// </summary>
        public void Init(bool newBasePresent = false)
        {
            if (mc.apm != null)
                mc.apm.Init();

            if (mc.conveyor != null)
                mc.conveyor.Init();

            isSetUp = true;

            healthUpdateId = CallbackHandler.Instance.RegisterCallback(OnHealthTimeUpdate);

            if (!newBasePresent)
            {
                StartWorkStateCounters();
                GenerateBuildingHealth();
            }
        }

        public void InitMesh(Transform meshGO)
        {
            correspondingMesh = meshGO;

            if (mc.buildingIOManager != null)
            {
                mc.buildingIOManager.Init();
                //mc.buildingIOManager.UpdateIOPhysics();
                mc.buildingIOManager.LinkAll();
            }
            else
                Debug.LogWarning("Skipping Building IO Initialization");

            originalMaterial = correspondingMesh.GetComponent<MeshRenderer>().sharedMaterial;
            RemoveIndicator();

            // Add collider to the mesh so the Building can be selected
            BoxCollider collider = correspondingMesh.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        #region Health Submodule
        public void GenerateBuildingHealth()
        {
            bBase.monthsLifespan = Mathf.RoundToInt(UnityEngine.Random.Range(bBase.monthsLifespanMin, bBase.monthsLifespanMax) * GameManager.Instance.CurrentGameProfile.monthsLifespanMultiplier);
            TimeSpan timeToWait = timeManager.CurrentTime.AddMonths(bBase.monthsLifespan) - timeManager.CurrentTime;
            bBase.timeToDrainHealth = new TimeSpan(timeToWait.Ticks / bBase.healthPercent);
            DepleteHealthEvent();
        }

        public void OnHealthTimeUpdate()
        {
            if (GameManager.Instance.CurrentGameProfile.enableBuildingDamage)
                bBase.healthPercent--;

            if (bBase.healthPercent <= 0)
            {
                bBase.healthPercent = 0;
                OnBuildingBreak();
                return;
            }

            DepleteHealthEvent();
        }

        private void DepleteHealthEvent()
        {
            bBase.healthWaitEvents.Add(timeManager.RegisterTimeWaiter(bBase.timeToDrainHealth, healthUpdateId));
        }

        public void OnBuildingShow()
        {
            isVisible = true;
            if (mc.buildingIOManager.isConveyor)
                mc.conveyor.LoadItemMeshes();
        }

        public void OnBuildingHide()
        {
            isVisible = false;
            if (mc.buildingIOManager.isConveyor)
                mc.conveyor.UnloadItemMeshes();
        }

        public virtual void OnBuildingBreak()
        {
            SetIndicator(BuildingManager.Instance.brokenIndicator);
        }

        public void Fix()
        {
            if (bBase.isFixRunning)
                return;

            float priceForFix = (float)(bBase.healthPercent + 1) / 100 * bBase.price * bBase.penaltyForFix * GameManager.Instance.CurrentGameProfile.buildingPenaltyForFixMultiplier;
            EconomyManager.Instance.AttemptTransaction(-priceForFix);
            WorkState = WorkStateEnum.Off;

            SetIndicator(BuildingManager.Instance.fixingIndicator);
            foreach (TimeWaitEvent ev in bBase.healthWaitEvents) { timeManager.UnregisterTimeWaiter(ev); }

            bBase.isFixRunning = true;

            float timeToWait = (100 - bBase.healthPercent) * bBase.timeToFixMultiplier * GameManager.Instance.CurrentGameProfile.timeToFixMultiplier;
            StartCoroutine(FixCountdown());

            IEnumerator FixCountdown()
            {
                yield return new WaitForSeconds(timeToWait);
                FinalizeFix();
            }

            void FinalizeFix()
            {
                bBase.isFixRunning = false;
                bBase.healthPercent = 100;
                WorkState = WorkStateEnum.On;
                DepleteHealthEvent();
                RemoveIndicator();
            }
        }
        #endregion

        #region WorkState Submodule
        /// <summary>
        /// Starts all of the <see cref="TimeCountEvent"/>s and sets the state for the Building.
        /// </summary>
        public void StartWorkStateCounters()
        {
            foreach (WorkStateEnum ws in (WorkStateEnum[])Enum.GetValues(typeof(WorkStateEnum)))
            {
                TimeCountEvent ev = timeManager.StartTimeCounter();
                bBase.workStateTimes.Add(ws, timeManager.timeCounters.IndexOf(ev));
            }

            WorkState = WorkStateEnum.On;
        }

        /// <summary>
        /// Retrieves the amount of time the building has been in a current <see cref="WorkStateEnum"/>
        /// </summary>
        /// <param name="ws">The <see cref="WorkStateEnum"/> to get the value from</param>
        /// <returns>A <see cref="TimeSpan"/> of how long the state has been active</returns>
        public TimeSpan GetTimeForWS(WorkStateEnum ws)
        {
            return timeManager.GetTCETimeSpan(bBase.workStateTimes[ws]);
        }

        /// <summary>
        /// Event for when the work state is changed in the machine.
        /// The method currently pauses the time via <see cref="timeManager.PauseTimeCounter(Guid)"/>
        /// </summary>
        /// <param name="newValue">The new value the building was set to</param>
        private void OnWorkStateChanged(WorkStateEnum newValue, bool quiet = false)
        {
            for (int i = 0; i < bBase.workStateTimes.Count; i++)
            {
                WorkStateEnum key = bBase.workStateTimes.Keys.ElementAt(i);
                int value = bBase.workStateTimes[key];

                if (key != newValue)
                {
                    timeManager.PauseTimeCounter(value); // stop counting current WS
                }

                else
                {
                    timeManager.ContinueTimeCounter(value); // count new WS
                }
            }

            if (mc.buildingIOManager)
            {
                if (mc.buildingIOManager.isConveyor)
                {
                    if (newValue == WorkStateEnum.On)
                        mc.buildingIOManager.ChangeConveyorState(true);
                    else if (newValue == WorkStateEnum.Off)
                        mc.buildingIOManager.ChangeConveyorState(false);

                    return;
                }
            }

            if (!quiet)
                mc.buildingIOManager.SetConveyorGroupState(newValue);
        }
        #endregion

        #region Electricity Submodule
        public int GetUsedElectricity(WorkStateEnum ws)
        {
            double wattsPerMinute;
            switch (ws)
            {
                case WorkStateEnum.Idle:
                    wattsPerMinute = bBase.wattsPerHourIdle / 60;
                    break;
                case WorkStateEnum.On:
                    wattsPerMinute = bBase.wattsPerHourWork / 60;
                    break;
                default:
                    wattsPerMinute = 0;
                    Debug.LogError("Trying to get watts used for an unknown WorkState");
                    break;
            }
            int wattsUsed = Convert.ToInt32(GetTimeForWS(ws).TotalMinutes * wattsPerMinute);
            return wattsUsed;
        }
        #endregion

        /// <summary>
        /// Sets the current indicator for the building.
        /// </summary>
        /// <param name="indicator">A <see cref="Transform"/> object representing the indicator prefab</param>
        public void SetIndicator(Transform indicator)
        {
            if (currentIndicator != null && currentIndicator.GetComponent<MeshRenderer>().Equals(indicator.GetComponent<MeshRenderer>())) return;

            RemoveIndicator();
            currentIndicator = ObjectPoolManager.Instance.ReuseObject(indicator.gameObject, transform.position + new Vector3(0, correspondingMesh.GetComponent<MeshRenderer>().bounds.size.y + 1f, 0), transform.rotation * Quaternion.Euler(0, 180, 0)).gameObject;
        }

        /// <summary>
        /// Removes the indicator from the 
        /// </summary>
        public void RemoveIndicator()
        {
            if (currentIndicator != null)
            {
                ObjectPoolManager.Instance.DestroyObject(currentIndicator);
                currentIndicator = null;
            }
        }

        /// <summary>
        /// Retrieves the current indicator (if any) the building is assigned to 
        /// </summary>
        /// <returns>The indicator's <see cref="GameObject"/>. If no indicator is present this will return null.</returns>
        public GameObject GetIndicator()
        {
            return currentIndicator;
        }

        /// <summary>
        /// Sets the workstate without triggering the OnWorkstateChanged event
        /// </summary>
        public void SetWorkstateSilent(WorkStateEnum newWorkState)
        {
            bBase.workState = newWorkState;
            OnWorkStateChanged(newWorkState, true);
        }

        private Material originalMaterial;
        private bool markedForDelete;
        public void MarkForDelete()
        {
            if (markedForDelete)
                return;
            Debug.Log("marking");
            markedForDelete = true;
            correspondingMesh.GetComponent<MeshRenderer>().material.SetFloat("_IsSelected", 1);
        }

        public void UnmarkForDelete()
        {
            if (!markedForDelete)
                return;
            markedForDelete = false;
            Debug.Log("unmarking");
            correspondingMesh.GetComponent<MeshRenderer>().material.SetFloat("_IsSelected", 0);
        }
    }
}
