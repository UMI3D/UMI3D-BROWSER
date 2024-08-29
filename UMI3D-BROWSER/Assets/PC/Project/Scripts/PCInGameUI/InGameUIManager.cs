using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.linker.ingameui;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3dBrowsers.ingame_ui
{
    public class InGameUIManager : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputAction openCloseInGamePanel;

        [Header("Dependencies")]
        [SerializeField] private MainInGamePanel mainInGamePanel;

        [Header("Linkers")]
        [SerializeField] private InGamePanelLinker inGamePanelLinker;
        [SerializeField] private InGameLinker inGameLinker;

        private void Awake()
        {
            openCloseInGamePanel.Enable();
            openCloseInGamePanel.performed += i => ToggleInGamePanel();
            inGamePanelLinker.OnOpenClosePanel += () => ToggleInGamePanel();
            inGameLinker.OnEnableDisableInGameUI += isEnable => gameObject.SetActive(isEnable);
            gameObject.SetActive(inGameLinker.IsEnable);
        }

        private void ToggleInGamePanel()
        {
            if (gameObject.activeSelf)
            {
                if (mainInGamePanel.gameObject.activeSelf)
                {
                    mainInGamePanel.gameObject.SetActive(false);
                }
                else
                {
                    mainInGamePanel.gameObject.SetActive(true);
                }
            }
        }
    }
}
