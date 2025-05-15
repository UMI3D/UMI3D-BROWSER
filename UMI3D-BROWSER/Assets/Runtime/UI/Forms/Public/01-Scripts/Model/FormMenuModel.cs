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

using inetum.unityUtils.observation;
using System;
using System.Collections.Generic;
using umi3d.browserRuntime.forms;
using UnityEngine;

namespace umi3d.browserRuntime.ui.formMenu
{
    public class FormMenuModel : ISubmitSubject, IFormMenuDisplayFormSubject
    {
        public FormMenuModel()
        {
            NotificationHub.Default.Subscribe(this,
                ID.FromType<FormNotificationKeys.CreateForm>(),
                (Callback)CreateForm);
        }

        #region Subject

        List<ISubmitObserver> _submitObserver = new();

        public void Subscribe(ISubmitObserver observer)
        {
            if (!_submitObserver.Contains(observer))
            {
                _submitObserver.Add(observer);
            }
        }

        public void Unsubscribe(ISubmitObserver observer)
        {
            _submitObserver.Remove(observer);
        }

        void NotifySubmitObservers()
        {
            foreach (var observer in _submitObserver)
            {
                try
                {
                    observer.OnSubmit();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IFormMenuDisplayFormObserver> _displayFormObservers = new();

        public void Subscribe(IFormMenuDisplayFormObserver observer)
        {
            if (!_displayFormObservers.Contains(observer))
            {
                _displayFormObservers.Add(observer);
            }
        }

        public void Unsubscribe(IFormMenuDisplayFormObserver observer)
        {
            _displayFormObservers.Remove(observer);
        }

        void NotifyDisplayFormObservers(Action<IFormMenuDisplayFormObserver> action)
        {
            foreach (var observer in _displayFormObservers)
            {
                try
                {
                    action(observer);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        #endregion

        void CreateForm(Notification notification)
        {
            if (notification.TryGetInfoT(FormNotificationKeys.CreateForm.FormDto, out common.interaction.ConnectionFormDto legacyFormDto, false))
            {
                NotifyDisplayFormObservers(observer => observer.DisplayLegacyForm(legacyFormDto));
            }

            if (notification.TryGetInfoT(FormNotificationKeys.CreateForm.FormDto, out common.interaction.form.FormDto formDto, false))
            {
                NotifyDisplayFormObservers(observer => observer.DisplayForm(formDto));
            }
        }

        public void Submit()
        {
            NotifySubmitObservers();
        }
    }
}