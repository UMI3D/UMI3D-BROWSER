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
using umi3d.browserRuntime.thumbnails;
using umi3d.common;
using umi3d.common.interaction.form;
using umi3dBrowsers.container;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    public class FormFactory : MonoBehaviour
    {
        [SerializeField] internal Transform _content;
        [SerializeField] internal FormGroupFactory _groupFactory;
        [SerializeField] internal FormInputFieldFactory _inputFieldFactory;
        [SerializeField] internal FormSliderFactory _sliderFactory;
        [SerializeField] internal FormButtonFactory _buttonFactory;
        [SerializeField] internal FormImageFactory _imageFactory;
        [SerializeField] internal FormTextFactory _textFactory;
        [SerializeField] internal FormThumbnailFactory _thumbnailFactory;
        [SerializeField] internal TabManager _tabManager;

        internal FormAnswerDto _formAnswerDto = new FormAnswerDto();
        private Notifier _sendAnswerNotifier;

        private void Awake()
        {
            _sendAnswerNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<FormNotificationKeys.SendAnswer>());

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
            if (notification.TryGetInfoT(FormNotificationKeys.CreateForm.FormDto, out FormDto formDto, false))
                CreateForm(formDto);
        }

        public void CreateForm(FormDto formDto)
        {
            if (!_content)
                _content = transform;

            if (_content.childCount > 0)
                Clear();

            _formAnswerDto.formId = formDto.guid;

            if (formDto.FirstChildren != null)
                foreach (var child in formDto.FirstChildren)
                    AddDiv(child, new Container(_content));
        }

        internal async void AddDiv(DivDto divDto, Container container)
        {
            if (divDto == null || container == null)
                return;

            // TODO: Dropdown
            switch (divDto)
            {
                case GroupDto groupDto:
                {
                    var group = _groupFactory.CreateGroup(groupDto, container.Transform);
                    if (groupDto.FirstChildren != null)
                        foreach (var child in groupDto.FirstChildren)
                            AddDiv(child, new (group.transform));
                    break;
                }
                case InputDto<string> inputStringDto:
                {
                    _inputFieldFactory.CreateInputField(inputStringDto, container.Transform, _formAnswerDto);
                    break;
                }
                case InputDto<int> inputIntDto:
                {
                    _inputFieldFactory.CreateInputField(inputIntDto, container.Transform, _formAnswerDto);
                    break;
                }
                case RangeDto<int> rangeIntDto:
                {
                    _sliderFactory.CreateSlider(rangeIntDto, container.Transform, _formAnswerDto);
                    break;
                }
                case RangeDto<float> rangeFloatDto:
                {
                    _sliderFactory.CreateSlider(rangeFloatDto, container.Transform, _formAnswerDto);
                    break;
                }
                case ButtonDto buttonDto:
                {
                    await _buttonFactory.CreateButton(buttonDto, container.Transform, _formAnswerDto, SendAnswer);
                    break;
                }
                case LabelDto labelDto:
                {
                    _textFactory.CreateText(labelDto, container.Transform);
                    break;
                }
                case ImageDto imageDto:
                {
                    if (imageDto.FirstChildren == null || imageDto.FirstChildren.Count == 0) // Normal Image
                        await _imageFactory.CreateImage(imageDto, container.Transform);
                    else // Thumbnail
                        await _thumbnailFactory.CreateThumbnail(imageDto, container, SendAnswer);
                    break;
                }
                case PageDto pageDto:
                {
                    var content = _tabManager.AddNewTab(pageDto.name, false, () => _formAnswerDto.pageId = pageDto.guid);
                    Container pageContainer = new(content.transform);
                    if (pageDto.FirstChildren != null)
                        foreach (var child in pageDto.FirstChildren)
                            if (child != null)
                                AddDiv(child, pageContainer);
                    _tabManager.InitSelectedButtonById();
                    break;
                }
                default:
                {
                    Debug.LogError("[FormFactory] Div Dto not supported!");
                    break;
                }
            }
        }

        internal void SendAnswer(string submitId)
        {
            _formAnswerDto.submitId = submitId;
            _sendAnswerNotifier[FormNotificationKeys.SendAnswer.FormAnswerDto] = _formAnswerDto;
            _sendAnswerNotifier.Notify();

            Clear();
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
#if UNITY_EDITOR
            for (int i = _content.childCount - 1; i >= 0; i--)
                DestroyImmediate(_content.GetChild(i).gameObject);
            _tabManager.Clear();
            _formAnswerDto = new();
#else
            for (int i = _content.childCount - 1; i >= 0; i--)
                Destroy(_content.GetChild(i).gameObject);
            _tabManager.Clear();
            _formAnswerDto = new();
#endif
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