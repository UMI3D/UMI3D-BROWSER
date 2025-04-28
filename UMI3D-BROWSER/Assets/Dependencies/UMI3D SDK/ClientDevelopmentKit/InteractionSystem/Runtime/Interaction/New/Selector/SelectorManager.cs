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
using System.Collections.ObjectModel;

namespace umi3d.cdk.interaction
{
    public class SelectorManager 
    {
        #region Initialize

        static Lazy<SelectorManager> _default = new(() => new());
        public static SelectorManager @default => _default.Value;

        SelectorManager()
        {
        }

        #endregion

        public ISelector serverSelector;

        public Selector lastSelectorUsed { get; internal set; }
        public Selector lastSelectorSelected { get; internal set; }
        public Selector lastSelectorDeselected { get; internal set; }

        List<Selector> _selectors = new List<Selector>();
        public ReadOnlyCollection<Selector> selectors => _selectors.AsReadOnly();

        public bool TryToInstantiateSelector(out Selector selector, string id)
        {
            selector = _selectors.Find(selector => selector.id == id);
            if (selector != null)
            {
                UnityEngine.Debug.LogWarning($"[SelectorManager] Warning: Cannot instantiate selector for '{id}' because this selector already exist.");
                return false;
            }

            selector = new(id);
            _selectors.Add(selector);
            return true;
        }

        public bool TryToGetSelector(out Selector selector, string id)
        {
            selector = _selectors.Find(selector => selector.id == id);
            if (selector == null)
            {
                UnityEngine.Debug.LogWarning($"[SelectorManager] Warning: selector for '{id}' has not been instantiated yet.");
                return false;
            }

            return true;
        }
    }
}