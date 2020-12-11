﻿using BuildingManagers;
using CoreManagement;
using EconomySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TimeSystem;
using UnityEngine;
using Utilities;

namespace BuildingModules
{
    public enum WorkStateEnum { On, Idle, Off }

    /// <summary>
    /// This class contains properties for class <c>Building</c> that can be changed
    /// in runtime by the Save and Load System.
    /// </summary>
    [Serializable]
    public class BuildingBase
    {
        public string name;

        [Header("Economics")]
        public float price;
        public float Price { get => price * GameManager.Instance.profile.globalPriceMultiplierBuildings; set => price = value; }

        [Header("Work States")]
        public WorkStateEnum workState;

        public Dictionary<WorkStateEnum, Guid> workStateTimes = new Dictionary<WorkStateEnum, Guid>();

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
    /// General Building class, contained on all GameObjects that function as a Building.
    /// Handles Building Initialization, work states, building health, electricity and visualizations.
    /// </summary>
    [RequireComponent(typeof(ModuleConnector))]
    public class Building : MonoBehaviour
    {
        public BuildingBase bBase;
        public ModuleConnector mc;
        [HideInInspector] public bool isSetUp;

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

        private int healthUpdateID;

        [HideInInspector] public string prefabLocation;

        /// <summary>
        /// Initializes the building's properties and work state.
        /// </summary>
        public void Init(bool newBasePresent = false)
        {
            gameObject.AddComponent<BoxCollider>();
            if (mc.buildingIOManager != null)
            {
                mc.buildingIOManager.Init();
                mc.buildingIOManager.UpdateIOPhysics();
                mc.buildingIOManager.LinkAll();
            }
            else
                Debug.LogWarning("Skipping Building IO Initialization");

            if (mc.apm != null)
                mc.apm.Init();

            isSetUp = true;

            healthUpdateID = CallbackHandler.Instance.RegisterCallback(OnHealthTimeUpdate);

            if (!newBasePresent)
            {
                StartWorkStateCounters();
                GenerateBuildingHealth();
            }

            originalMaterial = GetComponent<MeshRenderer>().sharedMaterial;
            RemoveIndicator();
        }

        #region Health Submodule
        public void GenerateBuildingHealth()
        {
            bBase.monthsLifespan = Mathf.RoundToInt(UnityEngine.Random.Range(bBase.monthsLifespanMin, bBase.monthsLifespanMax) * GameManager.Profile.monthsLifespanMultiplier);
            TimeSpan timeToWait = timeManager.CurrentTime.AddMonths(bBase.monthsLifespan) - timeManager.CurrentTime;
            bBase.timeToDrainHealth = new TimeSpan(timeToWait.Ticks / bBase.healthPercent);
            DepleteHealthEvent();
        }

        public void OnHealthTimeUpdate()
        {
            if (GameManager.Profile.enableBuildingDamage)
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
            bBase.healthWaitEvents.Add(timeManager.RegisterTimeWaiter(bBase.timeToDrainHealth, healthUpdateID));
        }

        public virtual void OnBuildingBreak()
        {
            SetIndicator(BuildingManager.Instance.brokenIndicator);
        }

        public void Fix()
        {
            if (bBase.isFixRunning)
                return;

            float priceForFix = (float)(bBase.healthPercent + 1) / 100 * bBase.price * bBase.penaltyForFix * GameManager.Profile.buildingPenaltyForFixMultiplier;
            economyManager.Balance -= (decimal)priceForFix;
            WorkState = WorkStateEnum.Off;

            SetIndicator(BuildingManager.Instance.fixingIndicator);
            foreach (TimeWaitEvent ev in bBase.healthWaitEvents) { timeManager.UnregisterTimeWaiter(ev); }

            bBase.isFixRunning = true;

            float timeToWait = (100 - bBase.healthPercent) * bBase.timeToFixMultiplier * GameManager.Profile.timeToFixMultiplier;
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
                bBase.workStateTimes.Add(ws, ev.hash);
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
                Guid value = bBase.workStateTimes[key];

                if (key != newValue)
                {
                    timeManager.PauseTimeCounter(value); // stop counting current WS
                }

                else
                {
                    timeManager.ContinueTimeCounter(value); // count new WS
                }
            }

            if (mc.buildingIOManager?.isConveyor == true)
            {
                if (newValue == WorkStateEnum.On)
                    mc.buildingIOManager.ChangeConveyorState(true);
                else if (newValue == WorkStateEnum.Off)
                    mc.buildingIOManager.ChangeConveyorState(false);

                return;
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
            currentIndicator = ObjectPoolManager.Instance.ReuseObject(indicator.gameObject, transform.position + new Vector3(0, GetComponent<MeshRenderer>().bounds.size.y + 1f, 0), transform.rotation * Quaternion.Euler(0, 180, 0)).gameObject;
            currentIndicator.transform.parent = transform;

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
        /// Retrieves the grid size of the building
        /// </summary>
        /// <returns>A <see cref="Vector2Int"/> representing the grid size</returns>
        public Vector2Int GetBuildSize()
        {
            Vector3 e = GetComponent<MeshRenderer>().bounds.size;
            return new Vector2Int((int)Math.Round(e.x), (int)Math.Round(e.z));
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
            markedForDelete = true;
            GetComponent<MeshRenderer>().material = BuildingManager.Instance.redArrow;
        }

        public void UnmarkForDelete()
        {
            if (!markedForDelete)
                return;
            markedForDelete = false;
            GetComponent<MeshRenderer>().material = originalMaterial;
        }
    }
}
