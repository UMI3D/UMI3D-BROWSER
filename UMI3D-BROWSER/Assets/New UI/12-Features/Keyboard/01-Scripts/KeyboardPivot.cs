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
        [SerializeField] IFollowable.TranslationComponents translationComponents;
        [SerializeField] IFollowable.RotationComponents rotationComponents;

        Task setupTarget;

        float lazyDelay = 1f;

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
        public Vector3 CurrentGuardianCenter
        {
            get => translationComponents.currentGuardianCenter;
            set => translationComponents.currentGuardianCenter = value;
        }
        public Quaternion CurrentArcCenter
        {
            get => rotationComponents.CurrentArcCenter;
            set => rotationComponents.CurrentArcCenter = value;
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

                    (this as IFollowable).Rest(target);

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

            Vector3 position = target.position;
            if ((this as IFollowable).ShouldLazyTranslate(ref position, translationComponents.lazyGuardianRadius))
            {
                if (translationComponents.lazyCoroutine == null)
                {
                    translationComponents.lazyCoroutine = StartCoroutine(LazyTranslation());
                }
            }

            Vector3 rotation = target.rotation.eulerAngles;
            if ((this as IFollowable).ShouldLazyRotate(ref rotation, rotationComponents.lazyArcPct, rotationComponents.filter))
            {
                if (rotationComponents.lazyCoroutine == null)
                {
                    rotationComponents.lazyCoroutine = StartCoroutine(LazyRotation());
                }
            }
        }

        IEnumerator LazyTranslation()
        {
            yield return new WaitForSeconds(lazyDelay);

            Vector3 position = target.position;
            if ((this as IFollowable).ShouldLazyTranslate(ref position, translationComponents.lazyGuardianRadius))
            {
                (this as IFollowable).Translate(position, false);
                (this as IFollowable).CurrentGuardianCenter = target.position;
            }

            translationComponents.lazyCoroutine = null;
        }

        IEnumerator LazyRotation()
        {
            yield return new WaitForSeconds(lazyDelay);

            Vector3 rotation = target.rotation.eulerAngles;
            if ((this as IFollowable).ShouldLazyRotate(ref rotation, rotationComponents.lazyArcPct, rotationComponents.filter))
            {
                (this as IFollowable).Rotate(Quaternion.Euler(rotation), false, rotationComponents.filter);
                (this as IFollowable).CurrentArcCenter = Quaternion.Euler(rotation);
            }

            rotationComponents.lazyCoroutine = null;
        }

#if UNITY_EDITOR

        [SerializeReference] bool debug = true;
        float radius = 1f;        
        
        void OnDrawGizmos()
        {
            if (!debug)
            {
                return;
            }

            GizmosDrawer.DrawWireSphere((this as IFollowable).CurrentGuardianCenter, translationComponents.lazyGuardianRadius, Color.cyan);

            Vector3 direction = RotationUtils.RotationToDirection((this as IFollowable).CurrentArcCenter);
            GizmosDrawer.DrawWireArc(
                target != null ? target.position : transform.position, 
                direction, 
                new(1f, 0f, 1f),
                rotationComponents.lazyArcPct, 
                radius
            );
        }

#endif
    }
}