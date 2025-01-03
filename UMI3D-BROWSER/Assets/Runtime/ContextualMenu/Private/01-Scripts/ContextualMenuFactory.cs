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
using System.Collections.Generic;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    [RequireComponent(typeof(ContextualMenuInputFieldFactory))]
    [RequireComponent(typeof(ContextualMenuToggleFactory))]
    public class ContextualMenuFactory : MonoBehaviour
    {
        [SerializeField] Transform _content;

        ContextualMenuInputFieldFactory _inputFieldFactory;
        ContextualMenuToggleFactory _toggleFactory;
        List<GameObject> _inputFields;
        List<GameObject> _toggles;

        private void Awake()
        {
            _inputFieldFactory = GetComponent<ContextualMenuInputFieldFactory>();
            _toggleFactory = GetComponent<ContextualMenuToggleFactory>();
            _inputFields = new List<GameObject>();
            _toggles = new List<GameObject>();

            NotificationHub.Default.Subscribe<ContextualMenuNotificationKeys.AddParameter>(this, AddParameter);
            NotificationHub.Default.Subscribe<ContextualMenuNotificationKeys.Close>(this, Clean);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }


        public void AddParameter(Notification notification)
        {
            if (!notification.TryGetInfoT(ContextualMenuNotificationKeys.AddParameter.Parameter, out AbstractParameterDto parameter))
                return;

            switch (parameter)
            {
                case StringParameterDto stringParameter:
                {
                    _inputFields.Add(_inputFieldFactory.GetOrCreate(_content, stringParameter));
                    break;
                }
                case BooleanParameterDto booleanParameter:
                {
                    _toggles.Add(_toggleFactory.GetOrCreate(_content, booleanParameter));
                    break;
                }
            }
        }

        public void Clean()
        {
            foreach (var inputField in _inputFields)
                _inputFieldFactory.Return(inputField);
            foreach (var toggle in _toggles)
                _toggleFactory.Return(toggle);
        }
    }
}