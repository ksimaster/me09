using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;




public class AdShowManager : MonoBehaviour
{
    [SerializeField] [Min(60)] private int timerForAd; //>60
    [SerializeField] private GameObject timerObj; // ������ �� ������� ����� ����� � ��������
    [SerializeField] private TMP_Text timerText; // TextMeshPro ������� �� �������(����� � ��������������)

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
        timerText.text = "�� ������ ������� 3 �������";
        yield return new WaitForSeconds(1);
        timerText.text = "�� ������ ������� 2 �������";
        yield return new WaitForSeconds(1);
        timerText.text = "�� ������ ������� 1 �������";
        yield return new WaitForSeconds(1);
        StartCoroutine(AdShow());
        YandexGame.FullscreenShow();
        timerText.text = null;


    }

}