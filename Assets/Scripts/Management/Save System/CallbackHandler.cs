using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generated ID for methods. Useful when needing to call a function after scene reload.
/// </summary>
public class CallbackHandler : MonoBehaviour
{
    public static CallbackHandler instance;

    public List<Action> callbacks = new List<Action>();

    private void Awake() => instance = this;

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
    /// <param name="ID">Unique ID</param>
    /// <returns>The method action</returns>
    public Action GetEvent(int ID)
    {
        return callbacks[ID];
    }
}