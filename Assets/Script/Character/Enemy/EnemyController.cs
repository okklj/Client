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
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameOberver
{
    private NavMeshAgent agent;
    private EnemyStatus enemyStatus;
    private Animator anim;
    protected CharacterStats characterStats;
    private Collider coll;

    [Header("Basic Settings")]
    public float sightRadius;//视野范围
    public bool isGuard;
    protected GameObject attackTarget;
    private float speed;//记录原来速度

    [Header("Patrol State")]//巡逻参数
    public float patrolRange;
    private Vector3 wayPoint;//在范围内随机移动点
    private Vector3 guardPos;//初始点
    public float lookAtTime;//停止观察时间
    private float remainLookAtTime;
    private float lastAttackTime;//攻击冷却

    private Quaternion guardRotation;//记录站桩时的方向

    //配合动画参数
    bool isWalk;
    bool isChase;//追逐
    bool isFollow;
    bool isDead;
    bool playerDead;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<BoxCollider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
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
        //TODO:场景切换后修改
        GameManager.Instance.AddObserver(this);
    }

    /*private void OnEnable()
    {
        GameManager.Instance.AddObserver(this);
    }*/
    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    private void Update()
    {
        if (characterStats.CurrentHealth <= 0)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwitchStatus();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
        
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStatus()
    {
        if (isDead)
        {
            enemyStatus = EnemyStatus.DEAD;
        }
        //如果发现player切换到CHASE
        else if (FoundPlayer())
        {
            enemyStatus = EnemyStatus.CHASE;
        }

        switch (enemyStatus)
        {
            case EnemyStatus.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);//lerp 角度缓慢变化，0.01为增量
                    }
                }
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
                //在攻击范围内攻击
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //拉脱回到上一个状态
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
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                //在攻击范围内攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }

                break;
            case EnemyStatus.DEAD:
                coll.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近身攻击
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //远程攻击
            anim.SetTrigger("Skill");
        }
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        return false;
    }


    //攻击范围检测
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        return false;
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


    //Animation Event
    void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            CharacterStats.TakeDamge(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        isChase = false;
        isWalk = false;
        playerDead = true;
        attackTarget = null;
        anim.SetBool("Win", true);
    }
}
