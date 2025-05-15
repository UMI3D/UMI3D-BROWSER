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
using TMPro;
using umi3d.browserRuntime.ui.slider;
using UnityEngine;

namespace umi3d
{
    [RequireComponent(typeof(TMP_Text))]
    public class SliderValueView : MonoBehaviour
    {
        TMP_Text _text;

        SliderModelContainer _modelContainer;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _modelContainer = GetComponentInParent<SliderModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<SliderNotifiactionKeys.SliderSet>(),
                (Callback)SliderSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
            NotificationHub.Default.Subscribe(this,
                ID.FromType<SliderNotifiactionKeys.SliderUpdated>(),
                (Callback)SliderUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void SliderSet(Notification notification)
        {
            if (notification.TryGetInfoT(SliderNotifiactionKeys.SliderSet.Value, out float value))
                _text.text = value.ToString();
        }

        private void SliderUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(SliderNotifiactionKeys.SliderUpdated.Value, out float value))
                _text.text = value.ToString();
        }
    }
}