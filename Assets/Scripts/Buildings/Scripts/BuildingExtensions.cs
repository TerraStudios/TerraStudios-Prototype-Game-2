﻿//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using BuildingModules;
using UnityEngine;

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
    public static Vector3Int GetDirection(this IODirection direction, Quaternion currentIORotation)
    {
        Vector3Int loc = direction switch
        {
            IODirection.Forward => Vector3Int.right,
            IODirection.Backward => Vector3Int.left,
            IODirection.Left => Vector3Int.forward,
            IODirection.Right => Vector3Int.back,
            _ => Vector3Int.zero
        };

        // Switch according to rotation 
        loc = currentIORotation.eulerAngles.y switch
        {
            90 => new Vector3Int(loc.z, loc.y, -loc.x),
            180 => new Vector3Int(-loc.x, loc.y, -loc.z),
            270 => new Vector3Int(-loc.z, loc.y, loc.x),
            _ => loc,
        };

        return loc;
    }
}
public static class IOTypeExtension
{
    /// <summary>
    /// Retrieves the opposite <see cref="IOType"/> for an <see cref="IOType"/>
    /// </summary>
    /// <param name="type">The <see cref="IOType"/> to be inverted</param>
    /// <returns>The opposite <see cref="IOType"/></returns>
    public static IOType GetOppositeType(this IOType type)
    {
        return type == IOType.Input ? IOType.Output : IOType.Input;
    }
}
