using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("Step 1")]
    [SerializeField] private Button _freeCarButton;
    [SerializeField] private Transform _clickHand;
    [Header("Step 2")]
    [SerializeField] private Transform _dragHand;
    [SerializeField] private Transform _boardSlot;
    private Func<bool> GameStarted;

    private void Awake()
    {
        if(_clickHand.gameObject.activeInHierarchy)
        {
            _clickHand.gameObject.SetActive(false);
        }
        if (_dragHand.gameObject.activeInHierarchy)
        {
            _dragHand.gameObject.SetActive(false);
        }
    }

    public void StartTutorial(Func<bool> OnGameStarted)
    {
        GameStarted += OnGameStarted;
        _freeCarButton.onClick.AddListener(OnStepOneCompleted);
        StepOneBehaviour();
    }
    
    public void StepOneBehaviour()
    {
        _clickHand.transform.position = _freeCarButton.transform.position;
        _clickHand.gameObject.SetActive(true);
    }

    private void OnStepOneCompleted()
    {
        _clickHand.gameObject.SetActive(false);
        StartCoroutine(StepTwoRoutine());
    }

    private IEnumerator StepTwoRoutine()
    {
        var screenSpacePos = Camera.main.WorldToScreenPoint(_boardSlot.position);
        _dragHand.transform.position = screenSpacePos;
        _dragHand.gameObject.SetActive(true);
        yield return new WaitUntil(GameStarted);
        _dragHand.gameObject.SetActive(false);
        yield break;
    }

    private void OnDestroy()
    {
        GameStarted = null;
    }
}
