using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using umi3d.cdk;
using umi3d.common.interaction.form;
using umi3d.common.interaction.form.ugui;
using UnityEngine;

public static class DivFormExtensionHelper
{
    public async static Task<Sprite> GetSprite(this ImageDto imageDto)
    {
        Sprite sprite = null;
        if (imageDto.resource != null)
            try
            {
                object spriteTask = await UMI3DResourcesManager.Instance._LoadFile(0,
                    imageDto.resource.variants[0],
                    new ImageDtoLoader()
                );

                Texture2D texture = spriteTask as Texture2D;
                sprite = Sprite.Create(texture,
                                    new Rect(0, 0, texture.Size().x, texture.Size().y),
                                    new Vector2());
            }
            catch (Exception ex)
            {
                Debug.LogException(new Exception("Make sure you are in play mode to load resource in the form," +
                    " or that every networking UMI3D behaviors are ready"));
            }

        return sprite;
    }
    public async static Task<Sprite> GetSprite(this ButtonDto buttonDto)
    {
        Sprite sprite = null;
        if (buttonDto.resource != null)
            try
            {
                object spriteTask = await UMI3DResourcesManager.Instance._LoadFile(0,
                    buttonDto.resource.variants[0],
                    new ImageDtoLoader()
                );

                Texture2D texture = spriteTask as Texture2D;
                sprite = Sprite.Create(texture,
                                    new Rect(0, 0, texture.Size().x, texture.Size().y),
                                    new Vector2());
            }
            catch (Exception ex)
            {
                Debug.LogException(new Exception("Make sure you are in play mode to load resource in the form," +
                    " or that every networking UMI3D behaviors are ready"));
            }

        return sprite;
    }

    public static Vector2 Size(this Texture2D texture)
    {
        return new Vector2(texture.width, texture.height);
    }

    public static Style GetStyle(this DivDto divDto)
    {
        Style style = new();
        if (divDto.styles == null)
            return style;

        foreach (var styleDto in divDto.styles)
        {
            if (styleDto.variants == null)
                continue;

            var variantDto = styleDto.variants.Find(variantDto => variantDto is UGUIStyleVariantDto) as UGUIStyleVariantDto;
            if (variantDto == null)
                continue;

            foreach (var styleItemDto in variantDto.StyleVariantItems)
                style.Apply(styleItemDto);
        }

        return style;
    }

    public class Style
    {
        public Vector2? Position { get; set; }
        public Vector2? Size { get; set; }
        public Vector2? AnchorMax { get; set; }
        public Vector2? AnchorMin { get; set; }
        public Vector2? Pivot { get; set; }
        public Color? Color { get; set; }
        public Color? HoverColor { get; set; }
        public int? FontSize { get; set; }
        public Color? FontColor { get; set; }
        public FontStyles? FontStyles { get; set; }
        public TextAlignmentOptions? FontAlignmentOptions { get; set; }

        internal void Apply(UGUIStyleItemDto styleItemDto)
        {
            switch (styleItemDto)
            {
                case PositionStyleDto positionStyleVariant:
                {
                    Position = new Vector2(positionStyleVariant.posX, positionStyleVariant.posY);
                    break;
                }
                case SizeStyleDto sizeStyleVariant:
                {
                    Size = new Vector2(sizeStyleVariant.width, sizeStyleVariant.height);
                    break;
                }
                case AnchorStyleDto anchorStyleVariant:
                {
                    AnchorMax = new Vector2(anchorStyleVariant.maxX, anchorStyleVariant.maxY);
                    AnchorMin = new Vector2(anchorStyleVariant.minX, anchorStyleVariant.minY);
                    Pivot = new Vector2(anchorStyleVariant.pivotX, anchorStyleVariant.pivotY);
                    break;
                }
                case ColorStyleDto colorStyleVariant:
                {
                    Color = colorStyleVariant.color.Struct();
                    break;
                }
                case HoverColorStyleDto hovercolorStyleVariant:
                {
                    HoverColor = hovercolorStyleVariant.color.Struct();
                    break;
                }
                case TextStyleDto textStyleVariant:
                {
                    FontSize = (int)textStyleVariant.fontSize;
                    FontColor = textStyleVariant.color?.color?.Struct();
                    FontStyles = GetFontStyle(textStyleVariant.fontStyles);
                    FontAlignmentOptions = GetAlignement(textStyleVariant.fontAlignments);
                    break;
                }
            }
        }



        private static TextAlignmentOptions GetAlignement(List<E_FontAlignment> alignments)
        {
            var options = new TextAlignmentOptions();
            if (alignments == null)
                return options;

            foreach (E_FontAlignment alignment in alignments)
            {
                switch (alignment)
                {
                    case E_FontAlignment.Left:
                        options = TextAlignmentOptions.Left;
                        break;
                    case E_FontAlignment.Center:
                        options = TextAlignmentOptions.Center;
                        break;
                    case E_FontAlignment.Right:
                        options = TextAlignmentOptions.Right;
                        break;
                    case E_FontAlignment.Justified:
                        options = TextAlignmentOptions.Justified;
                        break;
                    case E_FontAlignment.Flush:
                        options = TextAlignmentOptions.Flush;
                        break;
                    case E_FontAlignment.GeometryCenter:
                        options = TextAlignmentOptions.CenterGeoAligned;
                        break;
                    case E_FontAlignment.Top:
                        options = TextAlignmentOptions.Top;
                        break;
                    case E_FontAlignment.Middle:
                        break;
                    case E_FontAlignment.Bottom:
                        options = TextAlignmentOptions.Bottom;
                        break;
                    case E_FontAlignment.Baseline:
                        options = TextAlignmentOptions.Baseline;
                        break;
                    case E_FontAlignment.Midline:
                        options = TextAlignmentOptions.Midline;
                        break;
                    case E_FontAlignment.Capline:
                        options = TextAlignmentOptions.Capline;
                        break;
                }
            }

            return options;
        }

        private static FontStyles GetFontStyle(List<E_FontStyle> fontStyles)
        {
            var style = new FontStyles();
            if (fontStyles == null)
                return style;

            foreach (E_FontStyle fontStyle in fontStyles)
            {
                switch (fontStyle)
                {
                    case E_FontStyle.Bold:
                        style |= TMPro.FontStyles.Bold;
                        break;
                    case E_FontStyle.Italic:
                        style |= TMPro.FontStyles.Italic;
                        break;
                    case E_FontStyle.Underline:
                        style |= TMPro.FontStyles.Underline;
                        break;
                    case E_FontStyle.Strikethrough:
                        style |= TMPro.FontStyles.Strikethrough;
                        break;
                    case E_FontStyle.Lowercase:
                        style |= TMPro.FontStyles.LowerCase;
                        break;
                    case E_FontStyle.Uppercase:
                        style |= TMPro.FontStyles.UpperCase;
                        break;
                    case E_FontStyle.Smallcaps:
                        style |= TMPro.FontStyles.SmallCaps;
                        break;
                }
            }
            return style;
        }
    }
}
