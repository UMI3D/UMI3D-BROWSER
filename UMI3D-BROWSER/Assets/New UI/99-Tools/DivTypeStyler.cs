/*
Copyright 2019 - 2023 Inetum

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

using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.common;
using umi3d.common.interaction.form;
using umi3d.common.interaction.form.ugui;
using UnityEngine;

namespace form_generator
{
    [RequireComponent(typeof(DivTypeTagger))]
    public class DivTypeStyler : MonoBehaviour
    {
        [Header("To Track")]
        [SerializeField] private bool trackPosition = true;
        [SerializeField] private bool trackSize = true;

        [Header("Use anchors at your own risks (In dev)")]
        [SerializeField] private bool trackAnchor = false;

        internal StyleDto GetStyle()
        {
            StyleDto style = new();
            style.variants = new();
            style.variants.Add(GetUGUIVariant());

            return style;
        }

        private VariantStyleDto GetUGUIVariant()
        {
            UGUIStyleVariantDto style = new UGUIStyleVariantDto();
            style.StyleVariantItems = new();
            
            if (trackPosition)
            {
                PositionStyleDto positionStyle = GetPositionStyle();
                if (positionStyle != null)
                    style.StyleVariantItems.Add(positionStyle);
            }
            if (trackSize)
            {
                SizeStyleDto sizeStyle = GetSizeStyle();
                if (sizeStyle != null)
                    style.StyleVariantItems.Add(sizeStyle);
            }
            if (trackAnchor)
            {
                AnchorStyleDto anchorStyle = GetAnchorStyle();
                if (anchorStyle != null)
                    style.StyleVariantItems.Add(anchorStyle);
            }

            return style;
        }

        private PositionStyleDto GetPositionStyle()
        {
            PositionStyleDto style = new();
            RectTransform rect = GetComponent<RectTransform>();

            style.posZ = 0;
            style.posX = rect.anchoredPosition.x;
            style.posY = rect.anchoredPosition.y;

            return style;
        }

        private SizeStyleDto GetSizeStyle()
        {
            SizeStyleDto style = new();
            RectTransform rect = GetComponent<RectTransform>();

            style.width = rect.sizeDelta.x;
            style.height = rect.sizeDelta.y;

            return style;
        }

        private AnchorStyleDto GetAnchorStyle()
        {
            AnchorStyleDto anchorStyle = new();
            RectTransform rect = GetComponent<RectTransform>();

            anchorStyle.minX = rect.anchorMin.x;
            anchorStyle.minY = rect.anchorMin.y;
            anchorStyle.maxX = rect.anchorMax.x;
            anchorStyle.maxY = rect.anchorMax.y;
            anchorStyle.pivotX = rect.pivot.x;
            anchorStyle.pivotY = rect.pivot.y;

            return anchorStyle;
        }
    }
}
