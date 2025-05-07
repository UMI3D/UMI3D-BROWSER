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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inputField
{
    [RequireComponent(typeof(Button))]
    public class InputFieldPasswordVisibilityButton : MonoBehaviour
    {
        [SerializeField] private Sprite _passwordHidden;
        [SerializeField] private Sprite _passwordVisible;

        private Button _button;
        private InputFieldModelContainer _modelContainer;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _modelContainer = GetComponentInParent<InputFieldModelContainer>();

            _button.onClick.AddListener(OnClick);
            NotificationHub.Default.Subscribe(this,
                ID.FromType<InputFieldNotificationsKeys.InputFieldSet>(),
                (Callback)InputFieldSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            _modelContainer.model.SetPasswordVisibility(!_modelContainer.model.passwordVisibility);
            ((Image)_button.targetGraphic).sprite = _modelContainer.model.passwordVisibility ? _passwordVisible : _passwordHidden;
        }

        private void InputFieldSet(Notification notification)
        {
            if (notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldSet.ContentType, out TMP_InputField.ContentType contentType, false) && contentType == TMP_InputField.ContentType.Password)
                gameObject.SetActive(true);
        }
    }
}