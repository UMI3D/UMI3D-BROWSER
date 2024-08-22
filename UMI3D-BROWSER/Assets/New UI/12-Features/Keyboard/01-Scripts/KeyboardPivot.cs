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

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPivot : MonoBehaviour, IFollowable
    {
        [SerializeField] Transform target;
        [SerializeField] IFollowable.FollowSpeedComponents speedComponents;
        [SerializeField] IFollowable.FollowRotationFilterComponents filterComponents;
        IFollowable.FollowTargetComponents targetComponents;
        Vector3 offset = Vector3.zero;

        Task setupTarget;

        public float SmoothTranslationSpeed
        {
            get => speedComponents.SmoothTranslationSpeed;
            set => speedComponents.SmoothTranslationSpeed = value;
        }
        public float SmoothRotationSpeed 
        { 
            get => speedComponents.SmoothRotationSpeed; 
            set => speedComponents.SmoothRotationSpeed = value; 
        }
        public Vector3 Offset
        {
            get => offset;
            set => offset = value;
        }
        public Vector3 TranslationTarget
        {
            get => targetComponents.TranslationTarget;
            set => targetComponents.TranslationTarget = value;
        }
        public Quaternion RotationTarget 
        { 
            get => Quaternion.Euler(targetComponents.RotationTarget); 
            set => targetComponents.RotationTarget = value.eulerAngles; 
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

                    setupTarget = null;
                });

                setupTarget.Start(TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            (this as IFollowable).Translate(target.position);
            (this as IFollowable).Rotate(target.rotation, filterComponents.Filter, filterComponents.Sequences);
        }
    }
}