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

namespace umi3d.browserRuntime.ui
{
    public class KeyboardBackgroundAnimation : MonoBehaviour
    {
        [Tooltip("Limit of the difference between two floats.")]
        [SerializeField] float epsilon = .001f;

        Coroutine coroutine;
        RectTransform rectTransform;
        float width;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            width = rectTransform.rect.width;
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
                UnityEngine.Debug.LogError($"[KeyboardBackgroundAnimation] no KeyboardNotificationKeys.Info.IsOpening key.");
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.AnimationTime, out float animationTime))
            {
                UnityEngine.Debug.LogError($"[KeyboardBackgroundAnimation] no KeyboardNotificationKeys.Info.AnimationTime key.");
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.PhaseOneStartTimePercentage, out float phaseOnePct))
            {
                UnityEngine.Debug.LogError($"[KeyboardBackgroundAnimation] no KeyboardNotificationKeys.Info.PhaseOneStartTimePercentage key.");
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.WithAnimation, out bool isAnimated))
            {
                UnityEngine.Debug.LogError($"[KeyboardBackgroundAnimation] no KeyboardNotificationKeys.Info.WithAnimation key.");
                return;
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            if (isAnimated)
            {
                UnityEngine.Debug.Log($"message");
                coroutine = StartCoroutine(
                    isOpening
                    ? Opening(animationTime) 
                    : Closing(animationTime, phaseOnePct)
                );
            }
            else
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, isOpening ? width : 0f);
            }
        }

        IEnumerator Opening(float animationTime)
        {
            this.animationTime = animationTime;
            yield return Animation(
                0f, 
                width
            );
        }

        IEnumerator Closing(float animationTime, float phaseOnePct)
        {

            float elapsedTime = animationTime * completionPercentage;
            while (epsilon < animationTime * phaseOnePct - elapsedTime)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            this.animationTime = animationTime * (1 - phaseOnePct);
            yield return Animation(
                width, 
                0f
            );
        }

        float animationTime = 0f;
        float completionPercentage = 0f;
        IEnumerator Animation(float initial, float final)
        {
            float elapsedTime = animationTime * completionPercentage;

            while (epsilon < animationTime - elapsedTime)
            {
                float t = Easings.EaseInCirc(elapsedTime, animationTime);
                float x = Easings.Lerp(initial, final, t);

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);

                completionPercentage = elapsedTime / animationTime;

                yield return null;
                elapsedTime += Time.deltaTime;
            }

            completionPercentage = 0f;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, final);

            yield return null;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("TestOpenWithAnimation")]
        void TestOpenWithAnimation()
        {
            UnityEngine.Debug.Log($"test open with animation");
            NotificationHub.Default.Notify(
                this,
                "Animation",
                new()
                {
                    { "AnimationPhase", 0 },
                    { "IsStarting", true },
                    { "AnimationTime", 10f },
                    { "IsAnimated", true },
                }
            );
        }

        [ContextMenu("TestCloseWithAnimation")]
        void TestCloseWithAnimation()
        {
            UnityEngine.Debug.Log($"test close with animation");
            NotificationHub.Default.Notify(
                this,
                "Animation",
                new()
                {
                    { "AnimationPhase", 1 },
                    { "IsStarting", false },
                    { "AnimationTime", 10f },
                    { "IsAnimated", true },
                }
            );
        }

        [ContextMenu("TestClose")]
        void TestClose()
        {
            UnityEngine.Debug.Log($"test close");
            NotificationHub.Default.Notify(
                this,
                "Animation",
                new()
                {
                    { "IsStarting", false },
                    { "AnimationTime", 1f },
                    { "IsAnimated", false },
                }
            );
        }
#endif
    }
}