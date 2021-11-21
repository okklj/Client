using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack",menuName ="Character Stats/Attack")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;//Զ�̹�������
    public float coolDown;//CD��ȴʱ��
    public int minDamge;
    public int maxDamge;

    public float criticalMultiplier;//�����ӳ�
    public float criticalChance;//������

}
