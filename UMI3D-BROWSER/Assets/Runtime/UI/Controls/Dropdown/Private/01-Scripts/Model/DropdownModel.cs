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
    /// <summary>
    /// Model of a dropdown element
    /// </summary>
    public class DropdownModel : IDropdownModel
    {
        List<string> _options = new();

        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public string value { get; private set; }
        public IEnumerator<string> options => _options.GetEnumerator();

        public int IndexOf(string value)
        {
            return _options.IndexOf(value);
        }

        #region IDropdownSubject
        
        List<IDropdownLabelObserver> _labelObservers = new();

        public void Subscribe(IDropdownLabelObserver observer)
        {
            if (!_labelObservers.Contains(observer)) { return; }
            _labelObservers.Add(observer);
        }

        public void Unsubscribe(IDropdownLabelObserver observer)
        {
            _labelObservers.Remove(observer);
        }

        void NotifyLabelObserver()
        {
            foreach (var observer in _labelObservers)
            {
                try
                {
                    observer.UpdateLabel(label, isLabelVisible);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IDropdownValueObserver> _valueObserver = new();

        public void Subscribe(IDropdownValueObserver observer)
        {
            if (!_valueObserver.Contains(observer)) { return; }
            _valueObserver.Add(observer);
        }

        public void Unsubscribe(IDropdownValueObserver observer)
        {
            _valueObserver.Remove(observer);
        }

        void NotifyValueObserver()
        {
            foreach (var observer in _valueObserver)
            {
                try
                {
                    observer.updateValue(label);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IDropdownOptionsObserver> _optionsObservers = new();

        public void Subscribe(IDropdownOptionsObserver observer)
        {
            if (!_optionsObservers.Contains(observer)) { return; }
            _optionsObservers.Add(observer);
        }

        public void Unsubscribe(IDropdownOptionsObserver observer)
        {
            _optionsObservers.Remove(observer);
        }

        void NotifyOptionsObserver()
        {
            foreach (var observer in _optionsObservers)
            {
                try
                {
                    observer.updateOptions(_options);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        #endregion

        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            NotifyLabelObserver();
        }

        public void SetValue(string newValue)
        {
            value = newValue;
            NotifyValueObserver();
        }
        
        public void SetValue(int index)
        {
            if (index < 0 || index >= _options.Count) { return; }

            value = _options[index];
            NotifyValueObserver();
        }

        public void SetOptions(List<string> newOptions)
        {
            _options = newOptions;
            NotifyOptionsObserver();
        }
    }
}