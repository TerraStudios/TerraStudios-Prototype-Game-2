using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static double DistanceSquared(this Vector3 v, Vector3 o)
    {
        return (v - o).sqrMagnitude;
    }


}
