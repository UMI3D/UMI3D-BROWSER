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

namespace inetum.unityUtils
{
    public class NotificationHub 
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
        Dictionary<Object, HashSet<string>> _subscriberToID = new();

        List<Subscription> GetFilteredSubscriptions(Object subscriber, string id, Object[] publishers)
        {
            throw new System.NotImplementedException();
            //return _subscriptions.FindAll(
            //    sub => {
            //        return true;
            //    }
            //);
        }

        /// <summary>
        /// Add an entry to notify the <paramref name="subscriber"/> by calling the 
        /// <paramref name="action"/> when the publisher send a notification.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="id"></param>
        /// <param name="publishers"></param>
        /// <param name="action"></param>
        public void Subscribe(Object subscriber, string id, Object[] publishers, Action<Notification> action)
        {
            // Create a subscription entry.
            Subscription subscription = new() {
                Subscriber = subscriber,
                Publishers = publishers,
                Action = action
            };

            // Check if subscriptions already exist for that 'id'.
            if (_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                // Add the subscription to the list of subscriptions for that 'id'.
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

        /// <summary>
        /// Remove all entries concerning a specific <paramref name="subscriber"/>.
        /// </summary>
        /// <param name="subscriber"></param>
        public void Unsubscribe(Object subscriber)
        {
            // Check if that 'subscriber' listen to any notification.
            if (!_subscriberToID.TryGetValue(subscriber, out HashSet<string> ids))
            {
                // If subscriber is not listening to notification then return;
                return;
            }

            // Loop through all the ids that this 'subscriber' is listening to.
            foreach (string id in ids)
            {
                // Check if subscriptions exist for 'id'.
                if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
                {
                    UnityEngine.Debug.LogError($"[{nameof(Unsubscribe)}] Try remove subscriptions for {subscriber.GetType()}. No subscription for {id}, that should not happen.");
                    continue;
                }

                // Remove all the subscriptions concerning 'subscriber'.
                subscriptions.RemoveAll(sub => sub.Subscriber == subscriber);

                // Remove id from the subscriptions if there is no more subscribers.
                if (subscriptions.Count == 0)
                {
                    _subscriptions.Remove(id);
                }
            }

            // Remove 'subscriber' from '_subscriberToID'.
            _subscriberToID.Remove(subscriber);
        }

        /// <summary>
        /// Remove matching entries concerning a specific <paramref name="subscriber"/> and <paramref name="id"/>.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="id"></param>
        public void Unsubscribe(Object subscriber, string id)
        {
            // Check if that 'subscriber' listen to any notification.
            if (!_subscriberToID.TryGetValue(subscriber, out HashSet<string> ids))
            {
                // If subscriber is not listening to notifications then return;
                return;
            }

            // Check if 'subscriber' listen to 'id'.
            if (!ids.Contains(id))
            {
                // If subscriber is not listening to 'id' then return;
                return;
            }

            ids.Remove(id);

            // Check if subscriptions exist for 'id'.
            if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                UnityEngine.Debug.LogError($"[{nameof(Unsubscribe)}] Try remove subscriptions for {subscriber.GetType()}. No subscription for {id}, that should not happen.");
                return;
            }

            // Remove all the subscriptions concerning 'subscriber'.
            subscriptions.RemoveAll(sub => sub.Subscriber == subscriber);

            // Remove id from the subscriptions if there is no more subscribers.
            if (subscriptions.Count == 0)
            {
                _subscriptions.Remove(id);
            }
        }

        /// <summary>
        /// Send a notification to all the concerning subscribers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="publisher"></param>
        /// <param name="info"></param>
        public void Notify(string id, Object publisher, Dictionary<string, Object> info = null)
        {
            // Check if there are subscription for that 'id'.
            if (!_subscriptions.TryGetValue(id, out List<Subscription> subscriptions))
            {
                return;
            }

            // Create the notification.
            Notification notification = new Notification(id, publisher, info);

            foreach (Subscription subscription in subscriptions)
            {
                // If the subscription's subscriber is listening to this notification's publisher then send notification.
                if (subscription.Publishers == null || subscription.Publishers.Contains(publisher))
                {
                    subscription.Action(notification);
                }
            }
        }

        /// <summary>
        /// Class representing a subscription to a notification.
        /// </summary>
        private class Subscription
        {
            /// <summary>
            /// The object that wait for a notification.
            /// </summary>
            public Object Subscriber;

            /// <summary>
            /// The objects that can send the notification to the <see cref="Subscriber"/>.<br/>
            /// <br/>
            /// If the list is empty the <see cref="Subscriber"/> listen to everyone.
            /// </summary>
            public Object[] Publishers;

            /// <summary>
            /// Action to execute when the notification is received.
            /// </summary>
            public Action<Notification> Action;
        }
    }
}