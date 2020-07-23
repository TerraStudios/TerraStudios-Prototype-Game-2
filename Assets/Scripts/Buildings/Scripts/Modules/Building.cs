﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum WorkStateEnum { On, Idle, Off }

[RequireComponent(typeof(ModuleConnector))]
public class Building : MonoBehaviour
{
    public ModuleConnector mc;
    [HideInInspector] public bool isSetUp;
    [Header("Grid Building Properties")]
    public Transform prefab;
    public MeshRenderer renderer;


    [Header("Input / Outputs")]
    public bool showDirectionOnVisualize = true;

    private GameObject currentIndicator;

    [Header("Economics")]
    public int price;

    [Header("Work States")]
    private WorkStateEnum workState;
    public WorkStateEnum WorkState
    {
        get => workState;
        set
        {
            workState = value;
            OnWorkStateChanged(value);
        }
    }
    private Dictionary<WorkStateEnum, Guid> workStateTimes = new Dictionary<WorkStateEnum, Guid>();

    [Header("Health")]
    public int healthPercent = 100;
    public int monthsLifespanMin;
    public int monthsLifespanMax;
    public int penaltyForFix;
    public float timeToFixMultiplier;
    private int monthsLifespan;
    private List<TimeWaitEvent> healthWaitEvents = new List<TimeWaitEvent>();
    private TimeSpan timeToDrainHealth;

    [Header("Electricity")]
    public double wattsPerHourIdle;
    public double wattsPerHourWork;

    // Required Components (Systems)
    [HideInInspector] public TimeManager TimeManager;
    [HideInInspector] public EconomyManager EconomyManager;

    /// <summary>
    /// Caches the <see cref="MeshRenderer"/> of the building for later use in <see cref="BuildingIO.IsInputSupported(BuildingIO)"/>
    /// </summary>
    public void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Initializes the building's properties and work state.
    /// </summary>
    public void Init()
    {
        if (mc.BuildingIOManager != null)
            mc.BuildingIOManager.Init();
        else
            Debug.LogWarning("Skipping Building IO Initialization");

        if (mc.APM != null)
            mc.APM.Init();

        isSetUp = true;

        StartWorkStateCounters();
        GenerateBuildingHealth();

        
    }

    #region Health Submodule
    public void GenerateBuildingHealth()
    {
        monthsLifespan = UnityEngine.Random.Range(monthsLifespanMin, monthsLifespanMax);
        TimeSpan timeToWait = TimeManager.CurrentTime.AddMonths(monthsLifespan) - TimeManager.CurrentTime;
        timeToDrainHealth = new TimeSpan(timeToWait.Ticks / healthPercent);
        DepleteHealthEvent();
    }

    public void OnHealthTimeUpdate()
    {
        healthPercent--;

        if (healthPercent <= 0)
        {
            healthPercent = 0;
            OnBuildingBreak();
            return;
        }

        DepleteHealthEvent();
    }

    private void DepleteHealthEvent()
    {
        UnityEvent callback = new UnityEvent();
        callback.AddListener(OnHealthTimeUpdate);
        healthWaitEvents.Add(TimeManager.RegisterTimeWaiter(timeToDrainHealth, callback));
    }

    public virtual void OnBuildingBreak()
    {
        SetIndicator(BuildingManager.instance.BrokenIndicator);
    }

    public void Fix()
    {
        int priceForFix = (healthPercent + 1 / 100) * price * penaltyForFix;
        if (EconomyManager.Balance >= priceForFix)
        {
            WorkState = WorkStateEnum.Off;
            EconomyManager.Balance -= priceForFix;
            SetIndicator(BuildingManager.instance.FixingIndicator);
            foreach (TimeWaitEvent ev in healthWaitEvents) { TimeManager.UnregisterTimeWaiter(ev); }
        }
        else
        {
            Debug.LogError("Insufficent balance!");
        }

        float timeToWait = (100 - healthPercent) * timeToFixMultiplier;
        StartCoroutine(FixCountdown());

        IEnumerator FixCountdown()
        {
            yield return new WaitForSeconds(timeToWait);
            FinalizeFix();
        }

        void FinalizeFix()
        {
            healthPercent = 100;
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
            TimeCountEvent ev = TimeManager.StartTimeCounter();
            workStateTimes.Add(ws, ev.hash);
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
        return TimeManager.GetTCETimeSpan(workStateTimes[ws]);
    }

    /// <summary>
    /// Event for when the work state is changed in the machine.
    /// 
    /// The method currently pauses the time via <see cref="TimeManager.PauseTimeCounter(Guid)"/>
    /// </summary>
    /// <param name="newValue">The new value the building was set to</param>
    private void OnWorkStateChanged(WorkStateEnum newValue)
    {
        for (int i = 0; i < workStateTimes.Count; i++)
        {
            WorkStateEnum key = workStateTimes.Keys.ElementAt(i);
            Guid value = workStateTimes[key];

            if (key != newValue)
            {
                TimeManager.PauseTimeCounter(value); // stop counting current WS
            }

            else
            {
                TimeManager.ContinueTimeCounter(value); // count new WS
            }
        }

        if (mc.BuildingIOManager == null && mc.BuildingIOManager.isConveyor)
        {
            Debug.Log("Returned");
            return;
        }

        if (newValue == WorkStateEnum.Off)
        {
            mc.BuildingIOManager.ModifyConveyorGroup(null, false);
        }
        else
        {
            mc.BuildingIOManager.ModifyConveyorGroup(null, true);
        }
    }
    #endregion

    #region Electricity Submodule
    public int GetUsedElectricity(WorkStateEnum ws)
    {
        double wattsPerMinute;
        switch (ws)
        {
            case WorkStateEnum.Idle:
                wattsPerMinute = wattsPerHourIdle / 60;
                break;
            case WorkStateEnum.On:
                wattsPerMinute = wattsPerHourWork / 60;
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
        currentIndicator = ObjectPoolManager.instance.ReuseObject(indicator.gameObject, transform.position + new Vector3(0, GetComponent<MeshFilter>().mesh.bounds.size.y + 1f, 0), transform.rotation * Quaternion.Euler(0, 180, 0)).gameObject;
        currentIndicator.transform.parent = this.transform;

    }

    /// <summary>
    /// Removes the indicator from the 
    /// </summary>
    public void RemoveIndicator()
    {
        if (currentIndicator != null)
        {
            ObjectPoolManager.instance.DestroyObject(currentIndicator);
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
}
