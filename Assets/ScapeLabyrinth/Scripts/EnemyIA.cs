using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    NavMeshAgent nm;
    public Transform target;
    public float distanceThreshold = 10f;
    public enum AIState { idle, chasing, attack};

    public AIState aistate = AIState.idle;

    public Animator animator;
    public float attackThreshold = 1.5f;


    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        StartCoroutine(Think());
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

   
    void Update()
    {
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
                    if(dist > attackThreshold)
                    {
                        aistate = AIState.chasing;
                        animator.SetBool("Attacking", false);
                    }
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
