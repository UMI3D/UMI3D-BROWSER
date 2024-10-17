using System.Collections.Generic;
using TMPro;
using umi3d.common.interaction.form.ugui;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ButtonDisplayer : MonoBehaviour, IDisplayer
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;

        public object GetValue(bool trim)
        {
            throw new System.NotImplementedException();
        }

        public void SetTitle(string title)
        {
            text.text = title;
        }

        public void SetColor(Color color)
        {
            image.color = color;
        }

        public void SetPlaceHolder(List<string> placeHolder)
        {
            throw new System.NotImplementedException();
        }

        public void SetResource(object resource)
        {
            if (resource is TextStyleDto textStyle)
            {
                if (textStyle.fontSize != 0)
                    text.fontSize = textStyle.fontSize;
                if (textStyle.color != null)
                    text.color = new Color() {
                        a = textStyle.color.color.A,
                        b = textStyle.color.color.B,
                        g = textStyle.color.color.G,
                        r = textStyle.color.color.R,
                    };
                if (textStyle.fontStyles != null)
                    SetFontStyle(textStyle.fontStyles);
                if (textStyle.fontAlignments != null)
                    foreach (var alignments in textStyle.fontAlignments)
                        SetAlignement(alignments);
            }
            if (resource is Sprite sprite && sprite != null)
            {
                image.enabled = true;
                image.sprite = sprite;
            }
        }

        private void SetAlignement(E_FontAlignment alignment)
        {
            switch (alignment)
            {
                case E_FontAlignment.Left:
                    text.alignment = TextAlignmentOptions.Left;
                    break;
                case E_FontAlignment.Center:
                    text.alignment = TextAlignmentOptions.Center;
                    break;
                case E_FontAlignment.Right:
                    text.alignment = TextAlignmentOptions.Right;
                    break;
                case E_FontAlignment.Justified:
                    text.alignment = TextAlignmentOptions.Justified;
                    break;
                case E_FontAlignment.Flush:
                    text.alignment = TextAlignmentOptions.Flush;
                    break;
                case E_FontAlignment.GeometryCenter:
                    text.alignment = TextAlignmentOptions.CenterGeoAligned;
                    break;
                case E_FontAlignment.Top:
                    text.alignment = TextAlignmentOptions.Top;
                    break;
                case E_FontAlignment.Middle:
                    break;
                case E_FontAlignment.Bottom:
                    text.alignment = TextAlignmentOptions.Bottom;
                    break;
                case E_FontAlignment.Baseline:
                    text.alignment = TextAlignmentOptions.Baseline;
                    break;
                case E_FontAlignment.Midline:
                    text.alignment = TextAlignmentOptions.Midline;
                    break;
                case E_FontAlignment.Capline:
                    text.alignment = TextAlignmentOptions.Capline;
                    break;
            }
        }

        private void SetFontStyle(List<E_FontStyle> fontStyles)
        {
            foreach (E_FontStyle fontStyle in fontStyles)
            {
                switch (fontStyle)
                {
                    case E_FontStyle.Bold:
                        text.fontStyle |= FontStyles.Bold;
                        break;
                    case E_FontStyle.Italic:
                        text.fontStyle |= FontStyles.Italic;
                        break;
                    case E_FontStyle.Underline:
                        text.fontStyle |= FontStyles.Underline;
                        break;
                    case E_FontStyle.Strikethrough:
                        text.fontStyle |= FontStyles.Strikethrough;
                        break;
                    case E_FontStyle.Lowercase:
                        text.fontStyle |= FontStyles.LowerCase;
                        break;
                    case E_FontStyle.Uppercase:
                        text.fontStyle |= FontStyles.UpperCase;
                        break;
                    case E_FontStyle.Smallcaps:
                        text.fontStyle |= FontStyles.SmallCaps;
                        break;
                }
            }
        }
    }
}