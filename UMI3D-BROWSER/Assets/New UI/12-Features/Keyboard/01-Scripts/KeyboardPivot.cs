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
using System.Threading.Tasks;
using umi3d.browserRuntime.UX;
using UnityEngine;
using inetum.unityUtils.math;
using inetum.unityUtils.debug;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPivot : MonoBehaviour
    {
        [SerializeField] Transform target;
        Task setupTarget;

        LazyRotationAndTranslation lazyRotationAndTranslation;

        void Awake()
        {
            lazyRotationAndTranslation = GetComponent<LazyRotationAndTranslation>();
        }

        void OnEnable()
        {
            if (target == null && setupTarget == null)
            {
                setupTarget = new Task(async () =>
                {
                    while (Camera.main == null)
                    {
                        await Task.Yield();
                    }
                    target = Camera.main.transform;

                    lazyRotationAndTranslation.target = target;
                    lazyRotationAndTranslation.Rest();

                    setupTarget = null;
                });

                setupTarget.Start(TaskScheduler.FromCurrentSynchronizationContext());
            }
        }


    }
}