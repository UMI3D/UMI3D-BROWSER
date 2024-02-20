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

using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class UMI3DUI_Button : Button
    {
        private ISubDisplayer _subDisplayer;
        public ISubDisplayer SubDisplayer => _subDisplayer;
        public event Action OnHoverEnter;
        public event Action OnHoverExit;

        private int id;
        public int ID => id;    
        public void SetID(int id) { this.id = id; }

        protected override void Awake()
        {
            _subDisplayer = GetComponent<ISubDisplayer>();
            if (_subDisplayer == null) return;
            onClick.AddListener(_subDisplayer.Click);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            OnHoverEnter?.Invoke();
            if (_subDisplayer == null) return;
            _subDisplayer.HoverEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            OnHoverExit?.Invoke();
            if (_subDisplayer == null) return;
            _subDisplayer.HoverExit(eventData);
        }
    }
}

