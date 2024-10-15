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

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using umi3dBrowsers.utils;

public class Badge : MonoBehaviour
{
    public TextMeshProUGUI BadgeText;
    [SerializeField] RectTransform textTransform;
    [SerializeField] RectTransform backgroundTransform;
    [SerializeField] float padding = 10f;
    [SerializeField] bool isHorizontal = true;
    [SerializeField] bool isVertical = true;

    private UIColliderScaller scaller;

    private void Awake()
    {
        if (BadgeText == null)
        {
            BadgeText = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (textTransform == null)
        {
            textTransform = BadgeText.transform as RectTransform;
        }
        if (backgroundTransform == null)
        {
            backgroundTransform = GetComponentInChildren<Image>().transform as RectTransform;
        }

        scaller = GetComponent<UIColliderScaller>();
    }

    [ContextMenu("UpdateUI")]
    private void OnGUI()
    {
        backgroundTransform.sizeDelta = new Vector2(isHorizontal ? textTransform.sizeDelta.x + 2f * padding : backgroundTransform.sizeDelta.x,
            isVertical ? textTransform.sizeDelta.y + 2f * padding : backgroundTransform.sizeDelta.y);
        scaller?.ScaleCollider();
    }
}