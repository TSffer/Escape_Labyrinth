using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private float speed = 20.0f;
    private float angle;
    private float lookSensitivity = 3f;

    private Rigidbody rb;

    private PlayerMotor motor;

    protected Queue<Vector3> filterDataQueue = new Queue<Vector3>();
    public int filterLength = 3;
    GameObject hudText;
    private Text txt;
    private float des = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        motor = GetComponent<PlayerMotor>();

        for (int i = 0; i < filterLength; i++)
            filterDataQueue.Enqueue(Input.acceleration);
    }

    void Update()
    {
        speed = -Input.acceleration.z;
        
        if(0.72f > speed)
        {
            speed = -1;
        }
        
        Debug.Log(speed);
        float velocity = speed * 2.5f;
        rb.velocity = rb.transform.forward * velocity;

        angle = Input.acceleration.x * 20.0f;

        Vector3 rotation = new Vector3(0.0f, angle, 0.0f);
        rb.transform.Rotate(rotation);
    }

    /*void Update()
    {
        float temp = Input.acceleration.x;
        float z = Input.acceleration.z;
        transform.Translate(0 , 0,  -z * 5f * Time.deltaTime);
        transform.Rotate(0, -temp * speed, 0);
    }*/

    /*void FixedUpdate()
    {
        Vector3 movement = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
        rb.AddForce(movement * speed * Time.deltaTime);
    }*/

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
}