using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStatus
{
    GUARD, PATROL, CHASE, DEAD
}

//自动添加组件
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    public EnemyStatus enemyStatus;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        SwitchStatus();
    }

    void SwitchStatus()
    {
        switch (enemyStatus)
        {
            case EnemyStatus.GUARD:
                break;
            case EnemyStatus.PATROL:
                break;
            case EnemyStatus.CHASE:
                break;
            case EnemyStatus.DEAD:
                break;
        }
    }
}
