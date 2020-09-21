using UnityEngine;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public bool DebugMode = false; //May be moved to Super Secret Settings later on

    [HideInInspector] public CultureInfo currentCultureTimeDate;
    [HideInInspector] public CultureInfo currentCultureCurrency;

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

    public UserProfile uProfile;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        // Game Profile always has higher priority than User Profile

        if (uProfile.manualTimeDateCC)
            currentCultureTimeDate = CultureInfo.CreateSpecificCulture(uProfile.timeDateCC);
        else
            currentCultureTimeDate = CultureInfo.CurrentCulture;

        if (profile.forceManualCurrencyCC)
            currentCultureCurrency = CultureInfo.CreateSpecificCulture(profile.currencyCC);
        else if (uProfile.manualCurrencyCC)
            currentCultureCurrency = CultureInfo.CreateSpecificCulture(uProfile.currencyCC);
        else
            currentCultureTimeDate = CultureInfo.CurrentCulture;

        Log.DEBUG_MODE = DebugMode; //Set the debug mode for logging
    }

    public void GameOver() 
    {
        Debug.Log("Game Over!");
    }
}
