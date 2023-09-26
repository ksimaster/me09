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
    private float boostSpeed = 1f;

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
            _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, _animationSpeed * boostSpeed * Time.deltaTime);

            if (_currentValue < 1000) 
            {
                _textMesh.text = _currentValue.ToString("0") + " $"; ;
            } 
            else if(_currentValue > 1000 && _currentValue <= 1000000)
            {
                _textMesh.text = (_currentValue / 1000).ToString("0.0") + " K$";
                boostSpeed = 50f;
            }
            else if (_currentValue > 1000000 && _currentValue <= 1000000000)
            {
                _textMesh.text = (_currentValue / 1000000).ToString("0.0") + " M$";
                boostSpeed = 500f;
            }
            else if (_currentValue > 1000000000 && _currentValue <= 1000000000000)
            {
                _textMesh.text = (_currentValue / 1000000000).ToString("0.0") + " B$";
                boostSpeed = 5000f;
            }
            else if (_currentValue > 1000000000000 && _currentValue <= 1000000000000000)
            {
                _textMesh.text = (_currentValue / 1000000000000).ToString("0.0") + " KB$";
                boostSpeed = 50000f;
            }
            else if (_currentValue > 1000000000000000 && _currentValue <= 1000000000000000000)
            {
                _textMesh.text = (_currentValue / 1000000000000000).ToString("0.0") + " MB$";
                boostSpeed = 500000f;
            }
            else if (_currentValue > 1000000000000000000)
            {
                _textMesh.text = (_currentValue / 1000000000000000000).ToString("0.0") + " BB$";
                boostSpeed = 5000000f;
            }

            

            /*
            if (_currentValue >= 1000f)
            {
                _textMesh.text = string.Format(_formatString, (_currentValue / 1000f).ToString("0.0"));
            }
            else
            {
                _textMesh.text = string.Format(_formatString, _currentValue.ToString("0"));
            }
            */
        }
    }
}


