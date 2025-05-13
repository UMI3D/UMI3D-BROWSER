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

namespace umi3d.browserRuntime.ui.inputField
{
    public class InputFieldNotificationsKeys
    {
        /// <summary>
        /// Event raised when the input field is changed by the user
        /// </summary>
        public class InputFieldUpdated
        {
            /// <summary>
            /// The new value
            /// </summary>
            /// <remarks>
            /// Value is <see cref="string"/>
            /// </remarks>
            public const string Value = "Value";

            public const string ContentType = "ContentType";
        }

        /// <summary>
        /// Event raised when an input field is selected
        /// </summary>
        public class Selected { }

        /// <summary>
        /// Event raised when an input field is deselected
        /// </summary>
        public class Deselected { }
    }
}