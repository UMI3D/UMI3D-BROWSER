using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

[DisplayName("Base 2 Byte Formatter")]
public class ByteFormatter : FormatterBase
{
    public override string[] DefaultNames => new string[] { "byte" };

    public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.FormatterOptions != null && formattingInfo.FormatterOptions.Count() > 0)
        {
            // Here you can handle any additional options if needed
        }

        var provider = formattingInfo.FormatDetails.Provider;

        if (provider != null)
        {
            if (formattingInfo.CurrentValue is long sizeInBytes)
            {
                string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB" };
                double size = sizeInBytes;
                int suffixIndex = 0;

                while (size >= 1024 && suffixIndex < sizeSuffixes.Length - 1)
                {
                    size /= 1024;
                    suffixIndex++;
                }

                // Utilisation du format de chiffre de la langue en utilisant le formateur standard de la localisation de Unity
                string formattedNumber = string.Format(provider, "{0}", Math.Round(size, 2));
                string formattedFileSize = $"{formattedNumber} {sizeSuffixes[suffixIndex]}";

                formattingInfo.Write(formattedFileSize);

                return true;
            }
        }

        return false;
    }
}