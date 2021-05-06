//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using UnityEngine;
using BuildingModules;

/// <summary>
/// Because C# is bad, an extension method is needed to add a method for each direction. The extension retrieves the <see cref="Vector3Int"/> value for code later on.
/// </summary>
/// Usage: <code>IODirection.GetDirection();</code>
public static class IODirectionExtension
{
    /// <summary>
    /// Fetches the <see cref="Vector3Int"/> value of an <see cref="IODirection"/>
    /// </summary>
    /// <param name="direction">The <see cref="IODirection"/></param>
    /// <returns>A normalized <see cref="Vector3Int"/> for manipulating the direction</returns>
    public static Vector3Int GetDirection(this IODirection direction)
    {
        switch (direction)
        {
            case IODirection.Forward:
                return new Vector3Int(1, 0, 0);
            case IODirection.Backward:
                return new Vector3Int(-1, 0, 0);
            case IODirection.Left:
                return new Vector3Int(0, 0, 1);
            case IODirection.Right:
                return new Vector3Int(0, 0, -1);
            default:
                return Vector3Int.zero;
        }
    }
}
