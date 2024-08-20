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
                float currentWidth = rectTransform.rect.width;
                coroutine = StartCoroutine(
                    isStarting 
                    ? Opening(animationTime, currentWidth) 
                    : Closing(animationTime, currentWidth)
                );
            }
            else
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, isStarting ? width : 0f);
            }
        }

        IEnumerator Opening(float animationTime, float currentWidth)
        {
            float elapsedTime = currentWidth * animationTime / width;
            yield return Animation(animationTime, currentWidth, width, elapsedTime);
        }

        IEnumerator Closing(float animationTime, float currentWidth)
        {
            float elapsedTime = (width - currentWidth) * animationTime / width;
            yield return Animation(animationTime, currentWidth, 0f, elapsedTime);
        }

        IEnumerator Animation(float animationTime, float initial, float final, float elapsedTime)
        {
            while (epsilon < animationTime - elapsedTime)
            {
                float t = Easings.EaseInCirc(elapsedTime, animationTime);
                float x = Easings.Lerp(initial, final, t);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, final);
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