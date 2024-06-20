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
        text.color = color;
    }

    public void SetPlaceHolder(List<string> placeHolder)
    {
        throw new System.NotImplementedException();
    }

    public void SetResource(object resource)
    {
        if (resource is TextStyleDto txtStyle)
        {
            text.fontSize = txtStyle.fontSize;
            for (int i = 0; i < txtStyle.fontStyles.Count; i++)
            {
                SetFontStyle(txtStyle.fontStyles[i]);
            }
            for (int i = 0; i < txtStyle.fontAlignments.Count; i++)
            {
                SetAlignement(txtStyle.fontAlignments[i]);
            }
        }
        if (resource is ImageDto imageStyle)
        {
            image.enabled = true;
            image.sprite = imageStyle.GetSprite().Result;
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
