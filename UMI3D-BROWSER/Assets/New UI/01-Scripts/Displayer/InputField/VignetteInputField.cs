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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace umi3dBrowsers.displayer
{
    public class VignetteInputField : MonoBehaviour, ISubDisplayer
    {
        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float _animationValue;
        [SerializeField] private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private Coroutine _easeInOutCoroutine;

        [Header("obj")]
        [SerializeField] private TMP_UMI3DUIInputField inputField;

        public string Text { get => inputField.text; set => inputField.text = value; }
        public TMP_UMI3DUIInputField InputField => inputField;

        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;

        private void Awake()
        {
            inputField.onSelect.AddListener((a) => Click());
            inputField.onDeselect.AddListener(InputFieldDeselected);
            inputField.SetCallBacks(
                (a) => HoverEnter(a),
                (a) => HoverExit(a)
            );
        }

        private void OnEnable()
        {
            if (_easeInOutCoroutine != null)
            {
                StopCoroutine(_easeInOutCoroutine);
                _easeInOutCoroutine = null;
            }

            _easeInOutCoroutine = StartCoroutine(EaseInOut(true));
        }

        private void OnDisable()
        {
            if (_easeInOutCoroutine != null)
            {
                StopCoroutine(_easeInOutCoroutine);
                _easeInOutCoroutine = null;
            }
        }

        public void Click()
        {
            OnClick?.Invoke();
            //backGroundCanvasGroup.alpha = 0;
        }

        public void Disable()
        {
            if (_easeInOutCoroutine != null)
            {
                StopCoroutine(_easeInOutCoroutine);
                _easeInOutCoroutine = null;
            }

            if (gameObject.activeSelf)
            {
                _easeInOutCoroutine = StartCoroutine(EaseInOut(false));
            }
        }

        public void HoverEnter(PointerEventData eventData)
        {
            OnHover?.Invoke();
        }

        public void HoverExit(PointerEventData eventData)
        {
            
        }

        private void InputFieldDeselected(string arg0)
        {
            //backGroundCanvasGroup.alpha = 1f;
        }

        private IEnumerator EaseInOut(bool isEnabeling)
        {
            float startValue = _animationValue;
            float endValue = isEnabeling ? 1 : 0;

            float time = 0;
            if (animationDuration > 0)
            {
                while (time < animationDuration)
                {
                    time += Time.deltaTime;

                    float lerpFactor = _animationCurve.Evaluate(time / animationDuration);
                    float currentValue = _animationValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                    //backGroundCanvasGroup.alpha = currentValue;

                    yield return null;
                }
            }

            if (!isEnabeling) OnDisabled?.Invoke();
        }

        public void Init(Color normalColor, Color hoverColor, Color selectedColor)
        {

        }
    }
}

