using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.displayer;
using umi3dBrowsers.linker;
using UnityEngine;

namespace umi3dBrowsers.ingame_ui.notifications
{
    public class NotifIndicator : MonoBehaviour
    {
        [SerializeField] private ButtonStyle buttonStyle;
        [Header("Linker")]
        [SerializeField] private NotifIndicatorLinker linker;

        [Header("Color")]
        [SerializeField] private Color overrideColor;
        private Color _normalColor;
        private Color _hoverColor;
        private Color _selectedColor;

        private void Awake()
        {
            _normalColor = buttonStyle.NormalColor;
            _hoverColor = buttonStyle.HoverColor;
            _selectedColor = buttonStyle.SelectedColor;

            if (linker.hasNotifications)
            {
                OverrideColors();
            }

            linker.OnHasNotifications += () => OverrideColors();
            linker.OnHasNoNotifications += () => ResetColors();
        }

        private void ResetColors()
        {
            buttonStyle.SetNormalColor(_normalColor);
            buttonStyle.SetHoverColor(_hoverColor);
            buttonStyle.SetSelectedColor(_selectedColor);

            buttonStyle.applyColor();
        }

        private void OverrideColors()
        {
            buttonStyle.SetNormalColor(overrideColor);
            buttonStyle.SetHoverColor(overrideColor);
            buttonStyle.SetSelectedColor(overrideColor);

            buttonStyle.applyColor();
        }
    }
}

