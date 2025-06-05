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
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.cursor
{
    public class LockModeContextCollection 
    {
        Dictionary<object, CursorLockMode> _lockModeByContextes = new();

        public CursorLockMode Set(object key, CursorLockMode value)
        {
            _lockModeByContextes[key] = value;
            return Map();
        }

        public CursorLockMode Unset(object key)
        {
            _lockModeByContextes.Remove(key);
            return Map();
        }

        public CursorLockMode Map()
        {
            CursorLockMode lockMode = CursorLockMode.None;

            foreach (var mode in _lockModeByContextes.Values)
            {
                bool lockedModeFound = false;

                switch (mode)
                {
                    case CursorLockMode.None:
                        break;

                    case CursorLockMode.Locked:
                        lockedModeFound = true;
                        break;

                    case CursorLockMode.Confined:
                        lockMode = CursorLockMode.Confined;
                        break;
                    
                    default:
                        break;
                }

                if (lockedModeFound) { break; }
            }

            return lockMode;
        }

        public void Clear()
        {
            _lockModeByContextes.Clear();
        }
    }
}