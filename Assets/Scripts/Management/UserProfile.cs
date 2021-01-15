//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using UnityEngine;

namespace CoreManagement
{
    /// <summary>
    /// These properties are used so the player can customize his playing experience.
    /// </summary>
    [CreateAssetMenu(fileName = "New User Profile", menuName = "Settings/New User Profile")]
    public class UserProfile : ScriptableObject
    {
        public UserProfileData data = new UserProfileData();
    }

    [Serializable]
    public class UserProfileData
    {
        [Header("Currency visualization")]
        public bool manualTimeDateCC;
        public string timeDateCC;

        public bool manualCurrencyCC;
        public string currencyCC;
    }
}
