using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Top-level script of the game.
/// Holds properties like CultureInfo, GameProfile and UserProfile.
/// </summary>
public class GameManager : MonoBehaviour
{
    public bool DebugMode = false; //May be moved to Super Secret Settings later on

    [HideInInspector] public CultureInfo currentCultureTimeDate;
    [HideInInspector] public CultureInfo currentCultureCurrency;

    [Header("Game Settings")]
    public GameProfile profile;
    public static GameProfile Profile
    {
        get
        {
            return Instance.profile;
        }
    }
    public GameProfile easyProfile;
    public GameProfile mediumProfile;
    public GameProfile hardProfile;

    public UserProfile uProfile;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);

        // Game Profile always has higher priority than User Profile

        if (uProfile.manualTimeDateCC)
            currentCultureTimeDate = CultureInfo.CreateSpecificCulture(uProfile.timeDateCC);
        else
            currentCultureTimeDate = CultureInfo.CurrentCulture;

        if (Profile.forceManualCurrencyCC)
            currentCultureCurrency = CultureInfo.CreateSpecificCulture(Profile.currencyCC);
        else if (uProfile.manualCurrencyCC)
            currentCultureCurrency = CultureInfo.CreateSpecificCulture(uProfile.currencyCC);
        else
            currentCultureTimeDate = CultureInfo.CurrentCulture;

        Log.DEBUG_MODE = DebugMode; //Set the debug mode for logging
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGameAction());
    }

    private IEnumerator ResetGameAction()
    {
        yield return StartCoroutine(SceneHandler.instance.ReloadLevelAction());
        InitGame();
    }

    public void InitGame()
    {
        Time.timeScale = GameSave.current.timeSaveData.timeMultiplier;
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
