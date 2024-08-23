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
    public class KeyboardPivot : MonoBehaviour, IFollowable
    {
        [SerializeField] Transform target;

        [SerializeField] IFollowable.FollowSpeedComponents speedComponents;
        [SerializeField] IFollowable.RotationComponents rotationComponents;
        IFollowable.FollowTargetComponents targetComponents;
        Vector3 offset = Vector3.zero;
        Quaternion currentArcCenter;

        Task setupTarget;

        float lazyRotationDelay = 1f;
        Coroutine lazyRotation;

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
        public Quaternion CurrentArcCenter
        {
            get => currentArcCenter;
            set => currentArcCenter = value;
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

            Vector3 rotation = target.rotation.eulerAngles;
            if ((this as IFollowable).ShouldLazyRotate(ref rotation, rotationComponents.lazyArcPct, rotationComponents.filter))
            {
                if (lazyRotation == null)
                {
                    lazyRotation = StartCoroutine(LazyRotation());
                }
            }
        }

        IEnumerator LazyRotation()
        {
            yield return new WaitForSeconds(lazyRotationDelay);

            Vector3 rotation = target.rotation.eulerAngles;
            if ((this as IFollowable).ShouldLazyRotate(ref rotation, rotationComponents.lazyArcPct, rotationComponents.filter))
            {
                (this as IFollowable).CurrentArcCenter = Quaternion.Euler(rotation);
                (this as IFollowable).Rotate(Quaternion.Euler(rotation), false, rotationComponents.filter);
            }

            lazyRotation = null;
        }

#if UNITY_EDITOR

        float radius = 1f;
        
        private void OnDrawGizmos()
        {
            Vector3 direction = RotationUtils.RotationToDirection((this as IFollowable).CurrentArcCenter);
            GizmosDrawer.DrawWireArc(
                transform.position, 
                direction, 
                new(1f, 0f, 1f),
                rotationComponents.lazyArcPct, 
                radius
            );
        }

#endif
    }
}