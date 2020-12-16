using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalSuccess : MonoBehaviour
{
    public Text finalTime;
    public Text points;
    public GameObject panelsucces;
    [SerializeField]  public GameObject[] panelinterfaz;
    private float pointsinitial = 0.0f;
    private float tmp = 0.0f;
    public AudioSource m_AudioSource;
    public PlayerController playercontroller;
    public bool goalSuccess = false;

    void Update()
    {
        tmp += Time.deltaTime;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            for(int i = 0; i < panelinterfaz.Length; i++)
            {
                panelinterfaz[i].SetActive(false);
            }
            //panelinterfaz.SetActive(false);
            goalSuccess = true;
            m_AudioSource.mute = true;
            Time.timeScale = 0;
            panelsucces.SetActive(true);
            points.text = "Puntuación : " + playercontroller.tmpstun.ToString("f0");
            finalTime.text = "Tiempo : " + tmp.ToString("f0") + " sec";
        }
    }
}
