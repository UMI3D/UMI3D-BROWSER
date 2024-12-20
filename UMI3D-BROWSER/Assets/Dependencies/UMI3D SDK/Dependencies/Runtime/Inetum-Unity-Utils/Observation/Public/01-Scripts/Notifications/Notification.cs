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

using System;
using System.Collections.Generic;

namespace inetum.unityUtils.observation
{
    /// <summary>
    /// The notification send by the publisher to the subscribers.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Id of the notification.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// The publisher of the notification.
        /// </summary>
        public Object Publisher { get; private set; }

        /// <summary>
        /// Additional information.<br/>
        /// <br/>
        /// key: Id of the information, Value: the additional information.
        /// </summary>
        public Dictionary<string, Object> Info { get; private set; }

        Notification() { }

        /// <summary>
        /// Initializes a new instance of the Notification class.<br/>
        /// Logs an error if the id is null or empty, or if the publisher is null.<br/>
        /// <br/>
        /// <example>
        /// Given an id, publisher, and info when constructing a Notification then create a notification.<br/>
        /// <code>
        /// Notification notification = new(id, publisher, info);
        /// // notification.ID == id
        /// // notification.Publisher == publisher
        /// // notification.Info == info
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="id">The unique identifier for the notification.</param>
        /// <param name="publisher">The publisher of the notification.</param>
        /// <param name="info">Additional information related to the notification.</param>
        public Notification(string id, object publisher, Dictionary<string, object> info)
        {
            if (string.IsNullOrEmpty(id))
            {
                UnityEngine.Debug.LogError($"[Notification.Notification] Error: create a new notification with id null or empty.");
            }

            if (publisher == null)
            {
                string message = $"[Notification.Notification] Error: create a new notification with a null publisher.\n" +
                    $"Having a null publisher is a bad practice because it increase complexity while debugging.";
                UnityEngine.Debug.LogError(message);
            }

            ID = id;
            Publisher = publisher;
            Info = info;
        }

        /// <summary>
        /// Try to get the information stored with this <paramref name="key"/>.<br/>
        /// Return true if the information exist, else false.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryGetInfo(string key, out Object info, bool logError = true)
        {
            // Key cannot be null.
            if (key == null)
            {
                info = null;
                if (logError)
                {
                    UnityEngine.Debug.LogError($"[Notification.TryGetInfo] Error: key is null for notification '{ID}'.");
                }
                return false;
            }

            string errorMessage;
            if (Info == null)
            {
                info = null;
                if (logError)
                {
                    errorMessage = $"[Notification.TryGetInfo] Error: key '{key}' not found for notification '{ID}'.\n" +
                        $"Reason: info is null.";
                    UnityEngine.Debug.LogError(errorMessage);
                }
                return false;
            }
            else if (Info.Count == 0)
            {
                info = null;
                if (logError)
                {
                    errorMessage = $"[Notification.TryGetInfo] Error: key '{key}' not found for notification '{ID}'.\n" +
                        $"Reason: info is empty.";
                    UnityEngine.Debug.LogError(errorMessage);
                }
                return false;
            }
            else if (!Info.TryGetValue(key, out info))
            {
                info = null;
                if (logError)
                {
                    errorMessage = $"[Notification.TryGetInfo] Error: key '{key}' not found for notification '{ID}'.\n" +
                        $"Reason: info does not contain '{key}'.";
                    UnityEngine.Debug.LogError(errorMessage);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try to get the information stored with this <paramref name="key"/>.<br/>
        /// Return true if the information exist and is of type <typeparamref name="T"/>, else false.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="info"></param>
        /// <param name="logError">Whether a log error will be display if no value is found.</param>
        /// <returns></returns>
        public bool TryGetInfoT<T>(string key, out T info, bool logError = true)
        {
            if (!TryGetInfo(key, out object infoObject, logError))
            {
                info = default;
                return false;
            }

            // Try to cast the information.
            if (infoObject is not T infoT)
            {
                info = default;
                if (infoObject == null)
                {
                    // If infoObject is not T but is null then return true.
                    return true;
                }

                if (logError)
                {
                    string error = $"[Notification.TryGetInfoT] Error: notification '{ID}' does not contain key '{key}' of type {typeof(T)}.\n" +
                        $"Type of the object is {infoObject.GetType()}.";
                    UnityEngine.Debug.LogError(error);
                }
                return false;
            }

            info = infoT;
            return true;
        }

        /// <summary>
        /// Try to get the information stored with this <paramref name="key"/>.<br/>
        /// Return true if the information exist and is of type <see cref="Nullable{T}"/>, else false.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="info"></param>
        /// <param name="logError">Whether a log error will be display if no value is found.</param>
        /// <returns></returns>
        public bool TryGetInfoNullableT<T>(string key, out Nullable<T> info, bool logError = true)
            where T : struct
        {
            if (!TryGetInfo(key, out object infoObject, logError))
            {
                info = default;
                return false;
            }

            // Try to cast the information.
            if (infoObject is not T infoT)
            {
                info = null;
                if (infoObject == null)
                {
                    // If infoObject is not T but is null then return true.
                    // No cast exist to Nullable<T>.
                    return true;
                }

                if (logError)
                {
                    string error = $"Notification: '{ID}' does not contain info id: '{key}' of type {typeof(T)}.";
                    error += $"\nType of the object is {infoObject.GetType()}";
                    UnityEngine.Debug.LogError(error);
                }
                return false;
            }

            info = infoT;
            return true;
        }

        /// <summary>
        /// Display a log error.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="infoKey"></param>
        public void LogError(string subscriber, string infoKey, string message = null)
        {
            string error = $"[{subscriber}] notification: '{ID}' does not contain info id: '{infoKey}'.";
            if (!string.IsNullOrEmpty(error))
            {
                error += "\n";
                error += message;
            }
            UnityEngine.Debug.LogError(error);
        }
    }
}
