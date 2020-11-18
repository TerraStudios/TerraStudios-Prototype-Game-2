using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputSelector : MonoBehaviour
{
    // Dictionary values
    [HideInInspector] public MachineRecipe.InputData value;
    private int inputID;

    [Header("UI Components")]
    public TMP_Text itemNameText;
    public Button button;
    public TMP_Text buttonText;

    public int InputID
    {
        get => inputID;
        set
        {
            inputID = value;
            if (value == -1)
                buttonText.text = "Input Any";
            else
                buttonText.text = "Input " + value;
        }
    }

    private void Awake()
    {
        button.onClick.AddListener(OnChangeInputIDButtonClicked);
    }

    private void OnChangeInputIDButtonClicked()
    {
        BuildingManager.instance.FocusedBuilding.mc.APM.OnInputButtonPressed(this);
    }
}
