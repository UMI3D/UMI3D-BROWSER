using System.Collections.Generic;
using TMPro;
using umi3d.common.interaction.form.ugui;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UI;

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
            text.fontSize = textStyle.fontSize;
            text.color = new Color() {
                a = textStyle.color.color.A,
                b = textStyle.color.color.B,
                g = textStyle.color.color.G,
                r = textStyle.color.color.R,
            };
            if (textStyle.fontStyles != null)
                foreach (var style in textStyle.fontStyles)
                    SetFontStyle(style);
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
