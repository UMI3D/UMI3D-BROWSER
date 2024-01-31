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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ButtonDisplayer : MonoBehaviour, IUMI3DButtonHandler
    {
        [Header("trash button")]
        [SerializeField] private Color transprentColor = Color.gray;
        [SerializeField] private Color backgroundNormalColor = Color.gray;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color clickColor = Color.white;
        [Space]
        [SerializeField] private Button button;
        [SerializeField] private Collider button_collider;
        [SerializeField] private Image backGround = null;
        [SerializeField] private Image iconImage = null;
        [SerializeField] private Sprite normalIcon;
        [SerializeField] private Sprite hoverIcon;
        [SerializeField] private Sprite clickIcon;

        [Header("Animation")]
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Config")]
        [SerializeField] private bool stayClicked;
        [SerializeField] private bool canUnclick;



        private void OnEnable()
        {
            button.enabled = true;
            button_collider.enabled = true;

            if (_easeInOutCoroutine != null )
            {
                StopCoroutine(_easeInOutCoroutine );
                _easeInOutCoroutine = null;
            }

            _easeInOutCoroutine = StartCoroutine(EaseInOut(true));
        }

        private void OnDisable()
        {
            button.enabled = false;
            button_collider.enabled = false;

            if (_easeInOutCoroutine != null)
            {
                StopCoroutine(_easeInOutCoroutine);
                _easeInOutCoroutine = null;
            }

            _easeInOutCoroutine = StartCoroutine(EaseInOut(false));
        }

        public event Action OnClick;
        public event Action OnHover;

        private bool _isClicked;

        private float _animationValue;
        private Coroutine _easeInOutCoroutine;


        public void Click()
        {
            OnClick?.Invoke();

            if (canUnclick && _isClicked)
            {
                iconImage.sprite = hoverIcon;
                iconImage.color = hoverColor;
                _isClicked = false;
            }
            else
            {
                iconImage.sprite = clickIcon;
                iconImage.color = clickColor;
                _isClicked = true;
            }
        }

        public void HoverEnter(PointerEventData eventData)
        {
            OnHover?.Invoke();

            if (stayClicked && _isClicked) return;
            
            iconImage.sprite = hoverIcon;
            iconImage.color = hoverColor;
        }

        public void HoverExit(PointerEventData eventData)
        {
            if (stayClicked && _isClicked) return;

            iconImage.sprite = normalIcon;
            iconImage.color = normalColor;
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

                    float lerpFactor = slideEase.Evaluate(time / animationDuration);
                    float currentValue = _animationValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                    if (backGround != null)
                        backGround.color = Color.Lerp(transprentColor, backgroundNormalColor, currentValue);
                    if (iconImage != null)
                    {
                        if (stayClicked && _isClicked) 
                             iconImage.color = Color.Lerp(transprentColor, clickColor, currentValue);
                        else
                            iconImage.color = Color.Lerp(transprentColor, normalColor, currentValue);
                    }

                    yield return null;
                }
            }
        }
    }
}

