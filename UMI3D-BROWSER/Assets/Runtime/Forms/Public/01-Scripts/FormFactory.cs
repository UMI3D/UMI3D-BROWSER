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
        [SerializeField] internal TabManager _tabManager;
        [SerializeField] internal ThumbnailListModelContainer _thumbnailListModelContainerPrefab;

        internal FormAnswerDto _answerDto;
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
                    _inputFieldFactory.CreateInputField(inputStringDto, container.Transform);
                    break;
                }
                case InputDto<int> inputIntDto:
                {
                    _inputFieldFactory.CreateInputField(inputIntDto, container.Transform);
                    break;
                }
                case RangeDto<int> rangeIntDto:
                {
                    _sliderFactory.CreateSlider(rangeIntDto, container.Transform);
                    break;
                }
                case RangeDto<float> rangeFloatDto:
                {
                    _sliderFactory.CreateSlider(rangeFloatDto, container.Transform);
                    break;
                }
                case ButtonDto buttonDto:
                {
                    await _buttonFactory.CreateButton(buttonDto, container.Transform, SendAnswer);
                    break;
                }
                case LabelDto labelDto:
                {
                    _textFactory.CreateText(labelDto, container.Transform);
                    break;
                }
                case ImageDto imageDto:
                {
                    // Normal Image
                    if (imageDto.FirstChildren == null || imageDto.FirstChildren.Count == 0)
                        await _imageFactory.CreateImage(imageDto, container.Transform);
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

                        thumbnailListModelContainer.Model.AddThumbnail(labelText, await imageDto.GetSprite());
                    }
                    break;
                }
                case PageDto pageDto:
                {
                    var content = _tabManager.AddNewTab(pageDto.name, false);
                    if (pageDto.FirstChildren != null)
                        foreach (var child in pageDto.FirstChildren)
                            AddDiv(child, new (content.transform));
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

        internal void SendAnswer(string submitId, bool isBack = false)
        {
            _answerDto = new() {
                submitId = submitId,
                isBack = isBack
            };

            _sendAnswerNotifier[FormNotificationKeys.SendAnswer.FormAnswerDto] = _answerDto;
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
            if (!_content)
                return;
#if UNITY_EDITOR
            for (int i = _content.childCount - 1; i >= 0; i--)
                DestroyImmediate(_content.GetChild(i).gameObject);
            _tabManager.Clear();
#else
            for (int i = _content.childCount - 1; i >= 0; i--)
                Destroy(_content.GetChild(i).gameObject);
            _tabManager.Clear();
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