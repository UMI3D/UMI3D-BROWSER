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
using umi3dBrowsers.interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    [AddComponentMenu("UMI3D_UI/Toggle Switch", 30)]
    public class ToggleSwitch : MonoBehaviour, IPointerClickHandler, IUMI3DBrowserUI, IDisplayer
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
        [SerializeField] private Action onTransitionEffet;

        [Header("Elements to recolor")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image handleImage;
        [Space]
        [SerializeField] private bool recolorBackground;
        [SerializeField] private bool recolorHandle;

        [Header("Colors")]
        [SerializeField] private Color backgroundColorOff = Color.white;
        [SerializeField] private Color backgroundColorOn = Color.white;
        [Space]
        [SerializeField] private Color handleColorOff = Color.white;
        [SerializeField] private Color handleColorOn = Color.white;

        private bool _isBackGroundImageNotNull;
        private bool _isHandleImageNotNull;
        private ToggleSwitchGroupManager _toggleSwitchGroupManager;

        private void Awake()
        {
            SetupToggleComponents();

            CheckForNull();
            ChangeColors();
        }

        private void OnValidate()
        {
            SetupToggleComponents();

            _slider.value = sliderValue;

            CheckForNull();
            ChangeColors();
        }

        protected void OnEnable()
        {
            onTransitionEffet += ChangeColors;
        }

        protected void OnDisable()
        {
            onTransitionEffet -= ChangeColors;
        }

        private void CheckForNull()
        {
            _isBackGroundImageNotNull = backgroundImage != null;
            _isHandleImageNotNull = handleImage != null;
        }

        private void ChangeColors()
        {
            if (recolorBackground && _isBackGroundImageNotNull)
                backgroundImage.color = Color.Lerp(backgroundColorOff, backgroundColorOn, sliderValue);
            if (recolorHandle && _isHandleImageNotNull)
                handleImage.color = Color.Lerp(handleColorOff, handleColorOn, sliderValue);
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

                    onTransitionEffet?.Invoke();

                    yield return null;
                }
            }

            _slider.value = endValue;
        }

        public object GetValue(bool trim)
        {
            return CurrentValue;
        }

        public void SetTitle(string title)
        {
            Debug.Log("No title handling implemented for this element [Toggle switch]", this);
        }

        public void SetPlaceHolder(List<string> placeHolder)
        {
            if (placeHolder[0] == "0" && CurrentValue != false)
                Toggle();
            else if (placeHolder[0] == "1" &&  CurrentValue != true)
                Toggle();
                
        }

        public void SetColor(Color color)
        {

        }

        public void SetResource(object resource)
        {

        }
    }
}

