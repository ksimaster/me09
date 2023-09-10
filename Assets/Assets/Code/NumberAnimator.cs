using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    [SerializeField] private float _animationSpeed = 10f;
    [SerializeField] private string _formatString = "F2"; // format string for the resulting value

    private float _currentValue = 0.01f;
    private float _targetValue = 0f;

    public string AnimatedValueAsString { get { return _currentValue.ToString(_formatString); } }

    public void SetTargetValue(float targetValue)
    {
        _targetValue = targetValue;
    }

    public void Animate(float targetValue)
    {
        _targetValue = targetValue;
    }

    private void Update()
    {
        if (_currentValue != _targetValue)
        {
            _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, _animationSpeed * Time.deltaTime);

            if (_currentValue >= 1000f)
            {
                _textMesh.text = string.Format(_formatString, (_currentValue / 1000f).ToString("0.0") + "K");
            }
            else
            {
                _textMesh.text = string.Format(_formatString, _currentValue.ToString("0"));
            }
        }
    }
}


