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
using UnityEngine;

namespace umi3d.browserRuntime.worldController
{
    public class WorldControllersModel
    {
        static WorldControllersPSM _data;
        public WorldControllersPSM data
        {
            get
            {
                if (_data == null)
                {
                    _data = ScriptableObject.CreateInstance<WorldControllersPSM>();
                }

                return _data;
            }
        }

        /// <summary>
        /// Whether the data should be loaded or saved.<br/>
        /// <br/>
        /// If false then there is no loading and saving.
        /// </summary>
        bool isTesting;

        public WorldControllersModel(bool isTesting = false)
        {
            this.isTesting = isTesting;
            if (isTesting)
            {
                _data = null;
            }

            addedNotifier = NotificationHub.Default
                .GetNotifier(this, ID.FromType<WorldControllersNotificationKeys.Added>());

            updatedNotifier = NotificationHub.Default
                .GetNotifier(this, ID.FromType<WorldControllersNotificationKeys.Updated>());

            removedNotifier = NotificationHub.Default
                .GetNotifier(this, ID.FromType<WorldControllersNotificationKeys.Removed>());
        }

        #region Loading and Saving

        public void Load()
        {
            if (isTesting)
            {
                return;
            }

            data.Load();
        }

        public void Save()
        {
            if (isTesting)
            {
                return;
            }

            data.Save();
        }

        #endregion

        #region Simplify URL

        string SimplifyURL(string url)
        {
            url = url.TrimStart();
            url = url.TrimEnd();

            if (url.StartsWith("http://"))
            {
                url = url.Substring(7);
            }
            else if (url.StartsWith("https://"))
            {
                url = url.Substring(8);
            }

            return url;
        }

        #endregion

        #region Add

        Notifier addedNotifier;

        public void Add(WorldController worldController)
        {
            worldController.url = SimplifyURL(worldController.url);
            int index = data.worldControllers.FindIndex(wc => wc.url == worldController.url);
            if (index >= 0)
            {
                return;
            }

            _Add(worldController);
        }

        void _Add(WorldController worldController)
        {
            data.worldControllers.Add(worldController);

            addedNotifier[WorldControllersNotificationKeys.Added.WorldController] = worldController;
            addedNotifier.Notify();
        }

        #endregion

        #region Remove

        Notifier removedNotifier;

        public void Remove(WorldController worldController)
        {
            int index = data.worldControllers.IndexOf(worldController);
            if (index < 0)
            {
                return;
            }
            data.worldControllers.RemoveAt(index);

            removedNotifier[WorldControllersNotificationKeys.Removed.WorldController] = worldController;
        }

        #endregion

        #region Update

        Notifier updatedNotifier;

        public void Update(string url, string name, bool? isFavorite, System.DateTime? lastConnection)
        {
            url = SimplifyURL(url);
            int index = data.worldControllers.FindIndex(wc => wc.url == url);
            if (index < 0)
            {
                return;
            }

            WorldController worldController = data.worldControllers[index];
            if (name != null)
            {
                worldController.name = name;
            }
            if (isFavorite.HasValue)
            {
                worldController.isFavorite = isFavorite.Value;
            }
            if (lastConnection.HasValue)
            {
                worldController.lastConnection = lastConnection.Value;
            }

            _Update(index, worldController);
        }

        void _Update(int index, WorldController worldController)
        {
            data.worldControllers[index] = worldController;

            updatedNotifier[WorldControllersNotificationKeys.Updated.WorldController] = worldController;
            updatedNotifier.Notify();
        }

        #endregion

        #region Connection Succeeded

        public void OnConnectionSucceeded(string url, string name)
        {
            url = SimplifyURL(url);
            WorldController worldController;
            int index = data.worldControllers.FindIndex(wc => wc.url == url);
            if (index >= 0)
            {
                worldController = data.worldControllers[index];
                worldController.lastConnection = System.DateTime.Now;
                _Update(index, worldController);
            }
            else
            {
                worldController = new()
                {
                    url = url,
                    name = name,
                    isFavorite = false,
                    firstConnection = System.DateTime.Now,
                    lastConnection = System.DateTime.Now
                };
                _Add(worldController);
            }
        }

        #endregion

        #region Filtering and Sorting

        WorldControllersFilteringAndSorting _filteringAndSorting;
        WorldControllersFilteringAndSorting filteringAndSorting
        {
            get
            {
                if (_filteringAndSorting == null)
                {
                    _filteringAndSorting = new(data.worldControllers);
                }
                return _filteringAndSorting;
            }
        }

        #endregion
    }
}