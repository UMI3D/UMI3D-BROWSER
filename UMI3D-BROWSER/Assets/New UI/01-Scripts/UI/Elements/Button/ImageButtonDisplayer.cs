/*
Copyright 2019 - 2024 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/


using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageButtonDisplayer : SimpleButton
{
    private Image imageDisplayer;
    private TextMeshProUGUI textDisplayer;
    private RectTransform rectTransform;

    public event Action<PointerEventData> OnHoverEnter;
    public event Action<PointerEventData> OnHoverExit;

    protected override void Awake()
    {
        base.Awake();
        imageDisplayer = GetComponent<Image>();
        textDisplayer = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public override void OnPointerEnter(PointerEventData eventData) 
    {
        base.OnPointerEnter(eventData);
        OnHoverEnter?.Invoke(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        OnHoverExit?.Invoke(eventData);
    }

    public void InitComponentData(Image sprite = null, Color? spriteColor = null, string text = "", Color? textColor = null, float textSize = 16, float width = 100f, float height = 100f, float positionX = 0f, float positionY = 0f)
    {
        imageDisplayer = sprite;
        imageDisplayer.color = spriteColor ?? Color.white;
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = new Vector2(positionX, positionY);
        textDisplayer.text = text;
        textDisplayer.color = textColor ?? Color.white;
        textDisplayer.fontSize = textSize;
    }
}