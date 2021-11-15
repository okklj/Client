using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private GameObject attackTarget;//����Ŀ��
    private float lastAttackTime;//��ȴʱ��
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        MouseManager.Instance.OnEnemyClicked += AttackTarget;
    }
    private void Update()
    {
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }


    private void SwitchAnimation()
    {
        anim.SetFloat("Speed",Mathf.Abs(agent.velocity.sqrMagnitude));
    }

    private void AttackTarget(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
        }
        //TODO:�޸Ĺ���������Χ
        if (Vector3.Distance(attackTarget.transform.position, transform.position) > 3) return;
        //Attack
        if (lastAttackTime < 0&&Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Attack");
            //������ȴʱ��
            lastAttackTime = 0.5f;
        }
    }
}
