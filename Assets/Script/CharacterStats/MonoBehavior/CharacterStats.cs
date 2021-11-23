using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    public event Action<int,int> UpdateHealthBarOnAttack;//����Ѫ��
    public CharacterData_SO characterData;
    public CharacterData_SO templateData;//ģ������
    public AttackData_SO attackData;

    [HideInInspector]//��inspector���ڲ��ɼ�
    public bool isCritical;

    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }


    public int BaseDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.baseDefence;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            characterData.baseDefence = value;
        }
    }

    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
            {
                return characterData.currentDefence;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            characterData.currentDefence = value;
        }
    }

    #endregion

    #region Charater Combat
    public static void TakeDamge(CharacterStats attacker,CharacterStats defener)
    {
        int damage = Mathf.Max(1,attacker.CurrentDamage() - defener.CurrentDefence);
        defener.CurrentHealth = Mathf.Max(defener.CurrentHealth - damage, 0);
        Debug.Log(attacker.name + "��" + defener.name + "�����" + damage.ToString() + "�˺� ������"+attacker.isCritical);
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //update UI
        defener.UpdateHealthBarOnAttack?.Invoke(defener.CurrentHealth,defener.MaxHealth);
        //update ����ֵ
        if (defener.CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(defener.characterData.killPoint);
        }
    }

    public static void TakeDamge(int damage,CharacterStats defener)
    {
        int currentDamage= Mathf.Max(1, damage - defener.CurrentDefence);
        defener.CurrentHealth = Mathf.Max(defener.CurrentHealth - currentDamage, 0);
        defener.UpdateHealthBarOnAttack?.Invoke(defener.CurrentHealth, defener.MaxHealth);
        //������ʯͷ�˵ľ���ֵ����
        if (defener.CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(defener.characterData.killPoint);
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamge, attackData.maxDamge);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }
        return (int)coreDamage;
    }
    #endregion
}
