using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates
    {
        HitPlayer,HitEnemy,HitNothing
    }
    [Header("Basic Setting")]
    public float lifeTime=5f;
    public float force;
    public int damage=8;
    public RockStates rockStates;
    public GameObject breakEffect;
    private Vector3 direction;

    private void Start()
    {
        rockStates = RockStates.HitPlayer;
    }
    private void FixedUpdate()
    {
        if (GetComponent<Rigidbody>() == null) return;
        if (GetComponent<Rigidbody>().velocity.sqrMagnitude <= 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f&&rockStates==RockStates.HitNothing)
        {
            Destroy(this.gameObject);
        }
    }

    public void FlyToTarget(GameObject target)
    {
        this.gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().velocity = Vector3.one;
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = target.transform.position - transform.position + Vector3.up;
        direction.Normalize();
        this.GetComponent<Rigidbody>().AddForce(direction*force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision coll)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (coll.gameObject.CompareTag("Player"))
                {
                    coll.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    coll.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    coll.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    CharacterStats.TakeDamge(damage, coll.gameObject.GetComponent<CharacterStats>());
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (coll.gameObject.CompareTag("Enemy"))
                {
                    var otherStats = coll.gameObject.GetComponent<CharacterStats>();
                    CharacterStats.TakeDamge(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
