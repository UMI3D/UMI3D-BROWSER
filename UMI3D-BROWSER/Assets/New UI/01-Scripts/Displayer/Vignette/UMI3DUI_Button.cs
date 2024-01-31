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
        private IUMI3DButtonHandler _vignetteDisplayer;

        protected override void Awake()
        {
            _vignetteDisplayer = GetComponent<IUMI3DButtonHandler>();
            onClick.AddListener(() => _vignetteDisplayer.Click());
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {   
            _vignetteDisplayer.HoverEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            _vignetteDisplayer.HoverExit(eventData);
        }
    }
}

