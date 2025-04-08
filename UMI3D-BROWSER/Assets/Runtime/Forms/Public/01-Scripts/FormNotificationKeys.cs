/*
Copyright 2019 - 2025 Inetum

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

namespace umi3d.browserRuntime.forms
{
    public static class FormNotificationKeys
    {
        public class CreateForm
        {
            public readonly static string FormDto = "FormDto";
        }

        public class ItemSet
        {
            public static readonly string Position = "Position";
            public static readonly string Size = "Size";

            public static readonly string AnchorMin = "AnchorMin";
            public static readonly string AnchorMax = "AnchorMax";
            public static readonly string Pivot = "Pivot";

            public static readonly string TextFontSize = "TextFontSize";
            public static readonly string TextColor = "TextColor";
            public static readonly string TextStyles = "TextStyles";
            public static readonly string TextAlignmentOptions = "TextAlignmentOptions";
        }
    }
}