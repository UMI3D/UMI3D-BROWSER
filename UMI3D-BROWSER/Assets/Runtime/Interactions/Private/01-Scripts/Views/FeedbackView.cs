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

        RawImage roundImage;
        RectTransform roundRectTransform;

        RawImage circleImage;
        RectTransform circleRectTransform;

        IView view => this;

        void Awake()
        {
            view.Set(ref roundImage, 0);
            roundRectTransform = roundImage.GetComponent<RectTransform>();

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
            switch (model.distanceState)
            {
                case InteractableDistanceState.Far:
                    HideFeedbacks(true);
                    break;

                case InteractableDistanceState.Middle:
                    DisplayRoundFeedback(true);
                    break;

                case InteractableDistanceState.Close:
                    switch (model.hoveringState)
                    {
                        case InteractableHoveringState.NotHover:
                            DisplayRoundFeedback(true);
                            break;

                        case InteractableHoveringState.Hover:
                            DisplayCircleFeedback(true);
                            break;

                        default:
                            UnityEngine.Debug.Log($"[InteractableUIVC] Error: Unhandled case.");
                            break;
                    }
                    break;

                default:
                    UnityEngine.Debug.Log($"[InteractableUIVC] Error: Unhandled case.");
                    break;
            }
        }

        void DisplayRoundFeedback(bool withAnimation)
        {
            roundImage.enabled = true;
            circleImage.enabled = false;
        }

        void DisplayCircleFeedback(bool withAnimation)
        {
            roundImage.enabled = false;
            circleImage.enabled = true;
        }

        void HideFeedbacks(bool withAnimation)
        {
            roundImage.enabled = false;
            circleImage.enabled = false;
        }

        IEnumerator RoundFeedbackAnimation(bool display)
        {
            yield return null;

            //roundRectTransform.localScale
        }
    }
}