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

using inetum.unityUtils;
using System;
using System.Collections.Generic;

namespace umi3d.browserRuntime.ui
{
    /// <summary>
    /// Model of a dropdown element
    /// </summary>
    public class DropdownModel : ILabelSubject, IValueSubject<string>, IDropdownOptionsSubject, ISubmitObserver
    {
        List<string> _options = new();

        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public string value { get; private set; }
        public IEnumerator<string> options => _options.GetEnumerator();
        public event Action submit;

        public int IndexOf(string value)
        {
            return _options.IndexOf(value);
        }

        #region IDropdownSubject

        LabelSubject labelSubject = new LabelSubject();

        public void Subscribe(ILabelObserver observer)
        {
            labelSubject.Subscribe(observer);
        }

        public void Unsubscribe(ILabelObserver observer)
        {
            labelSubject.Unsubscribe(observer);
        }

        void NotifyLabelObserver()
        {
            labelSubject.NotifyLabelObserver(label, isLabelVisible);
        }

        ValueSubject<string> valueSubject = new();

        public void Subscribe(IValueObserver<string> observer)
        {
            valueSubject.Subscribe(observer);
        }

        public void Unsubscribe(IValueObserver<string> observer)
        {
            valueSubject.Unsubscribe(observer);
        }

        void NotifyValueObserver()
        {
            valueSubject.NotifyValueObserver(value);
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

        public void OnSubmit()
        {
            submit?.Invoke();
        }

        public void Clear()
        {
            submit = null;
        }

        public string debugString
        {
            get
            {
                string result = "---- DropdownModel ----\n";

                result += $"{isLabelVisible}, {label}, {value}, {_options.ToString<string>()}";

                return result;
            }
        }
    }
}