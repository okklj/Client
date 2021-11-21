using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private static float dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform,Transform target)
    {
        /*
         Vector3.Dot()
        点积是一个浮点值，它等于 将两个向量的大小相乘，然后乘以向量之间角度的余弦值。
        对于 normalized 向量，如果它们指向完全相同的方向，Dot 返回 1； 如果它们指向
        完全相反的方向，返回 -1；如果向量彼此垂直，则 Dot 返回 0。
         */
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();
        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        return dot >= dotThreshold;
    }
}
