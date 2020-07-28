using UnityEngine;
using System.Globalization;

public class GameManger : MonoBehaviour
{
    public string CountryCode = "fr-FR";
    public bool DebugMode = false;

    [HideInInspector] public CultureInfo currentCulture;

    private void Awake()
    {
        currentCulture = CultureInfo.CreateSpecificCulture(CountryCode);
        Log.DEBUG_MODE = DebugMode;
    }

    public void GameOver() 
    {
        Debug.Log("Game Over!");
    }
}
