using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25f;
    public GameObject rockPrefab;
    public Transform handPos;
    public float throwForce=10f;
    private GameObject rock;
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            //击退
            var direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            CharacterStats.TakeDamge(characterStats, targetStats);
        }
    }


    public void GenerateRock()
    {
        //rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);//维持原来的旋转
        if(rock==null)
        {
            rock = Instantiate(rockPrefab, handPos, false);
            rock.GetComponent<Rock>().enabled = false;
        }
            
    }
    public void ThrowRock()
    {
        if (attackTarget != null && rock != null)
        {
            rock.GetComponent<Rock>().enabled = true;
            rock.transform.SetParent(null);
            rock.GetComponent<Rock>().force = (float)(kickForce * 0.6);
            rock.GetComponent<Rock>().FlyToTarget(attackTarget);
            rock = null;
        }
    }
}
