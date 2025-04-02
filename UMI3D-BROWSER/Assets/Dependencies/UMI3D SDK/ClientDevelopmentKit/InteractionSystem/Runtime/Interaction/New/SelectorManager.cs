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

using inetum.unityUtils.observation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

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

        Delegates<ISelectorDelegate> _delegates = new();
        public Delegates<ISelectorDelegate> delegates => _delegates;

        List<Selector> _selectors = new List<Selector>();
        ReadOnlyCollection<Selector> selectors => _selectors.AsReadOnly();

        public Selector InstantiateSelector()
        {
            Selector selector = new();
            _selectors.Add(selector);

            return selector;
        }
    }
}