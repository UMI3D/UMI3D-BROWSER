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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ToggleSwitch : Selectable, IPointerClickHandler
    {
        [Header("Slider setup")]
        [SerializeField, Range(0, 1f)] private float sliderValue;

        public bool CurrentValue { get; private set; }  

        private Slider _slider;

        [Header("Animation")]
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0,0, 1, 1);

        private Coroutine _animationSliderCoroutine;

        [Header("Events")]
        [SerializeField] private UnityEvent onToggleOn;
        [SerializeField] private UnityEvent onToggleOff;

        private ToggleSwitchGroupManager _toggleSwitchGroupManager;

        private void OnValidate()
        {
            SetupToggleComponents();

            _slider.value = sliderValue; 
        }

        private void SetupToggleComponents()
        {
            if (_slider != null) return;

            SetUpSliderComponents();
        }

        private void SetUpSliderComponents()
        {
            _slider = GetComponent<Slider>();

            if (_slider == null)
            {
                Debug.Log("No Slider Found !", this);
            }

            _slider.interactable = false;
            var sliderColors = _slider.colors;
            sliderColors.disabledColor = Color.white;
            _slider.colors = sliderColors;
            _slider.transition = Selectable.Transition.None;
        }

        public void SetupForManager(ToggleSwitchGroupManager manager)
        {
            _toggleSwitchGroupManager = manager;
        }

        private void Awake()
        {
            SetupToggleComponents();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }

        public void Click()
        {
            Toggle();
        }

        private void Toggle()
        {
            if (_toggleSwitchGroupManager != null)
            {
                _toggleSwitchGroupManager.ToggleGroup(this);
            }
            else
            {
                SetStateAndStartAnimation(!CurrentValue);
            }
        }

        public void ToggleByGroupManager(bool state)
        {
            SetStateAndStartAnimation(state);
        }
        private bool _previousValue;

        private void SetStateAndStartAnimation(bool state)
        {
            _previousValue = CurrentValue;
            CurrentValue = state;

            if (_previousValue != CurrentValue) 
            {
                if (CurrentValue)
                {
                    onToggleOn?.Invoke();
                }
                else
                {
                    onToggleOff?.Invoke();
                }
            }

            if (_animationSliderCoroutine != null)
            {
                StopCoroutine(_animationSliderCoroutine);
            }

            _animationSliderCoroutine = StartCoroutine(AnimateSlider());
        }

        private IEnumerator AnimateSlider()
        {
            float startValue = _slider.value;
            float endValue = CurrentValue ? 1 :0;

            float time = 0;
            if (animationDuration > 0)
            {
                while(time < animationDuration)
                {
                    time += Time.deltaTime;

                    float lerpFactor = slideEase.Evaluate(time/animationDuration);
                    _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                    yield return null;
                }
            }

            _slider.value = endValue;
        }
    }
}

