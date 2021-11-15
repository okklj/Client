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
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyStatus enemyStatus;
    private Animator anim;

    [Header("Basic Settings")]
    public float sightRadius;//视野范围
    public bool isGuard;
    private GameObject attackTarget;
    private float speed;//记录原来速度

    [Header("Patrol State")]//巡逻参数
    public float patrolRange;
    private Vector3 wayPoint;//在范围内随机移动点
    private Vector3 guardPos;//初始点
    public float lookAtTime;//停止观察时间
    private float remainLookAtTime;


    //配合动画参数
    bool isWalk;
    bool isChase;//追逐
    bool isFollow;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStatus = EnemyStatus.GUARD;
        }
        else
        {
            enemyStatus = EnemyStatus.PATROL;
            GetNewWayPoint();//一开始给一个点，防止它巡逻到中点
        }
    }

    private void Update()
    {
        SwitchStatus();
        SwitchAnimation();
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }

    void SwitchStatus()
    {
        //如果发现player切换到CHASE
        if (FoundPlayer())
        {
            enemyStatus = EnemyStatus.CHASE;
        }

        switch (enemyStatus)
        {
            case EnemyStatus.GUARD:
                break;
            case EnemyStatus.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;//乘法比除法开销更小
                //判断是否到达巡逻点
                if (Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                        remainLookAtTime = lookAtTime;
                    }
                    
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStatus.CHASE:
                //TODO:在攻击范围内攻击
                //TODO:配合动画
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //TODO:拉脱回到上一个状态

                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        enemyStatus = isGuard ? EnemyStatus.GUARD : EnemyStatus.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }
                break;
            case EnemyStatus.DEAD:
                break;
        }
    }

    bool FoundPlayer()
    {
        //返回球形范围内所有碰撞体
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    void GetNewWayPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
        //NavMesh.SamplePosition(点，输出信息，查找范围，1代表walkable)
    }

    /// <summary>
    /// 帮助设置活动范围和视野范围
    /// </summary>
    private void OnDrawGizmosSelected()//选中物体在场景中画出区域
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//wireSphere :空心球体
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }

}
