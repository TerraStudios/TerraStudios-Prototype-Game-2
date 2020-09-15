using UnityEngine;

[CreateAssetMenu(fileName = "New User Profile", menuName = "Settings/New User Profile")]
public class UserProfile : ScriptableObject
{
    [Header("Currency visualization")]
    public bool manualTimeDateCC;
    public string timeDateCC;

    public bool manualCurrencyCC;
    public string currencyCC;
}
