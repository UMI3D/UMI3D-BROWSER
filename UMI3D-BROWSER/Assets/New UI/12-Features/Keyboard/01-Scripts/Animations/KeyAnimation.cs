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
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class KeyAnimation : MonoBehaviour
    {
        Coroutine coroutine;
        RectTransform rectTransform;
        Vector3 scale;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            scale = rectTransform.localScale;
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                "Animation",
                null,
                Animate
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, "Animation");
        }

        void Animate(Notification notification)
        {
            if (!notification.TryGetInfoT("IsStarting", out bool isStarting))
            {
                UnityEngine.Debug.LogError($"[KeyboardBackgroundAnimation] no IsStarting key.");
                return;
            }

            if (!notification.TryGetInfoT("AnimationTime", out float animationTime))
            {
                UnityEngine.Debug.LogError($"[KeyboardBackgroundAnimation] no AnimationTime key.");
                return;
            }

            if (!notification.TryGetInfoT("IsAnimated", out bool isAnimated))
            {
                UnityEngine.Debug.LogError($"[KeyboardBackgroundAnimation] no IsAnimated key.");
                return;
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            if (isAnimated)
            {
                Vector3 currentScale = rectTransform.localScale;
                coroutine = StartCoroutine(
                    isStarting
                    ? Opening(animationTime, currentScale)
                    : Closing(animationTime, currentScale)
                );
            }
            else
            {
                rectTransform.localScale = new(
                    isStarting ? 1f : 0f,
                    isStarting ? 1f : 0f,
                    isStarting ? 1f : 0f
                );
            }
        }

        IEnumerator Opening(float animationTime, Vector3 currentScale)
        {
            float elapsedTime = Average(currentScale) * animationTime / Average(scale);
            yield return Animation(
                animationTime, 
                currentScale, 
                scale, 
                elapsedTime,
                (a, b) => Easings.EaseOutBack(a, b)
            );
        }

        IEnumerator Closing(float animationTime, Vector3 currentScale)
        {
            float elapsedTime = Average(scale - currentScale) * animationTime / Average(scale);
            yield return Animation(
                animationTime, 
                currentScale, 
                Vector3.zero, 
                elapsedTime,
                Easings.EaseInCirc
            );
        }

        IEnumerator Animation(float animationTime, Vector3 initial, Vector3 final, float elapsedTime, Func<float, float, float> easingMethod)
        {
            while (elapsedTime < animationTime)
            {
                float t = easingMethod(elapsedTime, animationTime);
                Vector3 x = Easings.Lerp(initial, final, t);
                UnityEngine.Debug.Log($"t = {t} && x = {x}");
                rectTransform.localScale = x;
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            rectTransform.localScale = final;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        float Average(Vector3 a)
        {
            return (a.x + a.y + a.z) / 3f;
        }
    }
}