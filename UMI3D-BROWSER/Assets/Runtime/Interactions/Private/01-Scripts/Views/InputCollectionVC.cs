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
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.interactions
{
    internal class InputCollectionVC : MonoBehaviour, 
        IView, 
        ICollectionViewController<VerticalLayoutGroup, InputVC>, 
        ICollectionViewControllerDelegate<InputVC>,
        IInteractableUIDelegate
    {
        RectTransform rectTransform;
        NameView nameView;

        InteractableUIModel model;

        IView view => this;

        CollectionViewController<VerticalLayoutGroup, InputVC> collectionVC = new();

        #region ICollectionViewController

        [SerializeField] GameObject inputPrefab;

        public VerticalLayoutGroup layoutGroup
        {
            get
            {
                VerticalLayoutGroup _layoutGroup = null;
                if (!view.Set(ref _layoutGroup))
                {
                    UnityEngine.Debug.LogError($"Error: cannot set VerticalLayoutGroup");
                    return null;
                }

                return _layoutGroup;
            }
        }

        public ViewPooling<InputVC> viewPooling { get; private set; }

        public Transform container => transform;

        #endregion

        #region ICollectionViewControllerDelegate

        public async void OnViewActivatedAt(InputVC view, int index)
        {
            await Task.Yield();

            float maxWidth = 0f;
            foreach (var activeView in viewPooling.activeViews)
            {
                float width = activeView.transform.GetComponent<RectTransform>().rect.width;
                if (width > maxWidth) { maxWidth = width; }
            }
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);

            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.SetLayoutHorizontal();
        }

        #endregion

        void Awake()
        {
            Debug.Assert(view.Set(ref rectTransform));
            Debug.Assert(view.Set(ref nameView, 0));

            gameObject.SetActive(false);

            viewPooling = new(() =>
            {
                InputVC view = Instantiate(inputPrefab).GetComponent<InputVC>();
                view.transform.SetParent(transform, false);

                return view;
            }, actionOnGet: view =>
            {
                view.gameObject.SetActive(true);
            }, actionOnRelease: view =>
            {
                view.gameObject.SetActive(false);
                view.collectionVC.DeactivateAllView();
            });

            collectionVC.dataDelegate = this;
            collectionVC.delegates.Add(this);
        }

        public void SetModel(InteractableUIModel model)
        {
            if (this.model != null)
            {
                this.model.delegates.Remove(this);
            }
            this.model = model;
            model.delegates.Add(this);
            nameView.SetModel(model);
        }

        public void OnChangeOfHoveringState(InteractableHoveringState oldState, InteractableHoveringState newState)
        {
            gameObject.SetActive(newState == InteractableHoveringState.Hover);

            if (newState == InteractableHoveringState.Hover)
            {
                foreach (var @event in model.dataDelegate.events)
                {
                    collectionVC.ActiveViewAtLastIndex((view, index) =>
                    {
                        view.SetEvent(@event);
                        view.collectionVC.ActiveViewAt(transform.childCount - 2);
                    });
                }
            }
            else
            {
                collectionVC.DeactivateAllView();
            }
        }
    }
}