using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using inetum.unityUtils;

public class SimpleButton : Selectable
{
    public UnityEvent OnClick;
    public UnityEvent OnRelease;

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnClick.Invoke();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        OnRelease.Invoke();
    }
}
