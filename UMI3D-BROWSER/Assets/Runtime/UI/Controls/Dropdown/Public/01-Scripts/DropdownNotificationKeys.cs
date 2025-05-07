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

using System.Collections.Generic;

namespace umi3d.browserRuntime.ui.dropdown
{
    public class DropdownNotificationKeys
    {
        /// <summary>
        /// Event raised when the dropdown is changed by the code
        /// </summary>

        public class DropdownSet
        {
            /// <summary>
            /// If display the label of the dropdown
            /// </summary>
            /// <remarks>
            /// Value is <see cref="bool"/>
            /// </remarks>
            public const string IsLabelVisible = "IsLabelVisible";
            /// <summary>
            /// The label of the dropdown
            /// </summary>
            /// <remarks>
            /// Value is <see cref="string"/>
            /// </remarks>
            public const string Label = "Label";
            /// <summary>
            /// The value of the dropdown
            /// </summary>
            /// <remarks>
            /// Value is <see cref="string"/>
            /// </remarks>
            public const string Value = "Value";
            /// <summary>
            /// The options of the dropdown
            /// </summary>
            /// <remarks>
            /// Value is <see cref="List{string}"/>
            /// </remarks>
            public const string Options = "Options";
        }
        
        /// <summary>
        /// Event raised when the dropdown is changed by the user
        /// </summary>
        public class DropdownUpdated
        {
            /// <summary>
            /// The new value
            /// </summary>
            /// <remarks>
            /// Value is <see cref="string"/>
            /// </remarks>
            public const string Value = "Value";
        }

    }
}