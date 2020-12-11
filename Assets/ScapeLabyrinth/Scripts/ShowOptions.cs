using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class ShowOptions : MonoBehaviour
{
    public GameObject panelMenu;
    public string sceneName = "";
    public AudioSource m_AudioSource;
    //public Animator animator;

    void PauseGame()
    {
        m_AudioSource.mute = true;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        m_AudioSource.mute = false;
        Time.timeScale = 1;
    }

    public void activateOption()
    {
        panelMenu.SetActive(true);
        PauseGame();
    }

    public void disableOption()
    {
        panelMenu.SetActive(false);
        ResumeGame();
    }

    public void returnMenu()
    {
        ResumeGame();
        if (sceneName != "")
        {
            //StartCoroutine(LoadAsynchronously(sceneName));
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    public void exitGame()
    {
        Application.Quit();
    }

    /*public void attack()
    {
        animator.SetTrigger("Attack");
    }*/
}
