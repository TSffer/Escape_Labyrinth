using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalSuccess : MonoBehaviour
{
    public Text finalTime;
    public Text points;
    public GameObject panelsucces;
    private float pointsinitial = 600.0f;
    private float tmp = 0.0f;
    public AudioSource m_AudioSource;


    void Update()
    {
        tmp += Time.deltaTime;
        pointsinitial -= Time.deltaTime;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_AudioSource.mute = true;
            Time.timeScale = 0;
            panelsucces.SetActive(true);
            finalTime.text = "Tiempo : " + tmp.ToString("f0") + " sec";
            points.text = "Puntuación : " + pointsinitial.ToString("f0");
        }
    }
}
