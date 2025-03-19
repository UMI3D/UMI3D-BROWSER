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
using UnityEngine.Pool;

namespace umi3d.browserRuntime.interactions
{
    internal class InputView : MonoBehaviour, IView
    {
        InputIconView firstInputIcon;
        TMPro.TMP_Text textTMP;

        IView view => this;

        ObjectPool<InputIconView> inputIconsPool;
        //List<InputIconView> inputIconsActive = new(); // Uncomment this if you need to get the active icon.

        void Awake()
        {
            view.Set(ref firstInputIcon, 0);
            view.Set(ref textTMP, 1);

            inputIconsPool = new(createFunc: () =>
            {
                if (inputIconsPool.CountActive == 0)
                {
                    return firstInputIcon;
                }
                else
                {
                    return Instantiate(firstInputIcon);
                }
            }, actionOnGet: iconView =>
            {
                iconView.transform.SetParent(transform, false);
                iconView.transform.SetSiblingIndex(transform.childCount - 2);
                iconView.gameObject.SetActive(true);
                //inputIconsActive.Add(iconView); // Uncomment this if you need to get the active icon.
            }, actionOnRelease: iconView =>
            {
                iconView.gameObject.SetActive(false);
                //inputIconsActive.Remove(iconView); // Uncomment this if you need to get the active icon.
            });
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        void OnDestroy()
        {
        }

        void Update()
        {
        }
    }
}