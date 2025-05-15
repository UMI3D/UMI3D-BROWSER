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

using System;
using System.Collections.Generic;
using TMPro;

namespace umi3d.browserRuntime.ui
{
    /// <summary>
    /// Model of an input field element
    /// </summary>
    public class InputFieldModel : ILabelSubject, IValueSubject<string>, IInputFieldPinSubject, IInputFieldContentTypeSubject, IInputFieldNbLineSubject, IInputFieldPlaceholderSubject, IInputFieldPasswordSubject, ISubmitObserver
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public string value { get; private set; }
        public string placeholder { get; private set; }
        public bool isMultiline { get; private set; } = false;
        public int nbrLine { get; private set; } = 1;
        public TMP_InputField.ContentType contentType { get; private set; } = TMP_InputField.ContentType.Standard;
        public bool passwordVisibility { get; private set; } = false;
        public bool isPin { get; private set; } = false;
        public event Action submit;

        #region ISubject

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

        List<IInputFieldPinObserver> _pinObserver = new();

        public void Subscribe(IInputFieldPinObserver observer)
        {
            if (!_pinObserver.Contains(observer))
            {
                _pinObserver.Add(observer);
            }
        }

        public void Unsubscribe(IInputFieldPinObserver observer)
        {
            _pinObserver.Remove(observer);
        }

        void NotifyPinObserver()
        {
            foreach (var observer in _pinObserver)
            {
                try
                {
                    observer.UpdatePin(isPin);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IInputFieldContentTypeObserver> _contentTypeObserver = new();

        public void Subscribe(IInputFieldContentTypeObserver observer)
        {
            if (!_contentTypeObserver.Contains(observer))
            {
                _contentTypeObserver.Add(observer);
            }
        }

        public void Unsubscribe(IInputFieldContentTypeObserver observer)
        {
            _contentTypeObserver.Remove(observer);
        }

        void NotifyContentTypeObserver()
        {
            foreach (var observer in _contentTypeObserver)
            {
                try
                {
                    observer.UpdateContentType(contentType);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IInputFieldNbLineObserver> _nbLineObserver = new();

        public void Subscribe(IInputFieldNbLineObserver observer)
        {
            if (!_nbLineObserver.Contains(observer))
            {
                _nbLineObserver.Add(observer);
            }
        }

        public void Unsubscribe(IInputFieldNbLineObserver observer)
        {
            _nbLineObserver.Remove(observer);
        }

        void NotifyNbLineObserver()
        {
            foreach (var observer in _nbLineObserver)
            {
                try
                {
                    observer.UpdateNbLine(nbrLine);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IInputFieldPlaceholderObserver> _placeholderObserver = new();

        public void Subscribe(IInputFieldPlaceholderObserver observer)
        {
            if (!_placeholderObserver.Contains(observer))
            {
                _placeholderObserver.Add(observer);
            }
        }

        public void Unsubscribe(IInputFieldPlaceholderObserver observer)
        {
            _placeholderObserver.Remove(observer);
        }

        void NotifyPlaceholderObserver()
        {
            foreach (var observer in _placeholderObserver)
            {
                try
                {
                    observer.UpdatePlaceholder(placeholder);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IInputFieldPasswordObserver> _passworldObserver = new();

        public void Subscribe(IInputFieldPasswordObserver observer)
        {
            if (!_passworldObserver.Contains(observer))
            {
                _passworldObserver.Add(observer);
            }
        }

        public void Unsubscribe(IInputFieldPasswordObserver observer)
        {
            _passworldObserver.Remove(observer);
        }

        void NotifyPasswordObserver()
        {
            foreach (var observer in _passworldObserver)
            {
                try
                {
                    observer.UpdatePassword(passwordVisibility);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        #endregion


        /// <summary>
        /// Sets the label and updates its visibility status.<br/>
        /// <br/>
        /// <example>
        /// Given a new label, when setting the label, then the label is updated and its visibility is set accordingly.
        /// <code>
        /// _model.SetLabel("Test Label"); // label = "Test Label", isLabelVisible = true
        /// _model.SetLabel(""); // label = "", isLabelVisible = false
        /// _model.SetLabel(null); // label = null, isLabelVisible = false
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newLabel">The new label to set.</param>
        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            NotifyLabelObserver();
        }

        /// <summary>
        /// Sets the value of the input field and notifies the change.<br/>
        /// <br/>
        /// <example>
        /// Given a new value when setting the value then the value is updated and notification is sent.
        /// <code>
        /// _model.SetValue("Test Value");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        public void SetValue(string newValue)
        {
            value = newValue;
            NotifyValueObserver();
        }

        /// /// <summary>
        /// This method sets a new placeholder for the input field and notifies the change.<br/>
        /// <br/>
        /// <example>
        /// Given a new placeholder string when setting the placeholder then the placeholder is updated and notification is sent.
        /// <code>
        /// _model.SetPlaceholder("New Placeholder");
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="newPlaceholder">The new placeholder string to set.</param>
        public void SetPlaceholder(string newPlaceholder)
        {
            placeholder = newPlaceholder;
            NotifyPlaceholderObserver();
        }

        /// /// <summary>
        /// This method sets a new number of lines for the input field and notifies the change.<br/>
        /// <br/>
        /// <example>
        /// Given a new number of lines when setting the number of lines then the number of lines is updated and notification is sent.
        /// <code>
        /// _model.SetNbrLines(2);
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="newNbrLine">The new number of lines to set.</param>
        public void SetNbrLines(bool NewIsMultiline, int newNbrLine)
        {
            isMultiline = NewIsMultiline;
            nbrLine = isMultiline ? newNbrLine : 1;
            NotifyNbLineObserver();
        }


        /// /// <summary>
        /// This method sets if the input field need to be shown with "*****" and notifies the change.<br/>
        /// <br/>
        /// <example>
        /// Given a new isPrivate when setting the privacy then the isPrivate is updated and notification is sent.
        /// <code>
        /// _model.SetContentType(true);
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="newNbrLine">The new number of lines to set.</param>
        public void SetContentType(TMP_InputField.ContentType contentType)
        {
            this.contentType = contentType;
            NotifyContentTypeObserver();
        }

        public void SetPasswordVisibility(bool visibility)
        {
            passwordVisibility = visibility;
            contentType = passwordVisibility ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            NotifyPasswordObserver();
        }

        public void SetIsPin(bool value)
        {
            isPin = value;
            NotifyPinObserver();
        }

        public void OnSubmit()
        {
            submit?.Invoke();
        }

        public void Clear()
        {
            submit = null;
        }
    }
}