//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generated ID for methods. Useful when needing to call a function after scene reload.
/// </summary>
public class CallbackHandler : MonoBehaviour
{
    public static CallbackHandler Instance;

    public List<Action> callbacks = new List<Action>();

    private void Awake() => Instance = this;

    /// <summary>
    /// Generates a unique ID to the method
    /// </summary>
    /// <param name="ev">Method action</param>
    /// <returns>Unique method ID</returns>
    public int RegisterCallback(Action ev)
    {
        callbacks.Add(ev);
        return callbacks.FindIndex(a => a == ev);
    }

    /// <summary>
    /// Gets the method from the unique ID
    /// </summary>
    /// <param name="id">Unique ID</param>
    /// <returns>The method action</returns>
    public Action GetEvent(int id)
    {
        return callbacks[id];
    }
}
