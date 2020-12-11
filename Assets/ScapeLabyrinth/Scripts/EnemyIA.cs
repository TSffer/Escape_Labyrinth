using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    NavMeshAgent nm;
    public Transform target;
    public float distanceThreshold = 25.0f;
    public enum AIState { idle, chasing, attack, fall};
    public float StunnedTime = 3.0f;

    public AIState aistate = AIState.idle;

    public Animator animator;
    public float attackThreshold = 1.5f;
    private bool hit = false;

    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        StartCoroutine(Think());
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

   
    void Update()
    {
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            int NumberChildren = collision.transform.childCount;
         
            for (int n = 0; n < NumberChildren; n++)
            {
                if (collision.transform.GetChild(n).tag == "machete")
                {
                    Debug.Log("Exit called.");
                    hit = true;
                }
            }
        }
    }

    IEnumerator Think()
    {
        while(true)
        {
            switch(aistate)
            {
                case AIState.idle:
                    float dist = Vector3.Distance(target.position, transform.position);
                    if(dist < distanceThreshold)
                    {
                        aistate = AIState.chasing;
                        animator.SetBool("Chasing", true);
                    }
                    nm.SetDestination(transform.position);
                    break;
                case AIState.chasing:
                    dist = Vector3.Distance(target.position, transform.position);
                    if (dist > distanceThreshold)
                    {
                        aistate = AIState.idle;
                        animator.SetBool("Chasing", false);
                    }
                    if(dist < attackThreshold)
                    {
                        aistate = AIState.attack;
                        animator.SetBool("Attacking", true);
                    }
                    nm.SetDestination(target.position);
                    break;
                case AIState.attack:
                    nm.SetDestination(transform.position);
                    dist = Vector3.Distance(target.position, transform.position);
                    //animator.SetBool("Falling", false);
                    if (dist > attackThreshold)
                    {
                        aistate = AIState.chasing;
                        animator.SetBool("Attacking", false);
                    }
                    if(hit == true)
                    {
                        aistate = AIState.fall;
                    }
                    break;
                case AIState.fall:
                    animator.SetBool("Falling", true);
                    StunnedTime -= Time.deltaTime;
                    if (StunnedTime <= 0)
                    {
                        hit = false;
                        aistate = AIState.attack;
                        animator.SetBool("Falling", false);
                    }
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
