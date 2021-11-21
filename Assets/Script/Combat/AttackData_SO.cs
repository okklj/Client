using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack",menuName ="Character Stats/Attack")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;//远程攻击距离
    public float coolDown;//CD冷却时间
    public int minDamge;
    public int maxDamge;

    public float criticalMultiplier;//暴击加成
    public float criticalChance;//暴击率

}
