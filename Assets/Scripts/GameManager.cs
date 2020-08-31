using UnityEngine;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public string CountryCode = "en-US";
    public bool DebugMode = false; //May be moved to Super Secret Settings later on

    [HideInInspector] public CultureInfo currentCulture;

    [Header("Remove System")]
    public float removePenaltyMultiplier;
    public float garbageRemoveMultiplier;

    private void Awake()
    {
        currentCulture = CultureInfo.CreateSpecificCulture(CountryCode);
        Log.DEBUG_MODE = DebugMode; //Set the debug mode for logging
    }

    public void GameOver() 
    {
        Debug.Log("Game Over!");
    }
}
