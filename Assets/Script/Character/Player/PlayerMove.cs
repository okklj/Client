using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMove : MonoBehaviour
{
    private NavMeshAgent agent;
    public float moveSpeed;
    public bool isStop;
    private float originSpeed;
    private float addSpeed;
    private void Awake()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        originSpeed = agent.speed;
        addSpeed = originSpeed + 2f;
    }
    private void FixedUpdate()
    {
        if (!isStop)
        {
            Move();
        }
    }

    private void Move()
    {
        if (GameManager.Instance.playerStats.GetComponent<PlayerController>().isDead) return;
        float offset_x = Input.GetAxisRaw("Horizontal");
        float offset_z = Input.GetAxisRaw("Vertical");
        if (offset_z == -1) return;//防后撤
        if (offset_x != 0 || offset_z != 0)
        {
            agent.isStopped = false;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                agent.speed = addSpeed;
            }
            else
            {
                agent.speed = originSpeed;
            }
            Vector3 vector3 = new Vector3(offset_x * moveSpeed, 0, offset_z * moveSpeed);
            Vector3 direction = transform.TransformPoint(vector3);//TransformPoint：transform就是相对的物体，括号里的就是相对这个transform的本地坐标
            agent.destination = direction;
        }
        else
            agent.isStopped = true ;
        #region 曾经运动代码
        /*if (offset_x != 0 || offset_z != 0)
        {
            offset_z = Mathf.Clamp(offset_z, 0, 1);

            offset_z += 0.01f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                offset_z += 3f;
            }
            offset_z += moveSpeed;
            moveDirection = new Vector3(0, 0, offset_z + moveSpeed);
            Vector3 current_direction = transform.TransformPoint(moveDirection) - transform.position;
            agent.velocity = current_direction;
            agent.transform.Rotate(new Vector3(0,offset_x*roateSpeed,0));
        }*/
        #endregion
    }
}
