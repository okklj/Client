using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMove : MonoBehaviour
{
    private NavMeshAgent agent;
    public float moveSpeed;
    public float roateSpeed;
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
        if (offset_z == -1) return;//·Àºó³·
        if (offset_x != 0 || offset_z != 0)
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
        }
        
    }
}
