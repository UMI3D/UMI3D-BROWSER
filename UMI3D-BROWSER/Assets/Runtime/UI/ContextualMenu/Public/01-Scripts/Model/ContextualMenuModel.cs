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

using inetum.unityUtils.observation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.ui.tablet;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    public class ContextualMenuModel : IContextualMenuActivationSubject, IContextualMenuDisplayParameterSubject, ISubmitSubject
    {
        public bool isActive { get; private set; } = false;
        public ReadOnlyCollection<AbstractParameterDto> parameters => _parameters.AsReadOnly();

        List<AbstractParameterDto> _parameters = new();

        public ContextualMenuModel()
        {
            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ParameterInputFound>(),
                (Callback)ParameterInputFound
            );
            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ToolReleased>(),
                (Callback)ToolReleased
            );

            NotificationHub.Default.Subscribe(this,
                ID.FromType<TabletNotificationKeys.Opened>(),
                (Callback)Hide);
        }

        #region Subject

        List<IContextualMenuActivationObserver> _activationObservers = new();

        public void Subscribe(IContextualMenuActivationObserver observer)
        {
            if (!_activationObservers.Contains(observer))
            {
                _activationObservers.Add(observer);
            }
        }

        public void Unsubscribe(IContextualMenuActivationObserver observer)
        {
            _activationObservers.Remove(observer);
        }

        void NotifyActivationObserver()
        {
            foreach (var observer in _activationObservers)
            {
                try
                {
                    observer.UpdateActivation(isActive);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

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

        void NotifySubmitObserver()
        {
            foreach (var observer in _submitObserver)
            {
                try
                {
                    observer.OnSubmit();
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<IContextualMenuDisplayParameterObserver> _displayParameterObservers = new();

        public void Subscribe(IContextualMenuDisplayParameterObserver observer)
        {
            if (!_displayParameterObservers.Contains(observer))
            {
                _displayParameterObservers.Add(observer);
            }
        }

        public void Unsubscribe(IContextualMenuDisplayParameterObserver observer)
        {
            _displayParameterObservers.Remove(observer);
        }

        void NotifyDisplayParameterObserver(AbstractParameterDto dto, Projection projection)
        {
            foreach (var observer in _displayParameterObservers)
            {
                try
                {
                    observer.Display(dto, projection);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        #endregion

        void ParameterInputFound(Notification notification)
        {
            if (!notification.TryGetInfoT(InteractionNotificationKeys.ParameterInputFound.parameterDto, out AbstractParameterDto dto))
            {
                return;
            }

            _parameters.Add(dto);
        }
        void ToolReleased(Notification notification)
        {
            _parameters.Clear();
            if (isActive)
            {
                SetActive(false);
            }
        }

        public void DisplayParameters()
        {
            var reversedParameters = new List<AbstractParameterDto>(_parameters);
            reversedParameters.Reverse(); // Reverse to show element above in front (layout in the object is set to reverse too)

            foreach (var param in reversedParameters)
            {
                NotifyDisplayParameterObserver(param, null);
            }
        }

        public void SetActive(bool active)
        {
            isActive = active;
            NotifyActivationObserver();

            if (isActive)
            {
                NotificationHub.Default.Notify(this, ID.FromType<ContextualMenuNotificationKeys.Open>());
            }
            else
            {
                NotificationHub.Default.Notify(this, ID.FromType<ContextualMenuNotificationKeys.Close>());
            }
        }

        void Hide(Notification notification)
        {
            SetActive(false);
        }

        public void Submit()
        {
            NotifySubmitObserver();
        }
    }
}