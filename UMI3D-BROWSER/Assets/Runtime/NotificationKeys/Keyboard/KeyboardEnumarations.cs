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

namespace umi3d.browserRuntime.ui.keyboard
{
    /// <summary>
    /// The animation settings for the keyboard.
    /// </summary>
    public enum KeyboardAnimationType
    {
        OpenOrClose,
        KeyPress
    }

    /// <summary>
    /// The keyboard special keys.
    /// </summary>
    public enum SpecialKey
    {
        Enter,
        Quit
    }

    /// <summary>
    /// The localisation version of the keyboard.
    /// </summary>
    public enum KeyboardLocalisationVersion
    {
        QWERTY,
        AZERTY
    }

    /// <summary>
    /// How the text of a text field will be updated.
    /// </summary>
    public enum TextFieldTextUpdate
    {
        /// <summary>
        /// Some characters will be added at the string position.
        /// </summary>
        AddCharacters,
        /// <summary>
        /// Some characters will be removed at the string position.
        /// </summary>
        RemoveCharacters,
        /// <summary>
        /// The text will be submit so text field that wait for submission will be updated.
        /// </summary>
        SubmitText
    }
}