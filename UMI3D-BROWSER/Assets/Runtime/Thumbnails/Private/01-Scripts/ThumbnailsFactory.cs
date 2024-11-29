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
using UnityEngine;

namespace umi3d.browserRuntime.ui.thumbnails
{
    internal class ThumbnailsFactory : MonoBehaviour
    {
        [SerializeField] GameObject thumbnailPrefab;
        [SerializeField] GameObject emptyThumbnailPrefab;

        List<ThumbnailModelContainer> activatedThumbnails = new();
        List<ThumbnailModelContainer> deactivatedThumbnails = new();

        List<GameObject> activatedEmpties = new();
        List<GameObject> deactivatedEmpties = new();

        ThumbnailsModelContainer model;

        void Awake()
        {
            model = GetComponentInParent<ThumbnailsModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.Added>(
                this,
                Added
            );

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.Deleted>(
               this,
               Deleted
            );

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.ContentModeChanged>(
               this,
               ContentModeChanged
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void Added(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.Added.Thumbnail, out ThumbnailModel thumbnail))
            {
                return;
            }

            ThumbnailModelContainer modelContainer = FetchThumbnail();
            UpdateEmpties();
            ActiveThumbnail(modelContainer, thumbnail);
        }

        void Deleted(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.Deleted.Thumbnail, out ThumbnailModel thumbnail))
            {
                return;
            }

            ThumbnailModelContainer container = FindActivatedThumbnail(thumbnail);
            DeactivateThumbnail(container);
            UpdateEmpties();
        }

        void ContentModeChanged(Notification notification)
        {
            UpdateEmpties();
        }

        void UpdateEmpties()
        {
            int numberOfEmpties = model.model.NumberOfEmptyToDisplay();
            int diff;
            if (activatedEmpties.Count == numberOfEmpties)
            {
                return;
            }
            else if (activatedEmpties.Count < numberOfEmpties)
            {
                diff = numberOfEmpties - activatedEmpties.Count;
                for (int i = 0; i < diff; i++)
                {
                    AddEmpty();
                }
            }
            else
            {
                diff = activatedEmpties.Count - numberOfEmpties;
                for (int i = 0; i < diff; i++)
                {
                    RemoveEmpty();
                }
            }

            for (int i = 0; i < activatedEmpties.Count; i++)
            {
                activatedEmpties[i].transform.SetAsLastSibling();
            }
        }

        ThumbnailModelContainer FetchThumbnail()
        {
            ThumbnailModelContainer container = null;
            if (deactivatedThumbnails.Count == 0)
            {
                GameObject go = Instantiate(thumbnailPrefab);
                go.transform.SetParent(transform, false);
                container = go.GetComponent<ThumbnailModelContainer>();
            }
            else
            {
                container = deactivatedThumbnails[deactivatedThumbnails.Count - 1];
                deactivatedThumbnails.RemoveAt(deactivatedThumbnails.Count - 1);
            }

            activatedThumbnails.Add(container);
            container.transform.SetAsLastSibling();

            return container;
        }

        void ActiveThumbnail(ThumbnailModelContainer modelContainer, ThumbnailModel model)
        {
            modelContainer.SetModel(model);
            modelContainer.gameObject.SetActive(true);
        }

        ThumbnailModelContainer FindActivatedThumbnail(ThumbnailModel thumbnail)
        {

            int index = activatedThumbnails
                .FindIndex(_container => _container.model == thumbnail);
            if (index == -1)
            {
                UnityEngine.Debug.LogError($"Error: Try to remove a thumbnail that is not active.");
                return null;
            }
            ThumbnailModelContainer container = activatedThumbnails[index];
            activatedThumbnails.RemoveAt(index);
            deactivatedThumbnails.Add(container);

            return container;
        }

        void DeactivateThumbnail(ThumbnailModelContainer modelContainer)
        {
            modelContainer?.SetModel(null);
            modelContainer?.gameObject.SetActive(false);
        }

        void AddEmpty()
        {
            GameObject go;
            if (deactivatedEmpties.Count == 0)
            {
                go = Instantiate(emptyThumbnailPrefab);
                go.transform.SetParent(transform, false);
            }
            else
            {
                go = deactivatedEmpties[deactivatedEmpties.Count - 1];
                deactivatedEmpties.RemoveAt(deactivatedEmpties.Count - 1);
            }

            activatedEmpties.Add(go);
            go.transform.SetAsLastSibling();
            go.SetActive(true);
        }

        void RemoveEmpty()
        {
            if (activatedEmpties.Count == 0)
            {
                return;
            }

            GameObject go = activatedEmpties[activatedEmpties.Count - 1];
            activatedEmpties.RemoveAt(activatedEmpties.Count - 1);
            deactivatedEmpties.Add(go);
            go.SetActive(false);
        }
    }
}