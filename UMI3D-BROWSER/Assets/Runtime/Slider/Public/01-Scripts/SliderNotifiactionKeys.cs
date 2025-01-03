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

namespace umi3d.browserRuntime.ui.slider
{
    public class SliderNotifiactionKeys 
    {
        /// <summary>
         /// Event raised when the slider is changed by the code
         /// </summary>
        public class SliderSet
        {
            /// <summary>
            /// If display the label of the slider
            /// </summary>
            /// <remarks>
            /// Value is <see cref="bool"/>
            /// </remarks>
            public const string IsLabelVisible = "IsLabelVisible";
            /// <summary>
            /// The label of the slider
            /// </summary>
            /// <remarks>
            /// Value is <see cref="string"/>
            /// </remarks>
            public const string Label = "Label";
            /// <summary>
            /// The value of the slider
            /// </summary>
            /// <remarks>
            /// Value is <see cref="float"/>
            /// </remarks>
            public const string Value = "Value";
            /// <summary>
            /// If the slider is integer or float
            /// </summary>
            /// <remarks>
            /// Value is <see cref="bool"/>
            /// </remarks>
            public const string IsInteger = "IsInteger";
        }

        /// <summary>
        /// Event raised when the input field is changed by the user
        /// </summary>
        public class SliderUpdated
        {
            /// <summary>
            /// The new value
            /// </summary>
            /// <remarks>
            /// Value is <see cref="float"/>
            /// </remarks>
            public const string Value = "Value";
        }
    }
}