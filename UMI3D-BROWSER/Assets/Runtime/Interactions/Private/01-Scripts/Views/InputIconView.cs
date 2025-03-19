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

using inetum.unityUtils.ui.canvas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.interactions
{
    internal class InputIconView : MonoBehaviour, IView
    {

        [SerializeField] Sprite mouseLeftClick;
        [SerializeField] Sprite mouseRightClick;

        Image icon;
        TMPro.TMP_Text textTMP;

        IView view => this;

        void Awake()
        {
            view.Set(ref icon, 0);
            icon.enabled = false;

            view.Set(ref textTMP, 1);
            textTMP.enabled = false;
        }

        public void SetIcon()
        {
            textTMP.enabled = false;

            // TODO: Set sprite between left and right click.
            icon.sprite = mouseLeftClick;
            icon.enabled = true;
        }

        public void SetText(string text)
        {
            icon.enabled = false;

            this.textTMP.text = text;
            this.textTMP.enabled = true;

            // TODO: update this.size according to the size of the text.
        }
    }
}