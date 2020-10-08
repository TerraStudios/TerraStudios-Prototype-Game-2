using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackHandler : MonoBehaviour
{
    public static CallbackHandler instance;

    public List<Action> callbacks = new List<Action>();

    private void Awake() => instance = this;

    public int RegisterCallback(Action ev)
    {
        callbacks.Add(ev);
        return callbacks.FindIndex(a => a == ev);
    }

    public Action GetEvent(int ID) 
    {
        return callbacks[ID];
    }
}
