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

using GLTFast.Schema;
using UnityEngine;
using UnityEngine.UI;

public class HandleTabButtonState : MonoBehaviour
{
    public enum currentActiveTabButton
    {
        leftTab,
        rightTab
    }

    [SerializeField] TabButton leftTabButton;
    [SerializeField] TabButton rightTabButton;

    [SerializeField] GameObject container1;
    [SerializeField] GameObject container2;

    public currentActiveTabButton currentActiveButton = currentActiveTabButton.leftTab;

    private void Start()
    {
        SetSelectedButton();
        leftTabButton.OnClick.AddListener(SelectLeftButton);
        rightTabButton.OnClick.AddListener(SelectRightButton);
    }

    private void SelectLeftButton()
    {
        rightTabButton.isSelected = false;
        currentActiveButton = currentActiveTabButton.leftTab;
        SetSelectedButton();
    }

    private void SelectRightButton()
    {
        leftTabButton.isSelected = false;
        currentActiveButton = currentActiveTabButton.rightTab;
        SetSelectedButton();
    }

    private void SetSelectedButton()
    {
        switch (currentActiveButton)
        {
            case currentActiveTabButton.leftTab:

                leftTabButton.isSelected = true;
                leftTabButton.label.color = leftTabButton.selectedLabelColor;
                leftTabButton.hoverBar.color = new Color(leftTabButton.hoverBar.color.r, leftTabButton.hoverBar.color.g, leftTabButton.hoverBar.color.b, 1);

                rightTabButton.label.color = rightTabButton.labelBaseColor;
                rightTabButton.hoverBar.color = new Color(rightTabButton.hoverBar.color.r, rightTabButton.hoverBar.color.g, rightTabButton.hoverBar.color.b, 0);

                container1.SetActive(true);
                container2.SetActive(false);

                break;

            case currentActiveTabButton.rightTab:

                rightTabButton.isSelected = true;
                rightTabButton.label.color = rightTabButton.selectedLabelColor;
                rightTabButton.hoverBar.color = new Color(rightTabButton.hoverBar.color.r, rightTabButton.hoverBar.color.g, rightTabButton.hoverBar.color.b, 1);

                leftTabButton.label.color = leftTabButton.labelBaseColor;
                leftTabButton.hoverBar.color = new Color(leftTabButton.hoverBar.color.r, leftTabButton.hoverBar.color.g, leftTabButton.hoverBar.color.b, 0);

                container2.SetActive(true);
                container1.SetActive(false);
                break;

            default:
                break;
        }
    }

}
