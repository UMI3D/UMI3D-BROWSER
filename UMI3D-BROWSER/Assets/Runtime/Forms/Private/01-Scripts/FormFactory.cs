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
using umi3d.browserRuntime.button;
using umi3d.browserRuntime.image;
using umi3d.browserRuntime.text;
using umi3d.browserRuntime.thumbnails;
using umi3d.browserRuntime.ui.inputField;
using umi3d.browserRuntime.ui.slider;
using umi3d.common.interaction.form;
using umi3dBrowsers.container;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    internal class FormFactory : MonoBehaviour
    {
        [SerializeField] internal Transform _content;
        [SerializeField] internal Transform _groupPrefab;
        [SerializeField] internal InputFieldFactory _inputFieldFactory;
        [SerializeField] internal SliderFactory _sliderFactory;
        [SerializeField] internal ButtonFactory _buttonFactory;
        [SerializeField] internal ImageFactory _imageFactory;
        [SerializeField] internal TextFactory _textFactory;
        [SerializeField] internal TabManager _tabManager;
        [SerializeField] internal ThumbnailListModelContainer _thumbnailListModelContainerPrefab;

        private void Awake()
        {
            NotificationHub.Default.Subscribe(this,
                ID.FromType<FormNotificationKeys.CreateForm>(),
                (Callback)CreateForm);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void CreateForm(Notification notification)
        {
            if (notification.TryGetInfoT(FormNotificationKeys.CreateForm.FormDto, out FormDto formDto))
                CreateForm(formDto);
        }

        public void CreateForm(FormDto formDto)
        {
            if (!_content)
                _content = transform;

            if (_content.childCount > 0)
                Clear();

            if (formDto.FirstChildren != null)
                foreach (var child in formDto.FirstChildren)
                    AddDiv(child, new Container(_content));
        }

        internal async void AddDiv(DivDto divDto, Container container)
        {
            if (divDto == null || container == null)
                return;

            switch (divDto)
            {
                case GroupDto groupDto:
                {
                    var group = Instantiate(_groupPrefab);
                    group.transform.SetParent(container.Transform, false);
                    if (groupDto.FirstChildren != null)
                        foreach (var child in groupDto.FirstChildren)
                            AddDiv(child, new (group));
                    break;
                }
                case InputDto<string> inputStringDto:
                {
                    _inputFieldFactory.GetOrCreateInputField(
                        container.Transform, 
                        false,
                        inputStringDto.label,
                        inputStringDto.Value,
                        inputStringDto.PlaceHolder,
                        1,
                        TmpContentTypeFrom(inputStringDto.TextType));
                    break;
                }
                case InputDto<int> inputIntDto:
                {
                    _inputFieldFactory.GetOrCreateInputField(
                        container.Transform,
                        false,
                        inputIntDto.label,
                        inputIntDto.Value.ToString(),
                        inputIntDto.PlaceHolder.ToString(),
                        1,
                        TmpContentTypeFrom(inputIntDto.TextType));
                    break;
                }
                case RangeDto<int> rangeIntDto:
                {
                    _sliderFactory.GetOrCreateSlider(
                        container.Transform,
                        rangeIntDto.label,
                        rangeIntDto.Value,
                        rangeIntDto.Min,
                        rangeIntDto.Max,
                        true);
                    break;
                }
                case RangeDto<float> rangeFloatDto:
                {
                    _sliderFactory.GetOrCreateSlider(
                        container.Transform,
                        rangeFloatDto.label,
                        rangeFloatDto.Value,
                        rangeFloatDto.Min,
                        rangeFloatDto.Max,
                        false);
                    break;
                }
                case ButtonDto buttonDto:
                {
                    _buttonFactory.GetOrCreateButton(
                        container.Transform,
                        buttonDto.label,
                        null); // TODO answer dto
                    break;
                }
                case LabelDto labelDto:
                {
                    _textFactory.GetOrCreateText(
                        container.Transform,
                        labelDto.text);
                    break;
                }
                case ImageDto imageDto:
                {
                    // Normal Image
                    if (imageDto.FirstChildren == null || imageDto.FirstChildren.Count == 0)
                    {
                        _imageFactory.GetOrCreateImage(
                            container.Transform,
                            await imageDto.GetSprite());
                    } 
                    // Thumbnail
                    else
                    {
                        ThumbnailListModelContainer thumbnailListModelContainer = container.Transform.GetComponent<ThumbnailListModelContainer>();
                        if (!thumbnailListModelContainer)
                            thumbnailListModelContainer = ReplaceContainerWithPrefab(container, _thumbnailListModelContainerPrefab);
                        string labelText = null;
                        string headerText = null;

                        foreach (var child in imageDto.FirstChildren)
                        {
                            if (child is LabelDto labelDto)
                            {
                                if (labelDto.tag == null)
                                    labelText = labelDto.text;
                                else
                                    headerText = labelDto.text;
                            }
                        }

                        thumbnailListModelContainer.Model.AddThumbnail(
                            labelText,
                            await imageDto.GetSprite()
                            );
                    }
                    break;
                }
                case PageDto pageDto:
                {
                    var content = _tabManager.AddNewTab(pageDto.name, false);
                    if (pageDto.FirstChildren != null)
                        foreach (var child in pageDto.FirstChildren)
                            AddDiv(child, new (content.transform));
                    break;
                }
                default:
                {
                    Debug.LogError("[FormFactory] Div Dto not supported!");
                    break;
                }
            }
        }

        internal static T ReplaceContainerWithPrefab<T>(Container conatiner, T prefab) where T : MonoBehaviour
        {
            var newObject = Instantiate(prefab);
            newObject.transform.SetParent(conatiner.Transform.parent, false);
            foreach (Transform child in conatiner.Transform)
                child.SetParent(newObject.transform, false);

#if UNITY_EDITOR
            DestroyImmediate(conatiner.Transform.gameObject);
#else
            Destroy(conatiner.Transform.gameObject);
#endif
            conatiner.Transform = newObject.transform;

            return newObject;
        }

        internal void Clear()
        {
            if (!_content)
                return;
#if UNITY_EDITOR
            for (int i = _content.childCount - 1; i >= 0; i--)
                DestroyImmediate(_content.GetChild(i).gameObject);
#else
            for (int i = _content.childCount - 1; i >= 0; i--)
                Destroy(_content.GetChild(i).gameObject);
#endif
        }

        private TMP_InputField.ContentType TmpContentTypeFrom(TextType type)
        {
            switch (type)
            {
                case TextType.Text:
                    return TMP_InputField.ContentType.Standard;
                case TextType.Mail:
                    return TMP_InputField.ContentType.EmailAddress;
                case TextType.Password:
                    return TMP_InputField.ContentType.Password;
                case TextType.Phone:
                    return TMP_InputField.ContentType.IntegerNumber;
                case TextType.URL:
                    return TMP_InputField.ContentType.Standard;
                case TextType.Number:
                    return TMP_InputField.ContentType.IntegerNumber;
            }
            return TMP_InputField.ContentType.Standard;
        }

        internal class Container
        {
            public Transform Transform { get; set; }

            public Container(Transform content)
            {
                this.Transform = content;
            }
        }
    }
}