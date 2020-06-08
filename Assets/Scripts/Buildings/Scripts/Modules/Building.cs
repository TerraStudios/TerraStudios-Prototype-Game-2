﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum WorkStateEnum { On, Idle, Off }

public class Building : MonoBehaviour
{
    [HideInInspector] public bool isSetUp;
    [Header("Grid Building Properties")]
    public Transform prefab;


    public BuildingIOManager BuildingIOManager;

    [Header("Input / Outputs")]
    public bool allowAllInputs;
    public ItemData[] inputsAllowed;
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




    public void Init()
    {

        if (BuildingIOManager != null)
            BuildingIOManager.Init();
        else
            Debug.LogWarning("Skipping Building IO Initialization");

        HideBuildingDirection();

        isSetUp = true;

        StartWorkStateCounters();
        GenerateBuildingHealth();
    }

    public bool HasCentricTile()
    {
        Vector2Int buildSize = GetBuildSize();

        if (buildSize.x % 2 == 0 && buildSize.y % 2 == 0)
            return false;
        else
            return true;
    }

    private Transform directionArrow;

    public void HideBuildingDirection() { if (directionArrow && showDirectionOnVisualize) Destroy(directionArrow.gameObject); }

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
            OnBuildingBrake();
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

    public virtual void OnBuildingBrake()
    {
        Debug.Log("I misspelled the word break! (it broke)");
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
    public void StartWorkStateCounters()
    {
        foreach (WorkStateEnum ws in (WorkStateEnum[])Enum.GetValues(typeof(WorkStateEnum)))
        {
            TimeCountEvent ev = TimeManager.StartTimeCounter();
            workStateTimes.Add(ws, ev.hash);
        }

        WorkState = WorkStateEnum.On;
    }

    public TimeSpan GetTimeForWS(WorkStateEnum ws)
    {
        return TimeManager.GetTCETimeSpan(workStateTimes[ws]);
    }

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

        if (BuildingIOManager == null)
            return;

        if (newValue == WorkStateEnum.Off)
        {
            if (!BuildingIOManager.isConveyor)
                BuildingIOManager.ModifyConveyorState(null, false);
            else
            {
                BuildingIOManager.ConveyorManager.speed = 0;
            }
        }
        else
        {
            if (!BuildingIOManager.isConveyor)
                BuildingIOManager.ModifyConveyorState(null, true);
            else
            {
                BuildingIOManager.ConveyorManager.speed = 1;
            }
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

    public void SetIndicator(Transform indicator)
    {
        if (currentIndicator != null && currentIndicator.GetComponent<MeshRenderer>().Equals(indicator.GetComponent<MeshRenderer>())) return;

        RemoveIndicator();
        currentIndicator = Instantiate(indicator, transform.position + new Vector3(0, GetComponent<MeshFilter>().mesh.bounds.size.y + 1f, 0), transform.rotation * Quaternion.Euler(0, 180, 0)).gameObject;
        currentIndicator.transform.parent = this.transform;
        
        
    }



    public void RemoveIndicator()
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
    }

    public GameObject GetIndicator()
    {
        return currentIndicator;
    }

    public Vector2Int GetBuildSize()
    {
        Vector3 e =  GetComponent<MeshRenderer>().bounds.size;
        return new Vector2Int((int) Math.Round(e.x), (int) Math.Round(e.z));
    }
}
