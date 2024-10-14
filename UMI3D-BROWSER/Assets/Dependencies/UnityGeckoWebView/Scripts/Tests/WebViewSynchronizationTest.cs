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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    public class WebViewSynchronizationTest : MonoBehaviour
    {
        [SerializeField] Toggle isAdmin;
        [SerializeField] Button desynchronize;

        Notifier synchronisationAdministrationNotifier;
        Notifier desynchronizationNotifier;

        void Awake()
        {
            synchronisationAdministrationNotifier = NotificationHub.Default
                .GetNotifier<GeckoWebViewNotificationKeys.SynchronizationAdministrationChanged>(this);

            desynchronizationNotifier = NotificationHub.Default
                .GetNotifier<GeckoWebViewNotificationKeys.Desynchronization>(this);
        }

        void Start()
        {
            IsAdminValueChanged(isAdmin.isOn);
        }

        void OnEnable()
        {
            isAdmin.onValueChanged.AddListener(IsAdminValueChanged);
            desynchronize.onClick.AddListener(Desynchronize);
        }

        void OnDisable()
        {
            isAdmin.onValueChanged.RemoveListener(IsAdminValueChanged);
            desynchronize.onClick.RemoveListener(Desynchronize);
        }

        void IsAdminValueChanged(bool value) 
        {
            synchronisationAdministrationNotifier[GeckoWebViewNotificationKeys.SynchronizationAdministrationChanged.IsAdmin] = value;
            synchronisationAdministrationNotifier.Notify();
        }

        void Desynchronize()
        {
            desynchronizationNotifier.Notify();
        }
    }
}