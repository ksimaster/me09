using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIColorToggler : MonoBehaviour
{

    [SerializeField] private Image image;
    [SerializeField] private TMPro.TMP_Text text;
    [SerializeField] private Color finalColor;

    private Color startColor;
    UnityAction<bool> ToggleUIElementColor;

    private void Awake()
    {
        if(image != null)
        {
            startColor = image.color;
            ToggleUIElementColor += ToggleImageColor;
        }
        else if(text != null)
        {
            startColor = text.color;
            ToggleUIElementColor += ToggleTextColor;
        }
    }

    private void ToggleImageColor(bool val)
    {
        image.color = (val) ? startColor : finalColor;
    }

    private void ToggleTextColor(bool val)
    {
        text.color = (val) ? startColor : finalColor;
    }

    public void ToggleColor(bool val)
    {
        ToggleUIElementColor?.Invoke(val);
    }

    private void OnDestroy()
    {
        ToggleUIElementColor = null;
    }
}
