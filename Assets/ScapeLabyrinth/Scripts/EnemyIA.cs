using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyIA : MonoBehaviour
{
    NavMeshAgent nm;
    public Transform target;
    public float distanceThreshold = 25.0f;
    public enum AIState { idle, chasing, attack, fall};
    public float StunnedTime = 3.0f;

    public PlayerController playercontroller;
    public GoalSuccess[] goal;
    public AIState aistate = AIState.idle;
    public Animator animator;
    public float attackThreshold = 1.5f;
    public bool hit = false;
    public AudioClip[] m_Sounds;
    public AudioClip idlem_Sounds;
    private AudioSource m_AudioSource;
    private float m_StepCycle;
    private float m_StepInterval;

    void Start()
    {
        m_StepCycle = 0.0f;
        m_StepInterval = 0.03f;
        m_AudioSource = GetComponent<AudioSource>();
        nm = GetComponent<NavMeshAgent>();
        StartCoroutine(Think());
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

   
    void Update()
    {
        ProgressSoundCycle();

        for (int i = 0; i < goal.Length; i++)
        {
            if (goal[i].goalSuccess)
            {
                m_AudioSource.mute = true;
            }
        }
    }

    private void ProgressSoundCycle()
    {
        m_StepCycle = m_StepCycle + m_StepInterval;
        if (m_StepCycle > 1.5f)
        {
            int n = Random.Range(1, m_Sounds.Length);
            m_AudioSource.clip = m_Sounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            m_Sounds[n] = m_Sounds[0];
            m_Sounds[0] = m_AudioSource.clip;
            m_StepCycle = 0.0f;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && playercontroller.controlhit)
        {
            int NumberChildren = collision.transform.childCount;
         
            for (int n = 0; n < NumberChildren; n++)
            {
                if (collision.transform.GetChild(n).tag == "machete")
                {
                    Debug.Log("Hit player.");
                    hit = true;
                }
            }
            playercontroller.controlhit = false;
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
                    //m_AudioSource.mute = true;
                    
                    m_StepCycle = m_StepCycle + m_StepInterval;
                    if (m_StepCycle > 2.0f)
                    {
                        m_AudioSource.PlayOneShot(idlem_Sounds);
                        m_StepCycle = 0.0f;
                    }

                    //m_AudioSource.PlayOneShot(m_AudioSource.clip);
                    if (StunnedTime <= 0)
                    {
                        aistate = AIState.attack;
                        hit = false;
                        StunnedTime = 3.0f;
                        animator.SetBool("Falling", false);
                        //m_AudioSource.mute = false;
                    }
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
