using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ShowRestart : MonoBehaviour
{
   // private AudioSource audio;
    void Start()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainScene");
    }
}
