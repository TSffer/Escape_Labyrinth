using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine;

public class ControlSystem : MonoBehaviour
{
    public Text canvastime;
    public Text canvaslife;
    public Text points;
    private float tmp;
    private float points_game;
    public Transform player;
    public Transform enemy;

    void Start()
    {
        tmp = 0.01f;
        points_game = 0.0f;
    }

    void Update()
    {
        tmp += Time.deltaTime;
        //canvastime.text = "Tiempo : " + tmp.ToString("f0") + " sec";
        Vector3 forward = player.TransformDirection(Vector3.forward);
        Vector3 toOther = enemy.position - player.position;

        if(Vector3.Dot(forward, toOther) < 0)
        {
            Debug.Log("The other transform is behind me!");
        }

    }
}
