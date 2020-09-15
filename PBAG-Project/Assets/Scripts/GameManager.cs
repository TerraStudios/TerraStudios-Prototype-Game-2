using UnityEngine;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public string CountryCode = "en-US";
    public bool DebugMode = false; //May be moved to Super Secret Settings later on

    [HideInInspector] public CultureInfo currentCulture;

    [Header("Game Settings")]
    public GameProfile Profile;
    public static GameProfile profile
    {
        get
        {
            return instance.Profile;
        }
    }
    public GameProfile EasyProfile;
    public GameProfile MediumProfile;
    public GameProfile HardProfile;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        currentCulture = CultureInfo.CreateSpecificCulture(CountryCode);
        Log.DEBUG_MODE = DebugMode; //Set the debug mode for logging
    }

    public void GameOver() 
    {
        Debug.Log("Game Over!");
    }
}
