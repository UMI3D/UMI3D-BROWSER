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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    [RequireComponent(typeof(VignetteDisplayer))]
    public class VignetteButton : Button
    {
        private VignetteDisplayer _vignetteDisplayer;

        protected override void Awake()
        {
            _vignetteDisplayer = GetComponent<VignetteDisplayer>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {   
            _vignetteDisplayer.HoverEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            _vignetteDisplayer.HoverExit(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            _vignetteDisplayer.Click(eventData);
        }
    }
}

