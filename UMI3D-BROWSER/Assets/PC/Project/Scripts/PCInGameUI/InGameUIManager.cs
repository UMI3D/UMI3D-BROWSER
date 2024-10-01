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

using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3dBrowsers.displayer.ingame;
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

            if (inGameLinker.IsEnable == false)
            {
                ToggleInGamePanel();
                if (!debugMode)
                    gameObject.SetActive(inGameLinker.IsEnable);
                Cursor.lockState = CursorLockMode.Confined;
            }

            openCloseInGamePanel.performed += i => ToggleInGamePanel();
            inGamePanelLinker.OnOpenClosePanel += () => ToggleInGamePanel();
            inGameLinker.OnEnableDisableInGameUI += isEnable => gameObject.SetActive(isEnable);

            if (!debugMode)
                gameObject.SetActive(inGameLinker.IsEnable);

            BaseCursor.SetMovement(this, CursorMovement.Free);
        }

        private void OnEnable()
        {
            KeyboardShortcut.AddUpListener(ShortcutEnum.FreeCursor, FreeCursor);
            BaseCursor.SetMovement(this, CursorMovement.Center);
            BaseCursor.State = CursorState.Default;
        }

        private void OnDisable()
        {
            KeyboardShortcut.RemoveUpListener(ShortcutEnum.FreeCursor, FreeCursor);
            BaseCursor.SetMovement(this, CursorMovement.Free);
        }

        private void FreeCursor()
        {
            if (mainInGamePanel.gameObject.activeSelf)
                return;

            if (BaseCursor.Movement == CursorMovement.Center)
                BaseCursor.SetMovement(this, CursorMovement.Free);
            else
            {
                BaseCursor.SetMovement(this, CursorMovement.Center);
                BaseCursor.State = CursorState.Default;
            }
        }

        private void ToggleInGamePanel()
        {
            if (gameObject.activeSelf)
            {
                if (mainInGamePanel.gameObject.activeSelf)
                {
                    mainInGamePanel.gameObject.SetActive(false);
                    BaseCursor.SetMovement(this, CursorMovement.Center);
                    BaseCursor.State = CursorState.Default;
                }
                else
                {
                    mainInGamePanel.gameObject.SetActive(true);
                    BaseCursor.SetMovement(this, CursorMovement.Free);
                }
            }
        }
    }
}
