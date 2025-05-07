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

namespace umi3d.browserRuntime.ui.dropdown
{
    public class SimpleDropdownFactory : DropdownFactory
    {
        protected override void Awake()
        {
            base.Awake();

            _simpleCreateBehaviour = new SimpleCreateBehaviour(this);
        }
    }

    public class SimpleCreateBehaviour : ISimpleCreateBehaviour
    {
        DropdownFactory dropdownFactory;

        public SimpleCreateBehaviour(DropdownFactory dropdownFactory)
        {
            this.dropdownFactory = dropdownFactory;
        }

        public bool TryToGetOrCreate(out GameObject control, Transform parent, string label, List<string> options, string value)
        {
            if (!dropdownFactory._lstDropdownsAvailable.TryDequeue(out var dropdownModelContainer))
            {
                dropdownModelContainer = GameObject.Instantiate(dropdownFactory._dropdownPrefab);
            }
            control = dropdownModelContainer.gameObject;
            dropdownModelContainer.gameObject.SetActive(true);
            dropdownModelContainer.transform.SetParent(parent, false);

            if (!string.IsNullOrEmpty(label))
            {
                dropdownModelContainer.model.SetLabel(label);
            }            
            dropdownModelContainer.model.SetOptions(options ?? new List<string>());
            dropdownModelContainer.model.SetValue(value);

            return true;
        }
    }
}