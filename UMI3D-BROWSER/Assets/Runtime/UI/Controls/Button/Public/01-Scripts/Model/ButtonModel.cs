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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui
{
    public class ButtonModel : ILabelSubject, IButtonImageSubject
    {
        public bool isLabelVisible { get; private set; } = false;
        public string Label { get; private set; } = string.Empty;
        public event Action Callback;
        public Sprite Sprite { get; private set; } = null;
        public ColorBlock ColorBlock { get; private set; } = new();

        #region Subject

        LabelSubject _labelSubject = new();

        public void Subscribe(ILabelObserver observer)
        {
            _labelSubject.Subscribe(observer);
        }

        public void Unsubscribe(ILabelObserver observer)
        {
            _labelSubject.Unsubscribe(observer);
        }

        void NotifyLabelObserver()
        {
            _labelSubject.NotifyLabelObserver(Label, isLabelVisible);
        }

        List<IButtonImageObserver> _imageObservers = new();

        public void Subscribe(IButtonImageObserver observer)
        {
            if (!_imageObservers.Contains(observer))
            {
                _imageObservers.Add(observer);
            }
        }

        public void Unsubscribe(IButtonImageObserver observer)
        {
            _imageObservers.Remove(observer);
        }

        void NotifyImageObserver()
        {
            foreach (var observer in _imageObservers)
            {
                try
                {
                    observer.UpdateImage(Sprite, ColorBlock);
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
            Label = newLabel;
            NotifyLabelObserver();
        }

        public void SetCallback(Action callback)
        {
            Callback = callback;
        }

        public void SetImage(ColorBlock? colors, Sprite sprite)
        {
            ColorBlock = colors ?? ColorBlock.defaultColorBlock;
            Sprite = sprite;
            NotifyImageObserver();
        }

        public void Click()
        {
            Callback?.Invoke();
        }
    }
}