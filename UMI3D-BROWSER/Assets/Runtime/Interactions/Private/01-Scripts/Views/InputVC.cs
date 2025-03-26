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

using inetum.unityUtils.ui.canvas;
using System.Threading.Tasks;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.interactions
{
    internal class InputVC : MonoBehaviour, 
        IView,
        ICollectionViewController<HorizontalLayoutGroup, InputIconView>,
        ICollectionViewControllerDelegate<InputIconView>
    {
        RectTransform rectTransform;
        TMPro.TMP_Text textTMP;

        InteractableUIModel model;

        IView view => this;

        public CollectionViewController<HorizontalLayoutGroup, InputIconView> collectionVC { get; private set; } = new();

        #region ICollectionViewController

        [SerializeField] GameObject inputIconPrefab;

        public HorizontalLayoutGroup layoutGroup
        {
            get
            {
                HorizontalLayoutGroup _layoutGroup = null;
                if (!view.Set(ref _layoutGroup))
                {
                    UnityEngine.Debug.LogError($"Error: cannot set VerticalLayoutGroup");
                    return null;
                }

                return _layoutGroup;
            }
        }

        public ViewPooling<InputIconView> viewPooling { get; private set; }

        public Transform container => transform;

        #endregion

        #region ICollectionViewControllerDelegate

        public async void OnViewActivatedAt(InputIconView view, int index)
        {
            await Task.Yield();

            float width = 0f;
            int activeChildCount = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf) { continue; }

                activeChildCount++;
                width += child.GetComponent<RectTransform>().rect.width;
            }
            width += layoutGroup.padding.left + layoutGroup.padding.right;
            width += (activeChildCount - 1) * layoutGroup.spacing;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            VerticalLayoutGroup parentLayoutGroup = transform.parent.GetComponent<VerticalLayoutGroup>();
            parentLayoutGroup.CalculateLayoutInputHorizontal();
            parentLayoutGroup.SetLayoutHorizontal();
        }

        #endregion

        void Awake()
        {
            Debug.Assert(view.Set(ref rectTransform));
            Debug.Assert(view.Set(ref textTMP, 1));

            viewPooling = new(
                createFunc: () =>
                {
                    InputIconView view = Instantiate(inputIconPrefab).GetComponent<InputIconView>();
                    view.transform.SetParent(transform, false);

                    return view;
                }, actionOnGet: iconView =>
                {
                    iconView.gameObject.SetActive(true);
                }, actionOnRelease: iconView =>
                {
                    iconView.gameObject.SetActive(false);
                }
            );

            collectionVC.dataDelegate = this;
            collectionVC.delegates.Add(this);
        }

        public void SetEvent(EventDto @event)
        {
            textTMP.text = @event.name;
        }
    }
}