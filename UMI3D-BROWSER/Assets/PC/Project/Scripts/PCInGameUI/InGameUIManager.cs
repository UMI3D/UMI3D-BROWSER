using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3dBrowsers.linker.ingameui;
using UnityEngine;
using UnityEngine.InputSystem;
using static umi3d.baseBrowser.cursor.BaseCursor;

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



            KeyboardShortcut.AddUpListener(ShortcutEnum.FreeCursor, () => {
                if (mainInGamePanel.gameObject.activeSelf)
                    return;

                if (BaseCursor.Movement == CursorMovement.Center)
                    BaseCursor.SetMovement(this, CursorMovement.Free);
                else
                {
                    BaseCursor.SetMovement(this, CursorMovement.Center);
                    BaseCursor.State = CursorState.Default;
                }
            });
        }

        private void ToggleInGamePanel()
        {
            if (gameObject.activeSelf)
            {
                if (mainInGamePanel.gameObject.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    BaseCursor.SetMovement(this, CursorMovement.Center);
                    mainInGamePanel.gameObject.SetActive(false);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    BaseCursor.SetMovement(this, CursorMovement.Free);
                    mainInGamePanel.gameObject.SetActive(true);
                }
            }
        }
    }
}