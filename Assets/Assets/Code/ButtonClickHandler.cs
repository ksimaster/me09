using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ButtonClickHandler : MonoBehaviour
{
    public delegate void OnMouseDownDelegate();
    public event OnMouseDownDelegate OnMouseDownEvent;

    public delegate void OnMouseUpDelegate();
    public event OnMouseUpDelegate OnMouseUpEvent;

    void OnMouseDown()
    {
        if (OnMouseDownEvent != null)
        {
            OnMouseDownEvent();
        }
    }

    void OnMouseUp()
    {
        if (OnMouseUpEvent != null)
        {
            OnMouseUpEvent();
        }
    }
}