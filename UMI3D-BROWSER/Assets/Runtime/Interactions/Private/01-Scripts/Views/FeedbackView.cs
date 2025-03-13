/*
Copyright 2019 - 2025 Inetum

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

using inetum.unityUtils.ui.canvas;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.interactions
{
    internal class FeedbackView : MonoBehaviour, IView, IInteractableUIDelegate
    {
        public InteractableUIModel model { get; private set; }

        [SerializeField, Tooltip("Animation duration in second.")] float animationDuration = .5f;
        [SerializeField, Tooltip("Whether the feedbacks are animated.")] bool isAnimated = true;

        RawImage roundImage;
        RectTransform roundRectTransform;
        Coroutine roundAnimationCoroutine;
        Color roundImageColor;

        RawImage circleImage;
        RectTransform circleRectTransform;
        Coroutine circleAnimationCoroutine;

        IView view => this;

        void Awake()
        {
            view.Set(ref roundImage, 0);
            roundRectTransform = roundImage.GetComponent<RectTransform>();
            roundImageColor = roundImage.color;

            view.Set(ref circleImage, 1);
            circleRectTransform = circleImage.GetComponent<RectTransform>();
        }

        public void SetModel(InteractableUIModel model)
        {
            this.model = model;
            model.delegates.Add(this);

            UpdateFeedback();
        }

        #region IInteractableUIDelegate

        public void OnChangeOfDistanceState(InteractableDistanceState oldState, InteractableDistanceState newState)
        {
            UpdateFeedback();
        }

        public void OnChangeOfHoveringState(InteractableHoveringState oldState, InteractableHoveringState newState)
        {
            UpdateFeedback();
        }

        #endregion

        void UpdateFeedback()
        {
            switch (model.hoveringState)
            {
                case InteractableHoveringState.NotHover:
                    switch (model.distanceState)
                    {
                        case InteractableDistanceState.Far:
                            // Hide Feedbacks
                            HideFeedback(roundImage, roundRectTransform, ref roundAnimationCoroutine, withAnimation: isAnimated);
                            HideFeedback(circleImage, circleRectTransform, ref circleAnimationCoroutine, withAnimation: isAnimated);
                            break;

                        case InteractableDistanceState.Middle:
                            // Display Round Feedback
                            HideFeedback(circleImage, circleRectTransform, ref circleAnimationCoroutine, withAnimation: isAnimated);
                            DisplayFeedback(roundImage, roundRectTransform, ref roundAnimationCoroutine, withAnimation: isAnimated);
                            roundImage.color = new Color(roundImageColor.r, roundImageColor.g, roundImageColor.b, roundImageColor.a / 2f);
                            break;

                        case InteractableDistanceState.Close:
                            // Display Round Feedback
                            HideFeedback(circleImage, circleRectTransform, ref circleAnimationCoroutine, withAnimation: isAnimated);
                            DisplayFeedback(roundImage, roundRectTransform, ref roundAnimationCoroutine, withAnimation: isAnimated);
                            roundImage.color = roundImageColor;
                            break;

                        default:
                            UnityEngine.Debug.Log($"[InteractableUIVC] Error: Unhandled case.");
                            break;
                    }
                    break;

                case InteractableHoveringState.Hover:
                    // Display Circle Feedback
                    HideFeedback(roundImage, roundRectTransform, ref roundAnimationCoroutine, withAnimation: isAnimated);
                    DisplayFeedback(circleImage, circleRectTransform, ref circleAnimationCoroutine, withAnimation: isAnimated);
                    break;

                default:
                    UnityEngine.Debug.Log($"[InteractableUIVC] Error: Unhandled case.");
                    break;
            }
        }

        void DisplayFeedback(RawImage rawImage, RectTransform imageTransform, ref Coroutine animationCoroutine, bool withAnimation)
        {
            rawImage.enabled = true;
            if (withAnimation)
            {
                if (animationCoroutine != null)
                {
                    StopCoroutine(animationCoroutine);
                }

                animationCoroutine = StartCoroutine(Scale(imageTransform, 1f));
            }
            else
            {
                imageTransform.localScale = Vector3.one;
            }
        }

        void HideFeedback(RawImage rawImage, RectTransform imageTransform, ref Coroutine animationCoroutine, bool withAnimation)
        {
            if (withAnimation)
            {
                if (animationCoroutine != null)
                {
                    StopCoroutine(animationCoroutine);
                }

                animationCoroutine = StartCoroutine(Scale(imageTransform, 0f));
            }
            else
            {
                imageTransform.localScale = Vector3.zero;
                rawImage.enabled = false;
            }
        }

        IEnumerator Scale(RectTransform image, float targetScale)
        {
            UnityEngine.Debug.Log($"start animation");

            // Get the initial scale of the canvas
            float initialScaleX = image.localScale.x;
            float initialScaleY = image.localScale.y;

            // Track the elapsed time
            float elapsedTime = 0f;

            // Animate the scale over time
            while (elapsedTime < animationDuration)
            {
                // Calculate the new scale using Lerp
                float scaleX = Mathf.Lerp(initialScaleX, targetScale, elapsedTime / animationDuration);
                float scaleY = Mathf.Lerp(initialScaleY, targetScale, elapsedTime / animationDuration);
                image.localScale = new Vector3(scaleX, scaleY, 1f);

                // Increment the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }

            // Ensure the final scale is set to the target scale
            image.localScale = new Vector3(targetScale, targetScale, 1f);

            // Continue with the rest of your script here
            Debug.Log("Animation finished!");
        }
    }
}