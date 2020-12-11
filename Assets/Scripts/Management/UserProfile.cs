using UnityEngine;

namespace CoreManagement
{
    /// <summary>
    /// These properties are used so the player can customize his playing experience.
    /// </summary>
    [CreateAssetMenu(fileName = "New User Profile", menuName = "Settings/New User Profile")]
    public class UserProfile : ScriptableObject
    {
        [Header("Currency visualization")]
        public bool manualTimeDateCC;
        public string timeDateCC;

        public bool manualCurrencyCC;
        public string currencyCC;
    }
}
