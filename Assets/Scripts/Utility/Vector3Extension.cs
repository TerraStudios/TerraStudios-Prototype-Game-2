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
