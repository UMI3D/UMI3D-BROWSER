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
    public class ButtonDisplayer : MonoBehaviour, ISubDisplayer
    {
        [Header("trash button")]
        [SerializeField] private Color backgroundNormalColor = Color.gray;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color clickColor = Color.white;
        [Space]
        [SerializeField] private Button button;
        [SerializeField] private Image backGround = null;
        [SerializeField] private Image iconImage = null;
        [SerializeField] private Sprite normalIcon;
        [SerializeField] private Sprite hoverIcon;
        [SerializeField] private Sprite clickIcon;

        [Header("Animation")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Config")]
        [SerializeField] private bool stayClicked;
        [SerializeField] private bool canUnclick;

        public Color NormalColor { get => normalColor; set => normalColor = value; }
        public Color HoverColor { get => hoverColor; set => hoverColor = value; }

        public Sprite NormalIcon { get => normalIcon; set => normalIcon = value; }
        public Sprite HoverIcon { get => hoverIcon; set => hoverIcon = value; }
        public Sprite ClickIcon { get => clickIcon; set => clickIcon = value; }

        private void Awake()
        {
            backGround.color = backgroundNormalColor;
            iconImage.color = normalColor;
        }

        private void OnEnable()
        {
            button.enabled = true;

            if (backGround != null)
                backGround.color = backgroundNormalColor;
            if (iconImage != null)
            {
                if (stayClicked && _isClicked)
                    iconImage.color = clickColor;
                else
                    iconImage.color = normalColor;
            }

            if (_easeInOutCoroutine != null)
            {
                StopCoroutine(_easeInOutCoroutine);
                _easeInOutCoroutine = null;
            }

            _easeInOutCoroutine = StartCoroutine(EaseInOut(true));
        }

        private void OnDisable()
        {          
            //if (_easeInOutCoroutine != null)
            //{
            //    StopCoroutine(_easeInOutCoroutine);
            //    _easeInOutCoroutine = null;
            //}
        }

        public event Action OnClick;
        public event Action OnHover;
        public event Action OnDisabled;

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

                    canvasGroup.alpha = currentValue;

                    yield return null;
                }
            }

            if (!isEnabeling) OnDisabled?.Invoke();
        }

        public void Disable()
        {
            button.enabled = false;

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

        public void Init(Color normalColor, Color hoverColor, Color selectedColor)
        {
            throw new NotImplementedException();
        }
    }
}

