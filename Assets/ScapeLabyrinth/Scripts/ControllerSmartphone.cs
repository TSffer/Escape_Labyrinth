using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSmartphone : MonoBehaviour
{
    public float speed;
    public float angle;
    Rigidbody rb;

    private float y = 450;

    protected Queue<Vector3> filterDataQueue = new Queue<Vector3>();
    public int filterLength = 3;


    void Start()
    {
        speed = 0.0f;
        angle = 0.0f;
        //Screen.orientation = ScreenOrientation.LandscapeLeft;
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -98F, 0);

        for (int i = 0; i < filterLength; i++)
            filterDataQueue.Enqueue(Input.acceleration);
    }

    public void ControllAccelerometer()
    {
        speed = LowPassAccelerometer().y * 7.0f;
        angle = LowPassAccelerometer().x * 360.0f;

 /*       bool v = (LowPassAccelerometer().x == 0.3954459) && (LowPassAccelerometer().y == -0.01947658);
        if (v)
        {
            speed = 0.0f;
            angle = 0.0f;
        }
*/
        Vector3 movement = rb.transform.forward * Mathf.Min(speed, 5.0f) * 3.5f;
        Vector3 rotate = new Vector3(0.0f, Mathf.Pow(speed, 0.5f) * Mathf.Sin(Mathf.Deg2Rad * angle) * 0.8f, 0.0f);

        rb.velocity = movement;
        rb.transform.Rotate(rotate);
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
}
