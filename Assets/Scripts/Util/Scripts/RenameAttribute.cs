using System;
using UnityEngine;
using UnityEditor;

public class RenameAttribute : PropertyAttribute {
    public string newName { get; private set; }

    public RenameAttribute(string name)
    {
        newName = name;
    }
}