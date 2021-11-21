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
        �����һ������ֵ�������� �����������Ĵ�С��ˣ�Ȼ���������֮��Ƕȵ�����ֵ��
        ���� normalized �������������ָ����ȫ��ͬ�ķ���Dot ���� 1�� �������ָ��
        ��ȫ�෴�ķ��򣬷��� -1����������˴˴�ֱ���� Dot ���� 0��
         */
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();
        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        return dot >= dotThreshold;
    }
}
