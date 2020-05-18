using UnityEngine;
using System.Globalization;

public class GameManger : MonoBehaviour
{
    public string CountryCode = "fr-FR";

    [HideInInspector] public CultureInfo currentCulture;

    private void Awake()
    {
        currentCulture = CultureInfo.CreateSpecificCulture(CountryCode);
    }

    public void GameOver() 
    {
        Debug.Log("Game Over!");
    }
}
