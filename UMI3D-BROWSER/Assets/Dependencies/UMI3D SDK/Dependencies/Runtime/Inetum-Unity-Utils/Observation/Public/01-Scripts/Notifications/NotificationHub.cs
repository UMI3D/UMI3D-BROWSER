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
using System.Linq;

namespace inetum.unityUtils.observation
{
    public class NotificationHub : INotificationHub
    {
        /// <summary>
        /// The default instance of <see cref="NotificationHub"/>.
        /// </summary>
        public static NotificationHub Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new();
                }
                return _default;
            }
        }
        static NotificationHub _default;

        /// <summary>
        /// ID to subscriptions.
        /// </summary>
        Dictionary<string, List<Subscription>> _subscriptions = new();
        /// <summary>
        /// Subscriber to IDs.
        /// </summary>
        Dictionary<object, HashSet<string>> _subscriberToID = new();
        /// <summary>
        /// The status of notification for a given ID.
        /// </summary>
        Dictionary<string, bool> notifyStatus = new();

        static readonly object[] emptySubscribers = new object[0];
        /// <summary>
        /// Retrieves the subscribers for a given ID.<br/>
        /// If the ID is null or empty, logs an error and returns an empty array of subscribers.<br/>
        /// If there are no subscriptions for the given ID, returns an empty array of subscribers.<br/>
        /// Otherwise, returns the list of subscribers associated with the given ID.<br/>
        /// <br/>
        /// <example>
        /// Given an ID with subscriptions when getting subscribers for the ID then return the list of subscribers.<br/>
        /// <code>
        /// NotificationHub.Default.Subscribe(subscriber1, id, (Callback)(() => { }));
        /// NotificationHub.Default.Subscribe(subscriber2, id, (Callback)(() => { }));
        /// IEnumerable&lt;object&gt; subscribers = NotificationHub.Default.GetSubscribersFor(id); 
        /// // subscribers contains subscriber1 and subscriber2.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="id">The ID for which to retrieve subscribers.</param>
        /// <returns>An IEnumerable of subscribers for the given ID.</returns>
        public IEnumerable<object> GetSubscribersFor(ID id)
        {
            if (string.IsNullOrEmpty(id))
            {
                UnityEngine.Debug.LogError($"[NotificationHub.GetSubscribersFor] Error: id is null or empty.");
                return emptySubscribers;
            }

            if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                return emptySubscribers;
            }

            return subscriptions.Select(subscription => subscription.subscriber);
        }

        static readonly string[] emptyIds = new string[0];
        /// <summary>
        /// Retrieves the IDs associated with a given subscriber.<br/>
        /// If the subscriber is null, logs an error and returns an empty list of IDs.<br/>
        /// If there are no IDs associated with the given subscriber, returns an empty list of IDs.<br/>
        /// Otherwise, returns the list of IDs associated with the given subscriber.<br/>
        /// <br/>
        /// <example>
        /// Given a subscriber with subscriptions when getting IDs for the subscriber then return the list of IDs.<br/>
        /// <code>
        /// NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => { }));
        /// NotificationHub.Default.Subscribe(subscriber, id2, (Callback)(() => { }));
        /// IEnumerable&lt;string&gt; ids = NotificationHub.Default.GetIdsFor(subscriber);
        /// // ids contains id1 and id2.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="subscriber">The subscriber for which to retrieve IDs.</param>
        /// <returns>An IEnumerable of IDs associated with the given subscriber.</returns>
        public IEnumerable<string> GetIdsFor(object subscriber)
        {
            if (subscriber == null)
            {
                UnityEngine.Debug.LogError($"[NotificationHub.GetIdsFor] Error: subscriber is null.");
                return emptyIds;
            }

            if (!_subscriberToID.TryGetValue(subscriber, out HashSet<string> ids))
            {
                return emptyIds;
            }

            return ids;
        }

        /// <summary>
        /// Whether <paramref name="id"/> is being notified.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isNotifying(string id)
        {
            return notifyStatus.TryGetValue(id, out bool isNotifying) && isNotifying;
        }

        /// <summary>
        /// Whether id is being notified.
        /// </summary>
        /// <typeparam name="T">The id of the notification.</typeparam>
        /// <returns></returns>
        public bool isNotifying<T>()
        {
            return notifyStatus.TryGetValue(typeof(T).FullName, out bool isNotifying) && isNotifying;
        }

        public void Subscribe(
            object subscriber,
            ID id,
            Callback action,
            INotificationFilter publishersFilter = null
        )
        {
            if (string.IsNullOrEmpty(id))
            {
                UnityEngine.Debug.LogError($"[NotificationHub.Subscribe] Error: id is null or empty.");
                return;
            }

            if (subscriber == null)
            {
                UnityEngine.Debug.LogError($"[NotificationHub.Subscribe] Error: subscriber is null for id '{id}'.");
                return;
            }



            if (isNotifying(id))
            {
                string subscriberName = subscriber is string
                   ? subscriber as string
                   : subscriber.GetType().FullName;
                UnityEngine.Debug.LogError($"[{nameof(Subscribe)}] Try to subscribe {subscriberName} with id {id} while Notify is running with that id, that should not happen.");
            }

            // Create a subscription entry.
            Subscription subscription = new()
            {
                subscriber = subscriber,
                publishersFilter = publishersFilter,
                action = action
            };

            // Check if subscriptions already exist for that 'id'.
            if (_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                // If subscriptions already exist then add this subscription.
                subscriptions.Add(subscription);
            }
            else
            {
                // If no subscriptions exist for that 'id' create a new association 'id' -> subscriptions.
                _subscriptions.Add(id, new List<Subscription>() { subscription });
            }

            // Check if this 'subscriber' already listen to notifications.
            if (_subscriberToID.TryGetValue(subscriber, out HashSet<string> ids))
            {
                // Add the 'id' to the list of listen ids, if the list didn't contain this 'id' already.
                // This list is a set, that means there is no duplicate ids.
                ids.Add(id);
            }
            else
            {
                // If that 'subscriber' listen to no one, create a new association 'subscriber' -> ids.
                _subscriberToID.Add(subscriber, new HashSet<string>() { id });
            }
        }

        public void Unsubscribe(Object subscriber)
        {
            string subscriberName = subscriber is string
                ? subscriber as string
                : subscriber.GetType().FullName;

            // Check if that 'subscriber' listen to any notification.
            if (!_subscriberToID.TryGetValue(subscriber, out HashSet<string> ids))
            {
                UnityEngine.Debug.LogWarning($"[NotificationHub] Try to unsubscribe {subscriberName} but subscriber has not subscribed yet.");
                // If subscriber is not listening to notification then return;
                return;
            }

            // Loop through all the ids that this 'subscriber' is listening to.
            foreach (string id in ids)
            {
                // Check if subscriptions exist for 'id'.
                if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
                {
                    UnityEngine.Debug.LogError($"[{nameof(Unsubscribe)}] Try remove subscriptions for {subscriberName}. No subscription for {id}, that should not happen.");
                    continue;
                }

                if (isNotifying(id))
                {
                    UnityEngine.Debug.LogError($"[{nameof(Unsubscribe)}] Try remove subscriptions for {subscriberName}. Try to unsubscribe to {id} while Notify is running with that id, that should not happen.");
                    continue;
                }

                // Remove all the subscriptions concerning 'subscriber'.
                subscriptions.RemoveAll(sub => sub.subscriber == subscriber);

                // Remove id from the subscriptions if there is no more subscribers.
                if (subscriptions.Count == 0)
                {
                    _subscriptions.Remove(id);
                }
            }

            // Clear the ids.
            ids.Clear();

            // Remove 'subscriber' from '_subscriberToID'.
            _subscriberToID.Remove(subscriber);
        }

        public void Unsubscribe(Object subscriber, ID id)
        {
            string subscriberName = subscriber is string
                ? subscriber as string
                : subscriber.GetType().FullName;

            // Check if that 'subscriber' listen to any notification.
            if (!_subscriberToID.TryGetValue(subscriber, out HashSet<string> ids))
            {
                UnityEngine.Debug.LogWarning($"[NotificationHub] Try to unsubscribe {subscriberName} but subscriber has not subscribed yet.");
                // If subscriber is not listening to notifications then return;
                return;
            }

            // Check if 'subscriber' listen to 'id'.
            if (!ids.Contains(id))
            {
                UnityEngine.Debug.LogWarning($"[NotificationHub] Try to unsubscribe {subscriberName} with id {id} but subscriber has not subscribed to this id yet.");
                // If subscriber is not listening to 'id' then return;
                return;
            }

            if (isNotifying(id))
            {
                UnityEngine.Debug.LogError($"[{nameof(Unsubscribe)}] Try to unsubscribe {subscriberName} with id {id} while Notify is running with that id, that should not happen.");
                return;
            }

            // Remove this 'id' from the list of listen ids.
            ids.Remove(id);

            // If there is not more ids then remove 'subscriber' from '_subscriberToID'.
            if (ids.Count == 0)
            {
                _subscriberToID.Remove(subscriber);
            }

            // Check if subscriptions exist for 'id'.
            if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                UnityEngine.Debug.LogError($"[{nameof(Unsubscribe)}] Try remove subscriptions for {subscriberName}. No subscription for {id}, that should not happen.");
                return;
            }

            // Remove all the subscriptions concerning 'subscriber'.
            subscriptions.RemoveAll(sub => sub.subscriber == subscriber);

            // Remove id from the subscriptions if there is no more subscribers.
            if (subscriptions.Count == 0)
            {
                _subscriptions.Remove(id);
            }
        }

        public int Notify(
            Object publisher,
            ID id,
            Dictionary<string, Object> info = null,
            INotificationFilter subscribersFilter = null
        )
        {
            int observers = 0;

            // Check if there are subscription for that 'id'.
            if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                string publisherName = publisher is string
                ? publisher as string
                : publisher.GetType().FullName;

                UnityEngine.Debug.LogWarning($"[NotificationHub] {publisherName} try to notify with id {id} but no one is listening.");
                return observers;
            }

            // Create the notification.
            Notification notification = new Notification(id, publisher, info);

            notifyStatus[id] = true;

            for (int i = 0; i < subscriptions.Count; i++)
            {
                Subscription subscription = subscriptions[i];
                // filter the notification by subscribers and publishers.
                if ((subscribersFilter == null || subscribersFilter.IsAccepted(subscription.subscriber))
                    && (subscription.publishersFilter == null || subscription.publishersFilter.IsAccepted(publisher)))
                {
                    try
                    {
                        subscription.action(notification);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"[NotificationHub] Subscription action return an exception:\n " +
                            $"id: {id}, publisher: {publisher}, subscriber: {subscription.subscriber}");
                        UnityEngine.Debug.LogException(e);
                        continue;
                    }
                    observers++;
                }
            }

            notifyStatus[id] = false;

            return observers;
        }

        public Notifier GetNotifier(
            Object publisher,
            ID id,
            Dictionary<string, Object> info = null,
            INotificationFilter subscribersFilter = null
        )
        {
            return new Notifier(
                publisher,
                id,
                subscribersFilter,
                info,
                this
            );
        }

        /// <summary>
        /// Class representing a subscription to a notification.
        /// </summary>
        class Subscription
        {
            /// <summary>
            /// The object that wait for a notification. If subscriber is static then user typeof().FullName.
            /// </summary>
            public object subscriber;

            /// <summary>
            /// Only the notifications that pass this filter test can be sent to this <see cref="subscriber"/>.<br/>
            /// <br/>
            /// If null the <see cref="Subscriber"/> listen to everyone.
            /// </summary>
            public INotificationFilter publishersFilter;

            /// <summary>
            /// Action to execute when the notification is received.
            /// </summary>
            public Action<Notification> action;
        }
    }
}