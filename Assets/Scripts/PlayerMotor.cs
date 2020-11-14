using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private float velocity = 0;
    private Vector3 rotation = Vector3.zero;
    private Rigidbody rb;

    private float distance;

    public float time = 200;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(float _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    void Update()
    {
        time -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        //txt.text = "Distancia: " + (int)distance + "m || Speedd: " + velocity + " km/h";
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        /*if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        */
        distance += Mathf.Min(velocity, 5.0f) / 34;
        rb.velocity = rb.transform.forward * velocity;
    }

    void PerformRotation()
    {
        rb.transform.Rotate(rotation);
    }
}
