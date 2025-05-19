/*
Copyright 2019 - 2025 Inetum

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

using System;
using umi3d.browserRuntime.pc;
using UnityEngine;

namespace umi3d.browserRuntime.cursor
{
    public class PCCursorModel : CursorModel
    {
        LockModeContextCollection _lockModeContextCollection = new();

        public PCCursorModel()
        {
            UMI3DPCManager.@default.cursorModel = this;
        }

        public override void Clear()
        {
            _lockModeContextCollection.Clear();
            Reset();
        }

        public override void Reset()
        {
            CursorLockMode lockMode = _lockModeContextCollection.Map();

            SetLockMode(lockMode);
            switch (lockMode)
            {
                case CursorLockMode.None:
                case CursorLockMode.Confined:
                    SetVisible(true);
                    break;

                case CursorLockMode.Locked:
                    SetVisible(false);
                    break;

                default:
                    break;
            }
        }

        public override void SetInteractingState(CursorInteractingState interactingState)
        {
            this.interactingState = interactingState;
            NotifyInteractingStateObserver();
        }

        public override void SetLockMode(object context, CursorLockMode lockMode)
        {
            switch (lockMode)
            {
                case CursorLockMode.None:
                    lockMode = _lockModeContextCollection.Unset(context);
                    break;

                case CursorLockMode.Locked:
                case CursorLockMode.Confined:
                    lockMode = _lockModeContextCollection.Set(context, lockMode);
                    break;

                default:
                    break;
            }

            SetLockMode(lockMode);
            switch (lockMode)
            {
                case CursorLockMode.None:
                case CursorLockMode.Confined:
                    SetVisible(true);
                    break;

                case CursorLockMode.Locked:
                    SetVisible(false);
                    break;

                default:
                    break;
            }
        }

        public override void SetLockMode(CursorLockMode lockMode)
        {
            this.lockMode = lockMode;

            Cursor.lockState = lockMode;

            NotifyLockModeObserver();
        }

        public override void SetVisible(bool visible)
        {
            this.isVisible = visible;

            Cursor.visible = visible;

            NotifyVisibilityObserver();
        }
    }
}