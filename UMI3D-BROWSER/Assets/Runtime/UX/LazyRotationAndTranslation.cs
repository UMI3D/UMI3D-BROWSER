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

namespace umi3d.browserRuntime.UX
{
    public class LazyRotationAndTranslation : MonoBehaviour, IFollowable
    {
        [Tooltip("The object that will be tracked.")]
        public Transform target;

        [Tooltip("The safe zone when the target translate.")]
        [SerializeField] Guardian translationGuardian;
        [Tooltip("The safe zone when the target rotate.")]
        [SerializeField] Guardian RotationGuardian;
        [Tooltip("The delay before translate or rotate.")]
        [SerializeField] float lazyDelay = 1f;

        [Tooltip("The rotation filter.")]
        [SerializeField] Vector3 rotationFilter = Vector3.one;

        Coroutine lazyCoroutine;

        void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (translationGuardian.DoesNotContain(target) || RotationGuardian.DoesNotContain(target))
            {
                if (lazyCoroutine == null)
                {
                    lazyCoroutine = StartCoroutine(LazyTranslationOrRotation());
                }
            }
        }

        IEnumerator LazyTranslationOrRotation()
        {
            yield return new WaitForSeconds(lazyDelay);

            if (target == null)
            {
                yield break;
            }

            if (translationGuardian.DoesNotContain(target) || RotationGuardian.DoesNotContain(target))
            {
                Rest();
            }

            lazyCoroutine = null;
        }

        /// <summary>
        /// Reset the position and rotation of the guardians to match the current position and rotation.
        /// </summary>
        public void Rest()
        {
            (this as IFollowable).TranslateToward(target.position, 0f);
            (this as IFollowable).RotateToward(target.rotation, 0f, rotationFilter);
        }
    }
}