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
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;

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

        readonly object _lockObject = new object();

        /// <summary>
        /// ID to subscriptions.
        /// </summary>
        Dictionary<string, List<Subscription>> _subscriptions = new();
        /// <summary>
        /// An empty subscribers array to return when there are no subscriptions for the given ID.
        /// </summary>
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

            return subscriptions
                .Select(subscription => subscription.subscriber)
                .Distinct();
        }

        /// <summary>
        /// Subscriber to IDs.
        /// </summary>
        Dictionary<object, HashSet<string>> _subscriberToID = new();
        /// <summary>
        /// An empty ids array to return when there are no IDs associated with the given subscriber 
        /// </summary>
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
        /// This method returns the number of subscriptions for a given subscriber.<br/>
        /// If an ID is provided, it counts the subscriptions for that specific ID.<br/>
        /// If no ID is provided, it counts all subscriptions for the subscriber across all IDs.<br/>
        /// <br/>
        /// <example>
        /// Given a subscriber and an optional ID, when getting the number of subscriptions for the subscriber, then return the count of subscriptions.<br/>
        /// <code>
        /// NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => { }));
        /// NotificationHub.Default.Subscribe(subscriber, id2, (Callback)(() => { }));
        ///
        /// NotificationHub.Default.NumberOfSubscriptionsFor(subscriber); // return 2
        /// NotificationHub.Default.NumberOfSubscriptionsFor(subscriber, id1); // return 1
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="subscriber">The subscriber object.</param>
        /// <param name="id">The optional ID for which to count subscriptions.</param>
        /// <returns>The number of subscriptions for the given subscriber and optional ID.</returns>
        public int NumberOfSubscriptionsFor(object subscriber, ID? id = null)
        {
            if (subscriber == null)
            {
                UnityEngine.Debug.LogError($"[NotificationHub.NumberOfSubscriptionsFor] Error: subscriber is null.");
                return 0;
            }

            if (id != null)
            {
                if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
                {
                    return 0;
                }

                return subscriptions
                    .Where(subscription => subscription.subscriber == subscriber)
                    .Count();
            } else
            {
                if (!_subscriberToID.TryGetValue(subscriber, out HashSet<string> ids))
                {
                    return 0;
                }

                int count = 0;
                foreach (string _id in ids)
                {
                    count += NumberOfSubscriptionsFor(subscriber, _id);
                }

                return count;
            }
        }

        readonly object _notifyingCountLockObject = new object();
        int notifyingCount;
        bool isNotifying => notifyingCount > 0;

        Queue<Subscription> waitingSubscriptionsToBeAdded = new();
        Queue<Rejection> waitingRejectionsToBeRemoved = new();

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

            // Create a subscription entry.
            Subscription subscription = new()
            {
                id = id,
                subscriber = subscriber,
                action = action,
                publishersFilter = publishersFilter
            };

            lock (_lockObject)
            {
                _Subscribe(subscription);
            }
        }

        void _Subscribe(Subscription subscription)
        {
            // Check if subscriptions already exist for that 'id'.
            if (_subscriptions.TryGetValue(subscription.id, out List<Subscription> subscriptions))
            {
                // If subscriptions already exist then check if a subscription already exist for that 'subscriber'.
                int index = subscriptions.FindIndex(sub => sub.subscriber == subscription.subscriber);
                if (index >= 0)
                {
                    subscriptions[index] = subscription;
                }

                // Else add this subscription.
                subscriptions.Add(subscription);
            }
            else
            {
                // If no subscriptions exist for that 'id' create a new association 'id' -> subscriptions.
                _subscriptions.Add(subscription.id, new List<Subscription>() { subscription });
            }

            // Check if this 'subscriber' already listen to notifications.
            if (_subscriberToID.TryGetValue(subscription.subscriber, out HashSet<string> ids))
            {
                // Add the 'id' to the list of listen ids, if the list didn't contain this 'id' already.
                // This list is a set, that means there is no duplicate ids.
                ids.Add(subscription.id);
            }
            else
            {
                // If that 'subscriber' listen to no one, create a new association 'subscriber' -> ids.
                _subscriberToID.Add(subscription.subscriber, new HashSet<string>() { subscription.id });
            }
        }

        public void Unsubscribe(object subscriber, ID? id = null)
        {
            if (subscriber == null)
            {
                UnityEngine.Debug.LogError($"[NotificationHub.Unsubscribe] Error: subscriber is null.");
                return;
            }

            Rejection rejection = new(subscriber, id);

            lock (_lockObject)
            {
                _Unsubscribe(rejection);
            }
        }

        void _Unsubscribe(Rejection rejection)
        {
            // Check if that 'subscriber' listen to any notifications.
            if (!_subscriberToID.TryGetValue(rejection.subscriber, out HashSet<string> ids))
            {
                UnityEngine.Debug.LogWarning($"[NotificationHub.Unsubscribe] Warning: no subscription for '{rejection.subscriberName}'.");
                // If subscriber is not listening to notification then return;
                return;
            }

            if (rejection.id == null)
            {
                // Remove all the subscription for that 'subscriber'.
                // Loop through all the ids that this 'subscriber' is listening to.
                foreach (string _id in ids)
                {
                    RemoveIdForSubscriber(_id, rejection.subscriber);
                }

                // Clear the ids.
                ids.Clear();

                _subscriberToID.Remove(rejection.subscriber);
            } else
            {
                if (!ids.Contains(rejection.id))
                {
                    return;
                }
                RemoveIdForSubscriber(rejection.id, rejection.subscriber);

                // Remove this 'id' from the list of listen ids.
                ids.Remove(rejection.id);

                // If there is not more ids then remove 'subscriber' from '_subscriberToID'.
                if (ids.Count == 0)
                {
                    _subscriberToID.Remove(rejection.subscriber);
                }
            }
        }

        bool RemoveIdForSubscriber(string id, object subscriber)
        {
            // Check if subscriptions exist for 'id'.
            if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                string subscriberName = subscriber is string
                   ? subscriber as string
                   : subscriber.GetType().FullName;

                UnityEngine.Debug.LogError($"[NotificationHub] Error: no id '{id}' for subscriber '{subscriberName}'.");
                return false;
            }

            // Remove all the subscriptions concerning 'subscriber'.
            subscriptions.RemoveAll(sub => sub.subscriber == subscriber);

            // Remove id from the subscriptions if there is no more subscribers.
            if (subscriptions.Count == 0)
            {
                _subscriptions.Remove(id);
            }

            return true;
        }

        public int Notify(
            object publisher,
            ID id,
            Dictionary<string, object> info = null,
            INotificationFilter subscribersFilter = null
        )
        {
            if (publisher == null)
            {
                UnityEngine.Debug.LogWarning($"[NotificationHub.Notify] Warning: publisher is null. A null publisher makes debugging difficult.");
            }

            if (string.IsNullOrEmpty(id))
            {
                string publisherName = publisher is string
                ? publisher as string
                : (publisher?.GetType().FullName ?? "Unknown");

                UnityEngine.Debug.LogError($"[NotificationHub.Notify] Error: id is null or empty when publisher '{publisherName}' try to notify.");
                return 0;
            }

            // Check if there are subscription for that 'id'.
            if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                string publisherName = publisher is string
                ? publisher as string
                : (publisher?.GetType().FullName ?? "Unknown");

                UnityEngine.Debug.LogWarning($"[NotificationHub.Notify] Warning: '{publisherName}' try to notify with id '{id}' but no one is listening.");
                return 0;
            }

            List<Subscription> subscriptionsCopy;
            // To be thread safe.
            lock (_lockObject)
            {
                subscriptionsCopy = new List<Subscription>(subscriptions);
            }

            // Create the notification.
            Notification notification = new Notification(id, publisher, info);
            int observers = 0;
            for (int i = 0; i < subscriptionsCopy.Count; i++)
            {
                Subscription subscription = subscriptionsCopy[i];
                // filter the notification by subscribers and publishers.
                if (IsNotificationAccepted(subscribersFilter, subscription, publisher))
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

            return observers;
        }

        bool IsNotificationAccepted(
            INotificationFilter subscribersFilter, 
            Subscription subscription, 
            object publisher
        )
        {
            bool canSendToSubscriber = subscribersFilter?.IsAccepted(subscription.subscriber) ?? true;
            bool canSubscriberReceiveFromSubscriber = subscription.publishersFilter?.IsAccepted(publisher) ?? true;

            return canSendToSubscriber && canSubscriberReceiveFromSubscriber;
        }

        public Notifier GetNotifier(
            object publisher,
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
            /// Id of the notification.
            /// </summary>
            public string id;

            /// <summary>
            /// The object that wait for a notification. If subscriber is static then user typeof().FullName.
            /// </summary>
            public object subscriber;

            /// <summary>
            /// Action to execute when the notification is received.
            /// </summary>
            public Action<Notification> action;

            /// <summary>
            /// Only the notifications that pass this filter test can be sent to this <see cref="subscriber"/>.<br/>
            /// <br/>
            /// If null the <see cref="Subscriber"/> listen to everyone.
            /// </summary>
            public INotificationFilter publishersFilter;
        }

        struct Rejection
        {
            /// <summary>
            /// The object that wait for a notification. If subscriber is static then user typeof().FullName.
            /// </summary>
            public object subscriber;

            /// <summary>
            /// Id of the notification.
            /// </summary>
            public ID? id;

            /// <summary>
            /// The descriptive name of the subscriber to display in log.
            /// </summary>
            public string subscriberName;

            public Rejection(object subscriber, ID? id)
            {
                this.subscriber = subscriber;
                this.id = id;
                subscriberName = subscriber is string
                    ? subscriber as string
                    : subscriber.GetType().FullName;
            }
        }

        [Conditional("UNITY_EDITOR")]
        public void Clear()
        {
            _subscriberToID.Clear();
            _subscriptions.Clear();
        }
    }
}