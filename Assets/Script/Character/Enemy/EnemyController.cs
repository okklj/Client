using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStatus
{
    GUARD, PATROL, CHASE, DEAD
}

//�Զ�������
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
    public float sightRadius;//��Ұ��Χ
    public bool isGuard;
    protected GameObject attackTarget;
    private float speed;//��¼ԭ���ٶ�

    [Header("Patrol State")]//Ѳ�߲���
    public float patrolRange;
    private Vector3 wayPoint;//�ڷ�Χ������ƶ���
    private Vector3 guardPos;//��ʼ��
    public float lookAtTime;//ֹͣ�۲�ʱ��
    private float remainLookAtTime;
    private float lastAttackTime;//������ȴ

    private Quaternion guardRotation;//��¼վ׮ʱ�ķ���

    //��϶�������
    bool isWalk;
    bool isChase;//׷��
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
            GetNewWayPoint();//һ��ʼ��һ���㣬��ֹ��Ѳ�ߵ��е�
        }
        //TODO:�����л����޸�
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
        //�������player�л���CHASE
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
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);//lerp �ǶȻ����仯��0.01Ϊ����
                    }
                }
                break;
            case EnemyStatus.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;//�˷��ȳ���������С
                //�ж��Ƿ񵽴�Ѳ�ߵ�
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
                //�ڹ�����Χ�ڹ���
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //���ѻص���һ��״̬
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

                //�ڹ�����Χ�ڹ���
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //�����ж�
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //ִ�й���
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
            //������
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //Զ�̹���
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


    //������Χ���
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
        //�������η�Χ��������ײ��
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
        //NavMesh.SamplePosition(�㣬�����Ϣ�����ҷ�Χ��1����walkable)
    }

    /// <summary>
    /// �������û��Χ����Ұ��Χ
    /// </summary>
    private void OnDrawGizmosSelected()//ѡ�������ڳ����л�������
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//wireSphere :��������
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
