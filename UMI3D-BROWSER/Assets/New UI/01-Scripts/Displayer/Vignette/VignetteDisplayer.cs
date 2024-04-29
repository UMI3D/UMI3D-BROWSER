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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class VignetteDisplayer : MonoBehaviour, ISubDisplayer
    {
        [SerializeField] private Color transprentColor = Color.gray;
        [Header("Vignette main Image")]
        [SerializeField] private Image vignetteImage;

        [Header("buttons")]
        [SerializeField] ButtonDisplayer likeButton;
        [SerializeField] ButtonDisplayer trashButton;
        [Space]
        [SerializeField] VignetteInputField inputFieldBackground;


        [Header("Input field backgroung")]
        [SerializeField] private Image IF_background;
        [SerializeField] private Image pen;

        [Header("Animation")]
        [SerializeField] private float hoverExitDelay;

        Coroutine cadeInOutCoroutine;

        [SerializeField] private UnityEvent onVignetteClicked;
        
        enum VignetteState { notHovering, Hovering, HoveringSubElement}
        VignetteState vignetteState;

        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;

        private void Awake()
        {
            transprentColor.a = 0;
            DisableSubComponents();

            likeButton.OnHover += () => vignetteState = VignetteState.HoveringSubElement;
            trashButton.OnHover += () => vignetteState = VignetteState.HoveringSubElement;
            inputFieldBackground.OnHover += () => vignetteState = VignetteState.HoveringSubElement;

            likeButton.OnDisabled += () => DisableSubComponents();
            trashButton.OnDisabled += () => DisableSubComponents();
            inputFieldBackground.OnDisabled += () => DisableSubComponents();
        }

        public void SetupDisplay(string pName, Image pImage = null)
        {
            inputFieldBackground.Text = pName;
            if (pImage != null)
                vignetteImage = pImage;
        }

        public void SetupFavoriteButton(Action onFavorite)
        {
            likeButton.OnClick += onFavorite;
        }

        public void SetupRemoveButton(Action onRemove)
        {
            trashButton.OnClick += onRemove;
        }

        public void SetupRenameButton(Action<string> onRename)
        {
            inputFieldBackground.InputField.OnTextChanged += onRename;
        }

        public void HoverEnter(PointerEventData eventData)
        {
            DisableSubComponents();

            vignetteState = VignetteState.Hovering;

            likeButton.gameObject.SetActive(true);
            trashButton.gameObject.SetActive(true);
            pen.gameObject.SetActive(true);
            IF_background.enabled = true;

        }

        public void HoverExit(PointerEventData eventData)
        {
            vignetteState = VignetteState.notHovering;
            if (gameObject.activeInHierarchy)
                StartCoroutine(HoverDelay());
        }

        public void Click()
        {
            onVignetteClicked?.Invoke();
            OnClick?.Invoke();
            likeButton.Disable();
            trashButton.Disable();
            inputFieldBackground.Disable();
        }

        private void DisableSubComponents()
        {
            likeButton.gameObject.SetActive(false);
            trashButton.gameObject.SetActive(false);
            pen.gameObject.SetActive(false);
            IF_background.enabled = false;
        }

        private IEnumerator HoverDelay()
        {
            yield return new WaitForSeconds(hoverExitDelay);

            if (vignetteState == VignetteState.notHovering)
            {
                likeButton.Disable();
                trashButton.Disable();
            }
        }

        public void Disable()
        {
            
        }

        public void Init(Color normalColor, Color hoverColor, Color selectedColor)
        {
            throw new NotImplementedException();
        }
    }
}
