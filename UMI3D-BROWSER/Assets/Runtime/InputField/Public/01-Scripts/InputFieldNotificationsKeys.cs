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

namespace umi3d.browserRuntime.inputField
{
    public class InputFieldNotificationsKeys
    {
        public class InputFieldSet
        {
            public static readonly string IsTitleVisible = "inputfield-update-isTitleVisible";
            public static readonly string Title = "inputfield-update-title";
            public static readonly string Value = "inputfield-update-value";
            public static readonly string Placeholder = "inputfield-update-placeholder";
            public static readonly string NbrLine = "inputfield-update-nbrLine";
        }

        public class InputFieldUpdated
        {
            public static readonly string Value = "inputfield-update-value";
        }
    }
}