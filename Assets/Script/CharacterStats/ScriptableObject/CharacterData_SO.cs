using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]//创建子集菜单
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;//防御
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentExp;
    public int baseExp;
    public int currentLevel;
    public int maxLevel;
    public float levelBuff;

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        //所有想要提升的数值
        if (currentLevel == maxLevel) return;

        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);
        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
    }
}
