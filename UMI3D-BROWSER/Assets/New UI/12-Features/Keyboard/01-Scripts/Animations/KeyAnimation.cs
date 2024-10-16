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

using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyAnimation : MonoBehaviour
    {
        /// <summary>
        /// The animation of the key.
        /// </summary>
        new inetum.unityUtils.Animation animation = new();
        Coroutine coroutine;
        /// <summary>
        /// The initial scale of the key (when it is open).
        /// </summary>
        Vector3 scale;

        void Awake()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            // Get the scale of the key.
            // The keyboard has to be open.
            scale = rectTransform.localScale;

            animation
                .SetApplyValue<Vector3>(x => rectTransform.localScale = x)
                .SetLerp<Vector3>(Easings.Lerp);
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.OpenOrClose,
                null,
                Animate
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.OpenOrClose);
        }

        void Animate(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.IsOpening, out bool isOpening))
            {
                UnityEngine.Debug.LogError($"[KeyAnimation] no KeyboardNotificationKeys.Info.IsOpening key.");
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.AnimationTime, out float animationTime))
            {
                UnityEngine.Debug.LogError($"[KeyAnimation] no KeyboardNotificationKeys.Info.AnimationTime key.");
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.PhaseOneStartTimePercentage, out float phaseOnePct))
            {
                UnityEngine.Debug.LogError($"[KeyAnimation] no KeyboardNotificationKeys.Info.PhaseOneStartTimePercentage key.");
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.WithAnimation, out bool isAnimated))
            {
                UnityEngine.Debug.LogError($"[KeyAnimation] no KeyboardNotificationKeys.Info.WithAnimation key.");
                return;
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            if (isAnimated)
            {
                coroutine = StartCoroutine(
                    isOpening
                    ? Opening(animationTime, phaseOnePct)
                    : Closing(animationTime, phaseOnePct)
                );
            }
            else
            {
                animation.ApplyValue(
                    isOpening 
                    ? Vector3.one
                    : Vector3.zero
                );
            }
        }

        IEnumerator Opening(float animationTime, float phaseOnePct)
        {
            // Wait for the background animation to be half completed.
            float elapsedTime = animationTime * animation.completionPercentage;
            while (animation.epsilon < animationTime * phaseOnePct - elapsedTime)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            yield return animation
                .SetInitAndFinalValue(Vector3.zero, scale)
                .SetAnimationTime(animationTime * (1 - phaseOnePct))
                .SetEasing((e, a) => Easings.EaseOutBack(e, a))
                .Start();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        IEnumerator Closing(float animationTime, float phaseOnePct)
        {
            yield return animation
                .SetInitAndFinalValue(scale, Vector3.zero)
                .SetAnimationTime(animationTime * (1 - phaseOnePct))
                .SetEasing(Easings.EaseInCirc)
                .Start();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}