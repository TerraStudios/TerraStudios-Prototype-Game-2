using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum WorkStateEnum { On, Idle, Off }

[Serializable]
public class BuildingBase
{
    public string name;

    [Header("Economics")]
    public float price;
    public float Price { get => price * GameManager.instance.Profile.globalPriceMultiplierBuildings; set => price = value; }
 
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

[RequireComponent(typeof(ModuleConnector))]
public class Building : MonoBehaviour
{
    public BuildingBase Base;
    public ModuleConnector mc;
    [HideInInspector] public bool isSetUp;
    
    [Header("Grid Building Properties")]
    public Transform prefab;

    [Header("Input / Outputs")]
    public bool showDirectionOnVisualize = true;

    private GameObject currentIndicator;

    public WorkStateEnum WorkState
    {
        get => Base.workState;
        set
        {
            Base.workState = value;
            OnWorkStateChanged(value);
        }
    }

    
    // Required Components (Systems)
    [HideInInspector] public TimeManager TimeManager;
    [HideInInspector] public EconomyManager EconomyManager;

    private int HealthUpdateID;

    [HideInInspector] public string prefabLocation;

    /// <summary>
    /// Initializes the building's properties and work state.
    /// </summary>
    public void Init(bool newBasePresent = false)
    {
        if (mc.BuildingIOManager != null)
            mc.BuildingIOManager.Init();
        else
            Debug.LogWarning("Skipping Building IO Initialization");

        if (mc.APM != null)
            mc.APM.Init();

        isSetUp = true;

        HealthUpdateID = CallbackHandler.instance.RegisterCallback(OnHealthTimeUpdate);

        if (!newBasePresent)
        {
            StartWorkStateCounters();
            GenerateBuildingHealth();
        }

        originalMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        gameObject.AddComponent<BoxCollider>();
        RemoveIndicator();
        if (mc.BuildingIOManager)
            mc.BuildingIOManager.UpdateIOPhysics();
    }

    #region Health Submodule
    public void GenerateBuildingHealth()
    {
        Base.monthsLifespan = Mathf.RoundToInt(UnityEngine.Random.Range(Base.monthsLifespanMin, Base.monthsLifespanMax) * GameManager.profile.monthsLifespanMultiplier);
        TimeSpan timeToWait = TimeManager.CurrentTime.AddMonths(Base.monthsLifespan) - TimeManager.CurrentTime;
        Base.timeToDrainHealth = new TimeSpan(timeToWait.Ticks / Base.healthPercent);
        DepleteHealthEvent();
    }

    public void OnHealthTimeUpdate()
    {
        if (GameManager.profile.enableBuildingDamage)
            Base.healthPercent--;

        if (Base.healthPercent <= 0)
        {
            Base.healthPercent = 0;
            OnBuildingBreak();
            return;
        }

        DepleteHealthEvent();
    }

    private void DepleteHealthEvent()
    {
        Base.healthWaitEvents.Add(TimeManager.RegisterTimeWaiter(Base.timeToDrainHealth, HealthUpdateID));
    }

    public virtual void OnBuildingBreak()
    {
        SetIndicator(BuildingManager.instance.BrokenIndicator);
    }

    public void Fix()
    {
        if (Base.isFixRunning)
            return;

        float priceForFix = (float)(Base.healthPercent + 1) / 100 * Base.price * Base.penaltyForFix * GameManager.profile.buildingPenaltyForFixMultiplier;
        if (!EconomyManager.UpdateBalance((decimal)priceForFix))
        {
            WorkState = WorkStateEnum.Off;
            
            SetIndicator(BuildingManager.instance.FixingIndicator);
            foreach (TimeWaitEvent ev in Base.healthWaitEvents) { TimeManager.UnregisterTimeWaiter(ev); }
        }
        else
        {
            Debug.LogError("Insufficent balance to fix this building!");
            return;
        }

        Base.isFixRunning = true;

        float timeToWait = (100 - Base.healthPercent) * Base.timeToFixMultiplier * GameManager.profile.timeToFixMultiplier;
        StartCoroutine(FixCountdown());

        IEnumerator FixCountdown()
        {
            yield return new WaitForSeconds(timeToWait);
            FinalizeFix();
        }

        void FinalizeFix()
        {
            Base.isFixRunning = false;
            Base.healthPercent = 100;
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
            Base.workStateTimes.Add(ws, ev.hash);
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
        return TimeManager.GetTCETimeSpan(Base.workStateTimes[ws]);
    }

    /// <summary>
    /// Event for when the work state is changed in the machine.
    /// 
    /// The method currently pauses the time via <see cref="TimeManager.PauseTimeCounter(Guid)"/>
    /// </summary>
    /// <param name="newValue">The new value the building was set to</param>
    private void OnWorkStateChanged(WorkStateEnum newValue, bool quiet = false)
    {
        for (int i = 0; i < Base.workStateTimes.Count; i++)
        {
            WorkStateEnum key = Base.workStateTimes.Keys.ElementAt(i);
            Guid value = Base.workStateTimes[key];

            if (key != newValue)
            {
                TimeManager.PauseTimeCounter(value); // stop counting current WS
            }

            else
            {
                TimeManager.ContinueTimeCounter(value); // count new WS
            }
        }

        if (mc.BuildingIOManager != null && mc.BuildingIOManager.isConveyor)
        {
            if (newValue == WorkStateEnum.On)
                mc.BuildingIOManager.ChangeConveyorState(true);
            else if (newValue == WorkStateEnum.Off)
                mc.BuildingIOManager.ChangeConveyorState(false);

            return;
        }

        if (!quiet)
            mc.BuildingIOManager.SetConveyorGroupState(newValue);
    }
    #endregion

    #region Electricity Submodule
    public int GetUsedElectricity(WorkStateEnum ws)
    {
        double wattsPerMinute;
        switch (ws)
        {
            case WorkStateEnum.Idle:
                wattsPerMinute = Base.wattsPerHourIdle / 60;
                break;
            case WorkStateEnum.On:
                wattsPerMinute = Base.wattsPerHourWork / 60;
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
        currentIndicator = ObjectPoolManager.instance.ReuseObject(indicator.gameObject, transform.position + new Vector3(0, GetComponent<MeshRenderer>().bounds.size.y + 1f, 0), transform.rotation * Quaternion.Euler(0, 180, 0)).gameObject;
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

    /// <summary>
    /// Sets the workstate without triggering the OnWorkstateChanged event
    /// </summary>
    public void SetWorkstateSilent(WorkStateEnum newWorkState)
    {
        Base.workState = newWorkState;
        OnWorkStateChanged(newWorkState, true);
    }

    private Material originalMaterial;
    private bool markedForDelete;
    public void MarkForDelete()
    {
        if (markedForDelete)
            return;
        markedForDelete = true;
        GetComponent<MeshRenderer>().material = BuildingManager.instance.redArrow;
    }

    public void UnmarkForDelete()
    {
        if (!markedForDelete)
            return;
        markedForDelete = false;
        GetComponent<MeshRenderer>().material = originalMaterial;
    }
}
