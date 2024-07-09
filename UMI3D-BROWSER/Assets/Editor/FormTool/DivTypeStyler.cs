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
using MathNet.Numerics.Optimization.LineSearch;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        [Header("Color")]
        [SerializeField] private bool useColor = false;
        [SerializeField] private Color color = Color.white;

        //font
        private bool m_useFont = false;
        private TextMeshProUGUI _text = null;

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            if (_text != null) m_useFont = true;
        }


        internal StyleDto GetStyle()
        {
            StyleDto style = new();
            style.variants = new();
            style.variants.Add(GetUGUIVariant());

            return style;
        }

        private VariantStyleDto GetUGUIVariant()
        {
#if UNITY_EDITOR
            Awake();
#endif

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
            if (useColor)
            {
                ColorStyleDto colorStyle = GetColorStyle();
                if (colorStyle != null)
                    style.StyleVariantItems.Add(colorStyle);
            }
            if (m_useFont)
            {
                TextStyleDto textStyle = GetTextStyle();
                if (textStyle != null)
                    style.StyleVariantItems.Add(textStyle);
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

        private ColorStyleDto GetColorStyle()
        {
            ColorStyleDto colorStyle = new();
            colorStyle.color = new ColorDto();

            colorStyle.color.A = color.a;
            colorStyle.color.R = color.r;
            colorStyle.color.G = color.g;
            colorStyle.color.B = color.b;

            return colorStyle;
        }

        private TextStyleDto GetTextStyle()
        {
            TextStyleDto textStyle = new();

            ColorStyleDto colorStyle = new();
            colorStyle.color = new ColorDto();

            colorStyle.color.A = _text.color.a;
            colorStyle.color.R = _text.color.r;
            colorStyle.color.G = _text.color.g;
            colorStyle.color.B = _text.color.b;

            textStyle.fontSize = _text.fontSize;
            textStyle.color = colorStyle;
            textStyle.fontAlignments = GetCorrespondingFontAllignement(_text.alignment);
            textStyle.fontStyles = GetCorrespondingFontStyle(_text.fontStyle);
            return textStyle;
        }

        private List<E_FontAlignment> GetCorrespondingFontAllignement(TextAlignmentOptions options)
        {
            List<E_FontAlignment> allignements = new();
            switch (options)
            {
                case TextAlignmentOptions.TopLeft:
                    allignements.Add(E_FontAlignment.Top);
                    allignements.Add(E_FontAlignment.Left);
                    break;
                case TextAlignmentOptions.Top:
                    allignements.Add(E_FontAlignment.Top);
                    break;
                case TextAlignmentOptions.TopRight:
                    allignements.Add(E_FontAlignment.Top);
                    allignements.Add(E_FontAlignment.Right);
                    break;
                case TextAlignmentOptions.TopJustified:
                    allignements.Add(E_FontAlignment.Top);
                    allignements.Add(E_FontAlignment.Justified);
                    break;
                case TextAlignmentOptions.TopFlush:
                    allignements.Add(E_FontAlignment.Top);
                    allignements.Add(E_FontAlignment.Flush);
                    break;
                case TextAlignmentOptions.TopGeoAligned:
                    allignements.Add(E_FontAlignment.Top);
                    allignements.Add(E_FontAlignment.GeometryCenter);
                    break;
                case TextAlignmentOptions.Left:
                    allignements.Add(E_FontAlignment.Left);
                    break;
                case TextAlignmentOptions.Center:
                    allignements.Add(E_FontAlignment.Center);
                    break;
                case TextAlignmentOptions.Right:
                    allignements.Add(E_FontAlignment.Right);
                    break;
                case TextAlignmentOptions.Justified:
                    allignements.Add(E_FontAlignment.Justified);
                    break;
                case TextAlignmentOptions.Flush:
                    allignements.Add(E_FontAlignment.Flush);
                    break;
                case TextAlignmentOptions.CenterGeoAligned:
                    allignements.Add(E_FontAlignment.GeometryCenter);
                    allignements.Add(E_FontAlignment.Center);
                    break;
                case TextAlignmentOptions.BottomLeft:
                    allignements.Add(E_FontAlignment.Left);
                    allignements.Add(E_FontAlignment.Bottom);
                    break;
                case TextAlignmentOptions.Bottom:
                    allignements.Add(E_FontAlignment.Bottom);
                    break;
                case TextAlignmentOptions.BottomRight:
                    allignements.Add(E_FontAlignment.Right);
                    allignements.Add(E_FontAlignment.Bottom);
                    break;
                case TextAlignmentOptions.BottomJustified:
                    allignements.Add(E_FontAlignment.Justified);
                    allignements.Add(E_FontAlignment.Bottom);
                    break;
                case TextAlignmentOptions.BottomFlush:
                    allignements.Add(E_FontAlignment.Flush);
                    allignements.Add(E_FontAlignment.Bottom);
                    break;
                case TextAlignmentOptions.BottomGeoAligned:
                    allignements.Add(E_FontAlignment.GeometryCenter);
                    allignements.Add(E_FontAlignment.Bottom);
                    break;
                case TextAlignmentOptions.BaselineLeft:
                    allignements.Add(E_FontAlignment.Left);
                    allignements.Add(E_FontAlignment.Baseline);
                    break;
                case TextAlignmentOptions.Baseline:
                    allignements.Add(E_FontAlignment.Baseline);
                    break;
                case TextAlignmentOptions.BaselineRight:
                    allignements.Add(E_FontAlignment.Right);
                    allignements.Add(E_FontAlignment.Baseline);
                    break;
                case TextAlignmentOptions.BaselineJustified:
                    allignements.Add(E_FontAlignment.Justified);
                    allignements.Add(E_FontAlignment.Baseline);
                    break;
                case TextAlignmentOptions.BaselineFlush:
                    allignements.Add(E_FontAlignment.Flush);
                    allignements.Add(E_FontAlignment.Baseline);
                    break;
                case TextAlignmentOptions.BaselineGeoAligned:
                    allignements.Add(E_FontAlignment.GeometryCenter);
                    allignements.Add(E_FontAlignment.Baseline);
                    break;
                case TextAlignmentOptions.MidlineLeft:
                    allignements.Add(E_FontAlignment.Left);
                    allignements.Add(E_FontAlignment.Midline);
                    break;
                case TextAlignmentOptions.Midline:
                    allignements.Add(E_FontAlignment.Midline);
                    break;
                case TextAlignmentOptions.MidlineRight:
                    allignements.Add(E_FontAlignment.Right);
                    allignements.Add(E_FontAlignment.Midline);
                    break;
                case TextAlignmentOptions.MidlineJustified:
                    allignements.Add(E_FontAlignment.Justified);
                    allignements.Add(E_FontAlignment.Midline);
                    break;
                case TextAlignmentOptions.MidlineFlush:
                    allignements.Add(E_FontAlignment.Flush);
                    allignements.Add(E_FontAlignment.Midline);
                    break;
                case TextAlignmentOptions.MidlineGeoAligned:
                    allignements.Add(E_FontAlignment.Midline);
                    allignements.Add(E_FontAlignment.GeometryCenter);
                    break;
                case TextAlignmentOptions.CaplineLeft:
                    allignements.Add(E_FontAlignment.Left);
                    allignements.Add(E_FontAlignment.Capline);
                    break;
                case TextAlignmentOptions.Capline:
                    allignements.Add(E_FontAlignment.Capline);
                    break;
                case TextAlignmentOptions.CaplineRight:
                    allignements.Add(E_FontAlignment.Right);
                    allignements.Add(E_FontAlignment.Capline);
                    break;
                case TextAlignmentOptions.CaplineJustified:
                    allignements.Add(E_FontAlignment.Justified);
                    allignements.Add(E_FontAlignment.Capline);
                    break;
                case TextAlignmentOptions.CaplineFlush:
                    allignements.Add(E_FontAlignment.Flush);
                    allignements.Add(E_FontAlignment.Capline);
                    break;
                case TextAlignmentOptions.CaplineGeoAligned:
                    allignements.Add(E_FontAlignment.GeometryCenter);
                    allignements.Add(E_FontAlignment.Capline);
                    break;
                case TextAlignmentOptions.Converted:
                    break;
            }
            return allignements;
        }
        private List<E_FontStyle> GetCorrespondingFontStyle(FontStyles fontStyle)
        {
            List<E_FontStyle> styles = new();
            switch (fontStyle)
            {
                case FontStyles.Normal:
                    break;
                case FontStyles.Bold:
                    styles.Add(E_FontStyle.Bold);
                    break;
                case FontStyles.Italic:
                    styles.Add(E_FontStyle.Italic);
                    break;
                case FontStyles.Underline:
                    styles.Add(E_FontStyle.Underline);
                    break;
                case FontStyles.LowerCase:
                    styles.Add(E_FontStyle.Lowercase);
                    break;
                case FontStyles.UpperCase:
                    styles.Add(E_FontStyle.Uppercase);
                    break;
                case FontStyles.SmallCaps:
                    styles.Add(E_FontStyle.Smallcaps);
                    break;
                case FontStyles.Strikethrough:
                    styles.Add(E_FontStyle.Strikethrough);
                    break;
                case FontStyles.Superscript:
                    break;
                case FontStyles.Subscript:
                    break;
                case FontStyles.Highlight:
                    break;
            }
            return styles;
        }
    }
}
