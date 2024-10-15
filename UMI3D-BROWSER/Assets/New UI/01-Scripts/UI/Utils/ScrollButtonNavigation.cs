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

public class ScrollButtonNavigation : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private int visibleElements;

    private void OnEnable()
    {
        plusButton.onClick.AddListener(Plus);
        minusButton.onClick.AddListener(Minus);
        UpdateDisplay();
    }
    private void OnDisable()
    {
        plusButton.onClick.RemoveListener(Plus);
        minusButton.onClick.RemoveListener(Minus);
    }

    public void UpdateDisplay()
    {
        var moreElementVisible = content.childCount > visibleElements;
        plusButton.gameObject.SetActive(moreElementVisible);
        minusButton.gameObject.SetActive(moreElementVisible);
    }

    private void Plus()
    {
        if (content.childCount == 0)
            return;

        var tmp = content.childCount - visibleElements;
        if (tmp > 0)
            scrollBar.value = Mathf.Clamp01(scrollBar.value + (1.0f / tmp));
    }

    private void Minus()
    {
        if (content.childCount == 0)
            return;

        var tmp = content.childCount - visibleElements;
        if (tmp > 0)
            scrollBar.value = Mathf.Clamp01(scrollBar.value - (1.0f / tmp));
    }
}
