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

using System.Runtime.Serialization;
using UnityEngine;

/// <summary>
/// Allows Serialization and Deserialization of UnityEngine.Color
/// </summary>
public class ColorSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Color v = (Color)obj;
        info.AddValue("a", v.a);
        info.AddValue("r", v.r);
        info.AddValue("g", v.g);
        info.AddValue("b", v.b);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Color v = (Color)obj;
        v.a = (float)info.GetValue("a", typeof(float));
        v.r = (float)info.GetValue("r", typeof(float));
        v.g = (float)info.GetValue("g", typeof(float));
        v.b = (float)info.GetValue("b", typeof(float));
        obj = v;

        return obj;
    }
}