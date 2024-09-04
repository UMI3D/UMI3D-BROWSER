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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyStyle : MonoBehaviour
    {
        enum Style
        {
            /// <summary>
            /// For the characters 'a', 'b', 'c', ...
            /// </summary>
            LightGrey,
            /// <summary>
            /// For the digital keys '1', '2', '3', ...
            /// </summary>
            DarkGrey,
            /// <summary>
            /// For the special common keys 'enter', ',', 'symb', ...
            /// </summary>
            BlackGrey
        }

        [SerializeField] Style style;

        Button button;

        /// <summary>
        /// For the characters 'a', 'b', 'c', ...
        /// </summary>
        const string lightGrey = "#97979C";
        const string lightGreyHover = "#C3C3C3";

        /// <summary>
        /// For the digital keys '1', '2', '3', ...
        /// </summary>
        const string darkGrey = "#656565";
        const string darkGreyHover = "#727272";

        /// <summary>
        /// For the special common keys 'enter', ',', 'symb', ...
        /// </summary>
        const string blackGrey = "#494949";
        const string blackGreyHover = "#727272";

        const string disable = "#C8C8C8";
        const string blue = "#5EE3F0";

        private void OnValidate()
        {
            switch (style)
            {
                case Style.LightGrey:
                    ApplyStyle(lightGrey, lightGreyHover);
                    break;
                case Style.DarkGrey:
                    ApplyStyle(darkGrey, darkGreyHover);
                    break;
                case Style.BlackGrey:
                    ApplyStyle(blackGrey, blackGreyHover);
                    break;
                default:
                    break;
            }
        }

        void ApplyStyle(string normalHex, string highlightHex)
        {
            ColorBlock colorBlock = new();

            colorBlock.normalColor = HexToColor(normalHex);
            colorBlock.highlightedColor = HexToColor(highlightHex);
            colorBlock.selectedColor = HexToColor(normalHex);
            colorBlock.pressedColor = HexToColor(blue);
            colorBlock.disabledColor = HexToColor(disable, 10);
            colorBlock.colorMultiplier = 1;
            colorBlock.fadeDuration = .1f;

            button = gameObject.GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }
            button.colors = colorBlock;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(button);
#endif
        }

        static Color HexToColor(string hex, byte alpha = 255)
        {
            //in case the string is formatted 0xFFFFFF
            hex = hex.Replace("0x", "");
            //in case the string is formatted #FFFFFF
            hex = hex.Replace("#", "");

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            byte a;
            if (hex.Length == 8)
            {
                //Only use alpha if the string has enough characters
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                // fully visible.
                a = alpha;
            }

            // Color32 and Color implicitly convert to each other.
            return new Color32(r, g, b, a);
        }
    }
}