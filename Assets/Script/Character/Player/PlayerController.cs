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
    private CharacterStats characterStats;

    private GameObject attackTarget;//攻击目标
    private float lastAttackTime;//冷却时间

    public bool isDead;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }
    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }
    private void OnEnable()
    {
        GameManager.Instance.RigisterPlayer(this.characterStats);
        MouseManager.Instance.OnEnemyClicked += AttackTarget;
    }

    private void OnDisable()
    {
        //人物重新加载一定要取消订阅。
        MouseManager.Instance.OnEnemyClicked -= AttackTarget;
    }
    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        if (isDead)
        {
            GameManager.Instance.NotifyObserver();
        }
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }


    private void SwitchAnimation()
    {
        anim.SetFloat("Speed",Mathf.Abs(agent.velocity.sqrMagnitude));
        anim.SetBool("Death", isDead);
    }

    private void AttackTarget(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
        }
        if (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange) return;
        if (!transform.IsFacingTarget(target.transform)) return;
        //Attack
        if (lastAttackTime < 0&&Input.GetKeyDown(KeyCode.Mouse0))
        {
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget == null) return;
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
            }
        }
        else
        {
            if (attackTarget.GetComponent<CharacterStats>().CurrentHealth <= 0) return;
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            CharacterStats.TakeDamge(characterStats, targetStats);
        }
    }
}
