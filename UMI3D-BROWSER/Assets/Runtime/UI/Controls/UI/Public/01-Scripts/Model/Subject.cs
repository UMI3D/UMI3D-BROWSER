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
    public class LabelSubject : ILabelSubject
    {
        List<ILabelObserver> _labelObservers = new();

        public void Subscribe(ILabelObserver observer)
        {
            if (_labelObservers.Contains(observer)) { return; }
            _labelObservers.Add(observer);
        }

        public void Unsubscribe(ILabelObserver observer)
        {
            _labelObservers.Remove(observer);
        }

        public void NotifyLabelObserver(string label, bool isLabelVisible)
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
    }

    public class ValueSubject<T> : IValueSubject<T>
    {
        List<IValueObserver<T>> _valueObserver = new();

        public void Subscribe(IValueObserver<T> observer)
        {
            if (_valueObserver.Contains(observer)) { return; }
            _valueObserver.Add(observer);
        }

        public void Unsubscribe(IValueObserver<T> observer)
        {
            _valueObserver.Remove(observer);
        }

        public void NotifyValueObserver(T value)
        {
            foreach (var observer in _valueObserver)
            {
                try
                {
                    observer.updateValue(value);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }
    }
}