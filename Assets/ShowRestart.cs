using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowRestart : MonoBehaviour
{
   // public GameObject resetButton;
   /*
    void Start()
    {
        resetButton = GameObject.Find("ResetButton");
        resetButton.SetActive(true);
    }

    
    void Update()
    {
        
    }
    */
    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainScene");
    }
}
