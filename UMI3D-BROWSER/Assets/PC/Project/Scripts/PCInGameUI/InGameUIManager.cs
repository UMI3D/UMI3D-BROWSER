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

        [Header("Debug")]
        [SerializeField] private bool debugMode;

        private void Awake()
        {
            openCloseInGamePanel.Enable();
            openCloseInGamePanel.performed += i => ToggleInGamePanel();
            inGamePanelLinker.OnOpenClosePanel += () => ToggleInGamePanel();
            inGameLinker.OnEnableDisableInGameUI += isEnable => gameObject.SetActive(isEnable);
            if (!debugMode)
                gameObject.SetActive(inGameLinker.IsEnable);
        }

        private void ToggleInGamePanel()
        {
            if (gameObject.activeSelf)
            {
                if (mainInGamePanel.gameObject.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    mainInGamePanel.gameObject.SetActive(false);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    mainInGamePanel.gameObject.SetActive(true);
                }
            }
        }
    }
}
