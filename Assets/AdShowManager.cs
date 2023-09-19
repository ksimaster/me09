using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;




public class AdShowManager : MonoBehaviour
{
    [SerializeField] [Min(60)] private int timerForAd; //>60
    [SerializeField] private GameObject timerObj; // Канвас на котором весит текст с таймером
    [SerializeField] private TMP_Text timerText; // TextMeshPro элемент на канвасе(текст о предупреждении)

    private void Awake()
    {
        timerText.text = null;
    }

    void Start()
    {
        timerObj.SetActive(true);
        StartCoroutine(AdShow());
    }

    private IEnumerator AdShow()
    {
        yield return new WaitForSeconds(timerForAd);
        StartCoroutine(AdShowHelper());
    }

    IEnumerator AdShowHelper()
    {
        //timerObj.SetActive(true);
        timerText.text = "До показа рекламы 3 секунды";
        yield return new WaitForSeconds(1);
        timerText.text = "До показа рекламы 2 секунды";
        yield return new WaitForSeconds(1);
        timerText.text = "До показа рекламы 1 секунды";
        yield return new WaitForSeconds(1);
        StartCoroutine(AdShow());
        YandexGame.FullscreenShow();
        timerText.text = null;


    }

}