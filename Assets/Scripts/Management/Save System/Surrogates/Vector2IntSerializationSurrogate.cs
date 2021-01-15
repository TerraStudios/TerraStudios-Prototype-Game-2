//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Runtime.Serialization;
using UnityEngine;

/// <summary>
/// Allows Serialization and Deserialization of UnityEngine.Vector2Int
/// </summary>
public class Vector2IntSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector2Int v2 = (Vector2Int)obj;
        info.AddValue("x", v2.x);
        info.AddValue("y", v2.y);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector2Int v2 = (Vector2Int)obj;
        v2.x = (int)info.GetValue("x", typeof(int));
        v2.y = (int)info.GetValue("y", typeof(int));
        obj = v2;

        return obj;
    }
}