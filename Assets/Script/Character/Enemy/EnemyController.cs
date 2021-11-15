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
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyStatus enemyStatus;
    private Animator anim;

    [Header("Basic Settings")]
    public float sightRadius;//��Ұ��Χ
    public bool isGuard;
    private GameObject attackTarget;
    private float speed;//��¼ԭ���ٶ�

    [Header("Patrol State")]//Ѳ�߲���
    public float patrolRange;
    private Vector3 wayPoint;//�ڷ�Χ������ƶ���
    private Vector3 guardPos;//��ʼ��
    public float lookAtTime;//ֹͣ�۲�ʱ��
    private float remainLookAtTime;


    //��϶�������
    bool isWalk;
    bool isChase;//׷��
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
            GetNewWayPoint();//һ��ʼ��һ���㣬��ֹ��Ѳ�ߵ��е�
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
        //�������player�л���CHASE
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
                //TODO:�ڹ�����Χ�ڹ���
                //TODO:��϶���
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //TODO:���ѻص���һ��״̬

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

}
