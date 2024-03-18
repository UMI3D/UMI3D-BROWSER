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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDisplayer : MonoBehaviour
{
    private TextMeshProUGUI textDisplayer;
    private RectTransform rectTransform;

    private void Awake()
    {
        textDisplayer = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitComponentData(string text = "null", Color? textColor = null, float textSize = 16, float width = 100f, float height = 100f, float positionX = 0f, float positionY = 0f)
    {
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = new Vector2(positionX, positionY);
        textDisplayer.text = text;
        textDisplayer.color = textColor ?? Color.white;
        textDisplayer.fontSize = textSize;
    }
}
