using UnityEngine;
using System.Globalization;

public class GameManger : MonoBehaviour
{
    public string CountryCode = "fr-FR";
    public bool DebugMode = false; //May be moved to Super Secret Settings later on

    [HideInInspector] public CultureInfo currentCulture;

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
