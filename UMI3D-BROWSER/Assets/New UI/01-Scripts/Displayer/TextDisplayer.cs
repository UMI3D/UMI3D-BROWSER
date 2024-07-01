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
using TMPro;
using umi3d.common.interaction.form;
using umi3d.common.interaction.form.ugui;

//using umi3d.common.interaction.form;
//using umi3d.common.interaction.form.ugui;
using UnityEngine;

public class TextDisplayer : MonoBehaviour, IDisplayer
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform rectTransform;

    public object GetValue(bool trim)
    {
        throw new System.NotImplementedException();
    }

    public void SetTitle(string title)
    {
        text.text = title;
    }

    public void SetPlaceHolder(List<string> placeHolder)
    {
        throw new System.NotImplementedException();
    }

    public void SetColor(Color color)
    {
    }

    public void SetResource(object resource)
    {
        if (resource is TextStyleDto textStyle)
        {
            text.fontSize = textStyle.fontSize;
            text.color = new Color() {
                a = textStyle.color.color.A,
                b = textStyle.color.color.B,
                g = textStyle.color.color.G,
                r = textStyle.color.color.R,
            };
            if (textStyle.fontStyles != null)
                for (int i = 0; i < textStyle.fontStyles.Count; i++)
                    SetFontStyle(textStyle.fontStyles[i]);
            if (textStyle.fontAlignments != null)
                for (int i = 0; i < textStyle.fontAlignments.Count; i++)
                    SetAlignement(textStyle.fontAlignments[i]);
        }
    }

    private void SetAlignement(E_FontAlignment alignment)
    {
        switch (alignment)
        {
            case E_FontAlignment.Left:
                break;
            case E_FontAlignment.Center:
                break;
            case E_FontAlignment.Right:
                break;
            case E_FontAlignment.Justified:
                break;
            case E_FontAlignment.Flush:
                break;
            case E_FontAlignment.GeometryCenter:
                break;
            case E_FontAlignment.Top:
                break;
            case E_FontAlignment.Middle:
                break;
            case E_FontAlignment.Bottom:
                break;
            case E_FontAlignment.Baseline:
                break;
            case E_FontAlignment.Midline:
                break;
            case E_FontAlignment.Capline:
                break;
        }
    }

    private void SetFontStyle(E_FontStyle fontStyle)
    {
        switch (fontStyle)
        {
            case E_FontStyle.Bold:
                text.fontStyle = FontStyles.Bold;
                break;
            case E_FontStyle.Italic:
                text.fontStyle = FontStyles.Italic;
                break;
            case E_FontStyle.Underline:
                text.fontStyle = FontStyles.Underline;
                break;
            case E_FontStyle.Strikethrough:
                text.fontStyle = FontStyles.Strikethrough;
                break;
            case E_FontStyle.Lowercase:
                text.fontStyle = FontStyles.LowerCase;
                break;
            case E_FontStyle.Uppercase:
                text.fontStyle = FontStyles.UpperCase;
                break;
            case E_FontStyle.Smallcaps:
                text.fontStyle = FontStyles.SmallCaps;
                break;
        }
    }
}
