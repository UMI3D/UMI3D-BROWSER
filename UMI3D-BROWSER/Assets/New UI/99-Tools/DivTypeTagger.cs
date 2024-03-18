/*
Copyright 2019 - 2023 Inetum

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace form_generator
{
    public class DivTypeTagger : MonoBehaviour
    {
        [Header("DivInfo")]
        [SerializeField] private string name;
        [SerializeField, TextArea] private string toolTip;
        [SerializeField, TextArea] private string description;
        [SerializeField] private DivType divType;
        public DivType DivType => divType;
        public string Name => name;
        public string ToolTip => toolTip;
        public string Description => description;

        /// <summary>
        /// For tool execution only
        /// </summary>
        [SerializeField] private List<DivTypeTagger> children = new();
        public List<DivTypeTagger> Children => children;
    }

    public enum DivType
    {
        Form, Page, Group, Button, Enum, Image, Range, Input_Text, Label
    }
}

