//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

static class Vector3Extension
{
    public static int3 FloorToInt3(this Vector3 vec3)
    {
        return new int3(Mathf.FloorToInt(vec3.x), Mathf.FloorToInt(vec3.y), Mathf.FloorToInt(vec3.z));
    }

    public static int3 CeilToInt3(this Vector3 vec3)
    {
        return new int3(Mathf.CeilToInt(vec3.x), Mathf.CeilToInt(vec3.y), Mathf.CeilToInt(vec3.z));
    }
}
