using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class OutputSelector : MonoBehaviour
{
    // Dictionary values
    [HideInInspector] public MachineRecipe.OutputData value;
    private int outputID;

    [Header("UI Components")]
    public TMP_Text itemNameText;
    public Button button;
    public TMP_Text buttonText;

    public int OutputID 
    {
        get => outputID;
        set
        {
            outputID = value;
            buttonText.text = "Output " + value;
        }
    }

    private void Awake()
    {
        button.onClick.AddListener(OnChangeOutputIDButtonClicked);
    }

    private void OnChangeOutputIDButtonClicked() 
    {
        BuildingManager.instance.FocusedBuilding.mc.APM.OnOutputButtonPressed(this);
    }
}
