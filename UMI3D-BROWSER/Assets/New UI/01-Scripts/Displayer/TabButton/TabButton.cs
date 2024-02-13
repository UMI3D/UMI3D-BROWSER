using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

public class TabButton : Selectable
{
    public Image hoverBar;
    public TextMeshProUGUI label;
    public Color selectedLabelColor;
    public float animationTime = 0.3f;

    public Color labelBaseColor;
    public bool isSelected;

    protected override void Awake()
    {
        base.Awake();
        labelBaseColor = label.color;
    }

    protected void Update()
    {
        if (currentSelectionState == SelectionState.Selected)
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (IsInteractable() && navigation.mode != Navigation.Mode.None && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject, eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        EnableHoverBarFX();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        DisableHoverBarFX();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        label.color = selectedLabelColor;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        label.color = labelBaseColor;
        hoverBar.color = new Color(hoverBar.color.r, hoverBar.color.g, hoverBar.color.b, 0);
    }

    public void EnableHoverBarFX()
    {
       StartCoroutine(HoverBarAnimation(0, 1));      
    }

    public void DisableHoverBarFX()
    {
        if (!isSelected)
        {
            StartCoroutine(HoverBarAnimation(1, 0));
        }
    }

    private IEnumerator HoverBarAnimation(float to, float at)
    {
        float timer = 0f;
        while (timer < animationTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(to, at, timer / animationTime);
            hoverBar.color = new Color(hoverBar.color.r, hoverBar.color.g, hoverBar.color.b, alpha);
            yield return null;
        }
    }
}
