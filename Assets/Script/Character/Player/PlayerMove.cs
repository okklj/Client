using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMove : MonoBehaviour
{
    private NavMeshAgent agent;
    public float moveSpeed;
    private void Awake()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveDirection;
        float offset_x = Input.GetAxisRaw("Horizontal");
        float offset_z = Input.GetAxisRaw("Vertical");
        if (offset_z < 0) return;//·ÀÖ¹ºó³·³é´¤
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection = new Vector3(offset_x, 0, offset_z + moveSpeed + 3f)*2;
        }
        else
        {
            moveDirection = new Vector3(offset_x, 0, offset_z + moveSpeed)*2;
        }
        Vector3 current_direction = transform.TransformPoint(moveDirection) - transform.position;
        if (offset_x != 0 || offset_z != 0)
        {
            agent.velocity = current_direction;
        }
    }
}
