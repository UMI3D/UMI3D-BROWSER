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

using System.Collections.Generic;

namespace umi3d.browserRuntime.ui
{
    public interface IDropdownModel : ILabelSubject, IValueSubject<string>, IDropdownOptionsSubject
    {
        string label { get; }
        string value { get; }
        IEnumerator<string> options { get; }

        int IndexOf(string value);


        /// <summary>
        /// Sets the label of the dropdown and updates its visibility status.<br/>
        /// <br/>
        /// <example>
        /// Given a new label when setting the label then the label visibility is updated accordingly.
        /// <code>
        /// dropdownModel.SetLabel("New Label");
        /// // isLabelVisible = true
        /// // label = "New Label"
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newLabel">The new label to set.</param>
        public void SetLabel(string newLabel);


        /// <summary>
        /// Sets the value of the dropdown and notifies any listeners about the change.<br/>
        /// <br/>
        /// <example>
        /// Given a new value when setting the value then the value is updated and listeners are notified.
        /// <code>
        /// dropdownModel.SetValue("New Value");
        /// // value = "New Value"
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        public void SetValue(string newValue);

        /// <summary>
        /// Sets the value of the dropdown and notifies any listeners about the change.<br/>
        /// <br/>
        /// <example>
        /// Given a new index when setting the value then the value is updated and listeners are notified.
        /// <code>
        /// dropdownModel.SetValue(1);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="index">The index of the new value to set.</param>
        public void SetValue(int index);

        /// <summary>
        /// Sets the options of the dropdown and notifies any listeners about the change.<br/>
        /// <br/>
        /// <example>
        /// Given a new list of options when setting the options then the options are updated and listeners are notified.
        /// <code>
        /// dropdownModel.SetOptions(new List&lt;string> { "Option 1", "Option 2" });
        /// // options = new List&lt;string> { "Option 1", "Option 2" }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newOptions">The new list of options to set.</param>
        public void SetOptions(List<string> newOptions);
    }
}