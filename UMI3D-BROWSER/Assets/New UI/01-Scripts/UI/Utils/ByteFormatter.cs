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
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace umi3d.browserRuntime.ui.utils
{
    [DisplayName("Base 2 Byte Formatter")]
    public class ByteFormatter : FormatterBase
    {
        public override string[] DefaultNames => new string[] { "byte" };

        public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
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
}