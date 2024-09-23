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

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPreviewBarAnimation : MonoBehaviour
    {
        /// <summary>
        /// The animation of the preview bar.
        /// </summary>
        inetum.unityUtils.Animation backgroundAnimation = new();
        /// <summary>
        /// The animation of the preview bar.
        /// </summary>
        inetum.unityUtils.Animation textAreaAnimation = new();
        Coroutine coroutine;
        /// <summary>
        /// The initial width of the preview bar (when it is open).
        /// </summary>
        float width;

        TMPro.TMP_InputField inputField;

        void Awake()
        {
            inputField = GetComponentInChildren<TMPro.TMP_InputField>();

            RectTransform BgRT = inputField.targetGraphic.rectTransform;
            RectTransform tART = inputField.textViewport;

            // Get the width of the preview bar.
            // The preview bar has to be open.
            width = BgRT.rect.width;

            backgroundAnimation
                .SetApplyValue<float>(x => BgRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x))
                .SetEasing(Easings.EaseInCirc)
                .SetLerp<float>(Easings.Lerp);
            textAreaAnimation
               .SetApplyValue<float>(x => tART.localScale = new(x, x, x))
               .SetEasing(Easings.EaseInCirc)
               .SetLerp<float>(Easings.Lerp);
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
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.AnimationTime, out float animationTime))
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.PhaseOneStartTimePercentage, out float phaseOnePct))
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.WithAnimation, out bool isAnimated))
            {
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
                    ? Opening(animationTime)
                    : Closing(animationTime, phaseOnePct)
                );
            }
            else
            {
                backgroundAnimation.ApplyValue(isOpening ? width : 0f);
                textAreaAnimation.ApplyValue(isOpening ? 1f : 0f);
            }
            inputField.interactable = isOpening;
        }

        IEnumerator Opening(float animationTime)
        {
            var bg = backgroundAnimation
               .SetInitAndFinalValue(0f, width)
               .SetAnimationTime(animationTime)
               .Start();

            var textArea = textAreaAnimation
                 .SetInitAndFinalValue(0f, 1f)
                 .SetAnimationTime(animationTime)
                 .Start();

            bool shouldYield = bg.MoveNext();
            if (shouldYield)
            {
                textArea.MoveNext();
            }
            else
            {
                shouldYield = textArea.MoveNext();
            }

            while (shouldYield)
            {
                yield return null;

                shouldYield = bg.MoveNext();
                if (shouldYield)
                {
                    textArea.MoveNext();
                }
                else
                {
                    shouldYield = textArea.MoveNext();
                }
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        IEnumerator Closing(float animationTime, float phaseOnePct)
        {
            // Wait for the key animation to be completed.
            float elapsedTime = animationTime * backgroundAnimation.completionPercentage;
            while (backgroundAnimation.epsilon < animationTime * phaseOnePct - elapsedTime)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            var bg = backgroundAnimation
                .SetInitAndFinalValue(width, 0f)
                .SetAnimationTime(animationTime * (1 - phaseOnePct))
                .Start();

            var textArea = textAreaAnimation
                .SetInitAndFinalValue(1f, 0f)
                .SetAnimationTime(animationTime * (1 - phaseOnePct))
                .Start();

            bool shouldYield = bg.MoveNext();
            if (shouldYield)
            {
                textArea.MoveNext();
            }
            else
            {
                shouldYield = textArea.MoveNext();
            }
            while (shouldYield)
            {
                yield return null;
                shouldYield = bg.MoveNext();
                if (shouldYield)
                {
                    textArea.MoveNext();
                }
                else
                {
                    shouldYield = textArea.MoveNext();
                }
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}