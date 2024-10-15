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
using TMPro;
using umi3dBrowsers.Displayer;
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
        [SerializeField] private Color normalImageColor;
        [SerializeField] private Color hoverImageColor;

        [Header("buttons")]
        [SerializeField] private ButtonSubDisplayer likeButton;
        [SerializeField] private Image likeImage;
        [SerializeField] private Sprite normalLikeIcon;
        [SerializeField] private Color normalLikeColor;
        [SerializeField] private Sprite normaHoverLikeIcon;
        [SerializeField] private Color normalHoverLikeColor;
        [SerializeField] private Sprite selectedLikeIcon;
        [SerializeField] private Color selectedLikeColor;
        [SerializeField] private Sprite selectedHoverLikeIcon;
        [SerializeField] private Color selectedHoverLikeColor;
        [Space]
        [SerializeField] private ButtonSubDisplayer trashButton;
        [Space]
        [SerializeField] VignetteInputField inputFieldBackground;


        [Header("Input field backgroung")]
        [SerializeField] private Image IF_background;
        [SerializeField] private Image pen;

        [Header("Animation")]
        [SerializeField] private float hoverExitDelay;

        Coroutine cadeInOutCoroutine;

        [SerializeField] private UnityEvent onVignetteClicked;

        enum VignetteState { notHovering, Hovering, HoveringSubElement }
        VignetteState vignetteState;

        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;

        private TMP_Text inputFieldText;

        private bool m_usesFavoriteButton = true;
        private bool m_usesDeleteButton = true;
        public TMP_Text InputFieldText {
            get {
                if (inputFieldText == null)
                    inputFieldText = inputFieldBackground.GetComponentInChildren<TMP_Text>();
                return inputFieldText;
            }
        }

        private void Awake()
        {
            transprentColor.a = 0;
            DisableSubComponents();
            pen.gameObject.SetActive(false);
            IF_background.enabled = false;

            if (m_usesFavoriteButton)
                likeButton.OnHover += () => vignetteState = VignetteState.HoveringSubElement;
            if (m_usesDeleteButton)
                trashButton.OnHover += () => vignetteState = VignetteState.HoveringSubElement;
            inputFieldBackground.OnHover += () => {
                vignetteState = VignetteState.HoveringSubElement;

                pen.gameObject.SetActive(true);
                IF_background.enabled = true;
            };
            inputFieldBackground.OnHoverExit += () => {
                pen.gameObject.SetActive(false);
                IF_background.enabled = false;
            };

            if (m_usesFavoriteButton)
                likeButton.OnDisabled += () => DisableSubComponents();
            if (m_usesDeleteButton)
                trashButton.OnDisabled += () => DisableSubComponents();
            inputFieldBackground.OnDisabled += () => DisableSubComponents();
        }

        private void OnDestroy()
        {
            likeButton.OnDisabled -= () => DisableSubComponents();
            trashButton.OnDisabled -= () => DisableSubComponents();
            inputFieldBackground.OnDisabled -= () => DisableSubComponents();
        }

        public void SetupDisplay(string pName, Image pImage = null)
        {
            inputFieldBackground.Text = pName;
            if (pImage != null)
                vignetteImage = pImage;

            vignetteImage.color = normalImageColor;
            InputFieldText.color = normalImageColor;
        }

        internal void SetSprite(Sprite sprite)
        {
            vignetteImage.sprite = sprite;
        }

        public void SetupFavoriteButton(Action onFavorite, bool isFavorite = false)
        {
            if (!m_usesFavoriteButton) return;

            likeButton.OnClick += onFavorite;

            likeImage.sprite = isFavorite ? selectedLikeIcon : normalLikeIcon;
            likeImage.color = isFavorite ? selectedLikeColor : normalLikeColor;

            likeButton.NormalColor = isFavorite ? selectedLikeColor : normalLikeColor;
            likeButton.HoverColor = isFavorite ? selectedHoverLikeColor : normalHoverLikeColor;
            likeButton.NormalIcon = isFavorite ? selectedLikeIcon : normalLikeIcon;
            likeButton.HoverIcon = isFavorite ? selectedHoverLikeIcon : normaHoverLikeIcon;
        }

        public void SetupRemoveButton(Action onRemove)
        {
            if (m_usesDeleteButton)
                trashButton.OnClick += onRemove;
        }

        public void SetupRenameButton(Action<string> onRename)
        {
            inputFieldBackground.InputField.onValueChanged.AddListener(value => onRename(value));
        }

        public void HoverEnter(PointerEventData eventData)
        {
            DisableSubComponents();

            vignetteState = VignetteState.Hovering;

            vignetteImage.color = hoverImageColor;
            InputFieldText.color = hoverImageColor;

            if (m_usesFavoriteButton)
                likeButton.gameObject.SetActive(true);
            if (m_usesDeleteButton)
                trashButton.gameObject.SetActive(true);
        }

        public void HoverExit(PointerEventData eventData)
        {
            vignetteState = VignetteState.notHovering;
            if (gameObject.activeInHierarchy)
                StartCoroutine(HoverDelay());

            vignetteImage.color = normalImageColor;
            InputFieldText.color = normalImageColor;
        }

        public void Click()
        {
            onVignetteClicked?.Invoke();
            OnClick?.Invoke();
            if (m_usesFavoriteButton)
                likeButton.Disable();
            if (m_usesDeleteButton)
                trashButton.Disable();
            inputFieldBackground.Disable();
        }

        private void DisableSubComponents()
        {
            if (m_usesFavoriteButton)
                likeButton.gameObject.SetActive(false);
            if (m_usesDeleteButton)
                trashButton.gameObject.SetActive(false);
        }

        private IEnumerator HoverDelay()
        {
            yield return new WaitForSeconds(hoverExitDelay);

            if (vignetteState == VignetteState.notHovering)
            {
                if (m_usesFavoriteButton)
                    likeButton.Disable();
                if (m_usesDeleteButton)
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

        internal void SetDeleteActive(bool pUsesDeleteButton)
        {
            m_usesDeleteButton = pUsesDeleteButton;
            trashButton.gameObject.SetActive(pUsesDeleteButton);
        }

        internal void SetFavoryActive(bool pUsesFavoriteButton)
        {
            m_usesFavoriteButton = pUsesFavoriteButton;
            likeButton.gameObject.SetActive(pUsesFavoriteButton);
        }
    }
}
