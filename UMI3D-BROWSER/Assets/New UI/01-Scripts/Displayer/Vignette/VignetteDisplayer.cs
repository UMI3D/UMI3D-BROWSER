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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class VignetteDisplayer : MonoBehaviour
    {
        [SerializeField] private Color transprent = Color.gray;
        [Header("Vignette main Image")]
        [SerializeField] private Image vignetteImage;

        [Header("Like button")]
        [SerializeField] private Color normalLikeColor = Color.white;
        [SerializeField] private Color hoverLikeColor = Color.white;
        [Space]
        [SerializeField] private Image likeBackGround = null;
        [SerializeField] private Image likeIcon = null;
        [SerializeField] private Sprite normalHeart;
        [SerializeField] private Sprite clickedHeart;

        [Header("trash button")]
        [SerializeField] private Color normalTrashColor = Color.white;
        [SerializeField] private Color hoverTrashColor = Color.white;
        [Space]
        [SerializeField] private Image trashBackGround = null;
        [SerializeField] private Image trashIcon = null;
        [SerializeField] private Sprite normalTrashIcon;
        [SerializeField] private Sprite hoverTrashIcon;

        [Header("Input field backgroung")]
        [SerializeField] private Image IF_background;
        [SerializeField] private Image pen;

        [Header("Animation")]
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private void Awake()
        {
            transprent.a = 0;
        }

        public void HoverEnter(PointerEventData eventData)
        {
            Debug.Log("Enter");
        }

        public void HoverExit(PointerEventData eventData)
        {
            Debug.Log("Exit");
        }

        public void Click(PointerEventData eventData)
        {
            Debug.Log("Click");
        }
    }
}
