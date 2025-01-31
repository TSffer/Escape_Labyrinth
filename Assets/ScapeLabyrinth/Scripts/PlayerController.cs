﻿using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;
using System.Threading;
using EZCameraShake;
using System.Globalization;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0.0f;
    [SerializeField] private float angle;
    [SerializeField] private float lookSensitivity = 3f;

    [SerializeField] protected Queue<Vector3> filterDataQueue = new Queue<Vector3>();
    [SerializeField] private int filterLength = 3;
    [SerializeField] private float des = 0;

    [SerializeField] private AudioClip m_LandSound;
    [SerializeField] private AudioClip[] m_FootstepSounds;
    [SerializeField] private AudioClip[] m_HurtsSounds;
    public AudioClip m_DeadPlayer;
    public AudioClip m_PunchPlayer;

    public EnemyIA enemiyia;
    private Rigidbody rb;
    private float m_StepCycle;
    private AudioSource m_AudioSource;
    private CharacterController m_CharacterController;
    private bool m_IsWalking;
    private float m_RunStepLenghten;
    private float m_NextStep;
    private float m_StepInterval;
    //private Animator animator;
    public Text canvastime;
    public Text canvaslife;
    public Text canvaspoints;
    private float tmp = 0.0f;
    public float tmpstun = 0.0f;
    private float tmplife = 100.0f;
    private float points = 0.0f;
    private bool hit;
    //public AudioClip attackSound;
    public Transform target;
    public Camera camera;
    public GameObject enemy;
    public Animator animator;
    public bool controlhit = false;
    private float sleep = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_CharacterController = GetComponent<CharacterController>();
        //animator = GetComponentInChildren<Animator>();
        //enemy = GameObject.FindGameObjectWithTag("enemy");

        m_StepCycle = 0.0f;
        m_IsWalking = false;
        m_RunStepLenghten = 0.0f;
        m_NextStep = m_StepCycle / 2f;
        m_StepInterval = 0.03f;
        for (int i = 0; i < filterLength; i++)
            filterDataQueue.Enqueue(Input.acceleration);
    }

    void Update()
    {
        //speed = -Input.acceleration.z;
        m_RunStepLenghten = speed;
        speed = -LowPassAccelerometer().z;

        tmp += Time.deltaTime;
        canvastime.text = "Tiempo : " + tmp.ToString("f0") + " sec";
        hit = enemiyia.hit;

        if(hit == true)
        {
            tmpstun += Time.deltaTime;
            canvaspoints.text = "Puntuación : " + tmpstun.ToString("f0");
        }

        if(tmp >= 10.0f)
        {
            enemy.SetActive(true);
        }

        if(0.65f > speed)
        {
            speed = -1;
        }

        float dist = Vector3.Distance(target.position, transform.position);

        if (dist < 7.0f  && dist > 3.0f && tmp >= 10.0f && hit == false)
        {
            long[] pattern = { 0, 1000, 1000};
            Vibration.Vibrate(100);
          
            //Handheld.Vibrate();
        }

        if (dist < 3.0f && tmp >= 10.0f && hit == false)
        {
            long[] pattern = {1000, 1000 };
            Vibration.Vibrate(500);

            //Handheld.Vibrate();
        }
        
        if(tmplife >= 0.0f)
        {
            float velocity = speed * 2.5f;
            rb.velocity = rb.transform.forward * velocity;
           
            angle = LowPassAccelerometer().x * 15.0f;

            Vector3 rotation = new Vector3(0.0f, angle, 0.0f);
            rb.transform.Rotate(rotation);

            ProgressStepCycle(speed);
        }

        if (dist < 1.5f && tmp >= 10.0f && hit == false)
        {
            //StartCoroutine(camerashake.Shake(0.15f, 0.4f)); 
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 0.1f);
            tmplife -= Time.deltaTime*10;
            canvaslife.text = "Salud : " + tmplife.ToString("f0");

            m_StepCycle = m_StepCycle + m_StepInterval;

            //m_NextStep = m_StepCycle + m_StepInterval;
            if (m_StepCycle > 0.3f)
            {
                int n = Random.Range(1, m_HurtsSounds.Length);
                m_AudioSource.clip = m_HurtsSounds[n];
                m_AudioSource.PlayOneShot(m_AudioSource.clip);

                m_HurtsSounds[n] = m_HurtsSounds[0];
                m_HurtsSounds[0] = m_AudioSource.clip;
                m_StepCycle = 0.0f;
            }

            if (tmplife <= 0.0f)
            {
                speed = 0.0f;
                tmplife = 0.0f;
                m_AudioSource.clip = m_DeadPlayer;
                m_AudioSource.PlayOneShot(m_AudioSource.clip);

                m_AudioSource.mute = true;
                transform.rotation = Quaternion.Euler(80.0f, 0.0f, 0.0f);
                camera.transform.rotation = Quaternion.Euler(-150.0f, 0.0f, 0.0f);
                //Time.timeScale = 0;
                sleep += Time.deltaTime;
                //if(sleep >= 1)
                //{
                    SceneManager.LoadScene("Main_Scene");
                    sleep = 0.0f;
                //}
            }
        }

    }

    private void ProgressStepCycle(float speed)
    {        
        m_StepCycle = m_StepCycle + m_StepInterval;
 
        //m_NextStep = m_StepCycle + m_StepInterval;
        if(m_StepCycle > 0.7f)
        {
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
            m_StepCycle = 0.0f;
        }
    }

    public void attack()
    {
        animator.SetTrigger("Attack");
        controlhit = true;
        m_AudioSource.clip = m_PunchPlayer;
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
    }

    public Vector3 LowPassAccelerometer()
    {
        if (filterLength <= 0)
            return Input.acceleration;

        filterDataQueue.Enqueue(Input.acceleration);
        filterDataQueue.Dequeue();

        Vector3 vFiltered = Vector3.zero;
        foreach (Vector3 v in filterDataQueue)
            vFiltered += v;
        vFiltered /= filterLength;
        return vFiltered;
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        //if (sceneName != "")
        //{
            //StartCoroutine(LoadAsynchronously("Main_Scene"));
        SceneManager.LoadScene("Main_Scene", LoadSceneMode.Single);
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        //}
        //else
        //    return;

        PlayerData data = SaveSystem.LoadPlayer();
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        transform.position = position;
    }

}