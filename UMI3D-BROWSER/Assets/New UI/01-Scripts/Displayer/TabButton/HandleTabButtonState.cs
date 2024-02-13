using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTabButtonState : MonoBehaviour
{
    public enum currentActiveTabButton
    {
        leftTab,
        rightTab
    }

    [SerializeField] TabButton leftTabButton;
    [SerializeField] TabButton rightTabButton;

    public currentActiveTabButton currentActiveButton = currentActiveTabButton.leftTab;

    // Prevoir un last active tab

    void Update()
    {
       switch (currentActiveButton)
        {
            case currentActiveTabButton.leftTab:
                leftTabButton.label.color = leftTabButton.selectedLabelColor;
                leftTabButton.hoverBar.color = new Color(leftTabButton.hoverBar.color.r, leftTabButton.hoverBar.color.g, leftTabButton.hoverBar.color.b, 1);
                rightTabButton.label.color = rightTabButton.labelBaseColor;
                rightTabButton.hoverBar.color = new Color(rightTabButton.hoverBar.color.r, rightTabButton.hoverBar.color.g, rightTabButton.hoverBar.color.b, 0);

                break;

            case currentActiveTabButton.rightTab:
                rightTabButton.label.color = rightTabButton.selectedLabelColor;
                rightTabButton.hoverBar.color = new Color(rightTabButton.hoverBar.color.r, rightTabButton.hoverBar.color.g, rightTabButton.hoverBar.color.b, 1);
                leftTabButton.label.color = leftTabButton.labelBaseColor;
                leftTabButton.hoverBar.color = new Color(leftTabButton.hoverBar.color.r, leftTabButton.hoverBar.color.g, leftTabButton.hoverBar.color.b, 0);
                break;

            default:
                break;
        }
    }
}
