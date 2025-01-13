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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using inetum.unityUtils.lifeCycle;
using inetum.unityUtils.observation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class QuittingTests
{
    public class QuitTest
    {
        [TearDown]
        public void TearDown()
        {
            Quitting.instance.Reset();
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.Confirmation, this);
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.QuittingStarted, this);
        }

        [Test]
        public void GivenNothing_WhenDoingNothing_ThenNotQuitting()
        {
            Assert.AreEqual(Quitting.QuittingState.NotQuitting, Quitting.instance.state);
        }

        [Test]
        public void GivenNothing_WhenQuittingWithoutAskingForConfirmation_ThenIsQuitting()
        {
            Quitting.instance.Quit(this, false);

            Assert.AreEqual(Quitting.QuittingState.IsQuitting, Quitting.instance.state);
        }

        [Test]
        public void GivenSubscription_WhenQuittingWithoutAskingForConfirmation_ThenConfirmationNotificationNotSentAndQuittingNotificationSent()
        {
            int confirmationCount = 0;
            Quitting.instance.SubscribeFor(
                Quitting.SubscriptionType.Confirmation,
                this,
                (Callback)(() => { confirmationCount++; })
            );
            int quittingCount = 0;
            Quitting.instance.SubscribeFor(
                Quitting.SubscriptionType.QuittingStarted,
                this,
                (Callback)(() => { quittingCount++; })
            );

            Quitting.instance.Quit(this, false);

            Assert.AreEqual(0, confirmationCount);
            Assert.AreEqual(1, quittingCount);
        }

        [Test]
        public void GivenNothing_WhenQuittingWithAskingForConfirmation_ThenWaitsForConfirmation()
        {
            Quitting.instance.Quit(this, true);

            Assert.AreEqual(Quitting.QuittingState.WaitsForConfirmation, Quitting.instance.state);
        }

        [Test]
        public void GivenSubscription_WhenQuittingWithAskingForConfirmation_ThenConfirmationNotificationSentAndQuittingNotificationNotSent()
        {
            Quitting.instance.Quit(this, true);
            Assert.AreEqual(Quitting.QuittingState.WaitsForConfirmation, Quitting.instance.state);

            // --- New Test ---
            int confirmationCount = 0;
            Quitting.instance.SubscribeFor(
                Quitting.SubscriptionType.Confirmation,
                this,
                (Callback)(() => { confirmationCount++; })
            );
            int quittingCount = 0;
            Quitting.instance.SubscribeFor(
                Quitting.SubscriptionType.QuittingStarted,
                this,
                (Callback)(() => { quittingCount++; })
            );
            Quitting.instance.Quit(this, true);
            Assert.AreEqual(1, confirmationCount);
            Assert.AreEqual(0, quittingCount);
        }
    }

    public class ConfirmTest
    {
        [TearDown]
        public void TearDown()
        {
            Quitting.instance.Reset();
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.Confirmation, this);
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.QuittingStarted, this);
        }

        [Test]
        public void GivenNothing_WhenAborting_ThenNotQuitting()
        {
            Quitting.instance.Confirm(this, false);
            Assert.AreEqual(Quitting.QuittingState.NotQuitting, Quitting.instance.state);
        }

        [Test]
        public void GivenNothing_WhenConfirming_ThenIsQuitting()
        {
            Quitting.instance.Confirm(this, true);
            Assert.AreEqual(Quitting.QuittingState.IsQuitting, Quitting.instance.state);
        }

        [Test]
        public void GivenQuittingWithAskingForConfirmation_WhenAborting_ThenNotQuitting()
        {
            Quitting.instance.Quit(this, true);
            Quitting.instance.Confirm(this, false);
            Assert.AreEqual(Quitting.QuittingState.NotQuitting, Quitting.instance.state);

            // --- New Test ---
            Quitting.instance.SubscribeFor(
                Quitting.SubscriptionType.Confirmation,
                this,
                (Callback)(() => { Quitting.instance.Confirm(this, false); })
            );
            Quitting.instance.Quit(this, true);
            Assert.AreEqual(Quitting.QuittingState.NotQuitting, Quitting.instance.state);
        }

        [Test]
        public void GivenQuittingWithAskingForConfirmation_WhenConfirming_ThenIsQuitting()
        {
            Quitting.instance.Quit(this, true);
            Quitting.instance.Confirm(this, true);
            Assert.AreEqual(Quitting.QuittingState.IsQuitting, Quitting.instance.state);

            // --- New Test ---
            Quitting.instance.SubscribeFor(
                Quitting.SubscriptionType.Confirmation,
                this,
                (Callback)(() => { Quitting.instance.Confirm(this, true); })
            );
            Quitting.instance.Quit(this, true);
            Assert.AreEqual(Quitting.QuittingState.IsQuitting, Quitting.instance.state);
        }
    }

    public class SubscribeForTest
    {
        [TearDown]
        public void TearDown()
        {
            Quitting.instance.Reset();
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.Confirmation, this);
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.QuittingStarted, this);
        }

        [Test]
        public void GivenNullSubscriber_WhenSubscribingFor_ThenLogError()
        {
            // --- New Test ---
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.Confirmation, null, (Callback)(() => { }));
            string id = ID.FromType<QuittingNotificationKeys.AskForConfirmation>();
            LogAssert.Expect(LogType.Error, $"[NotificationHub.Subscribe] Error: subscriber is null for id '{id}'.");

            // --- New Test ---
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.QuittingStarted, null, (Callback)(() => { }));
            id = ID.FromType<QuittingNotificationKeys.ApplicationIsQuitting>();
            LogAssert.Expect(LogType.Error, $"[NotificationHub.Subscribe] Error: subscriber is null for id '{id}'.");
        }

        [Test]
        public void GivenNothing_WhenSubscribingFor_ThenSubscriberAddedToNotificationHub()
        {
            // --- New Test ---
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.Confirmation, this, (Callback)(() => { }));
            string id = ID.FromType<QuittingNotificationKeys.AskForConfirmation>();
            IEnumerable<object> subscribers = NotificationHub.Default.GetSubscribersFor(id);
            Assert.AreEqual(1, subscribers.Count());
            IEnumerator<object> subscribersEnumerator = subscribers.GetEnumerator();
            Assert.True(subscribersEnumerator.MoveNext());
            Assert.AreEqual(this, subscribersEnumerator.Current);

            // --- New Test ---
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.QuittingStarted, this, (Callback)(() => { }));
            id = ID.FromType<QuittingNotificationKeys.ApplicationIsQuitting>();
            subscribers = NotificationHub.Default.GetSubscribersFor(id);
            Assert.AreEqual(1, subscribers.Count());
            subscribersEnumerator = subscribers.GetEnumerator();
            Assert.True(subscribersEnumerator.MoveNext());
            Assert.AreEqual(this, subscribersEnumerator.Current);
        }

        [Test]
        public void GivenSubscriptions_WhenQuitting_ThenNotificationInfoAreCorrect()
        {
            int count = 0;
            void CallbackAskForConfirmation(Notification notification)
            {
                if (!notification.TryGetInfo(QuittingNotificationKeys.AskForConfirmation.Publisher, out object publisher))
                {
                    Assert.Fail();
                }

                if (publisher != this)
                {
                    Assert.Fail();
                }

                count++;
            }

            void CallbackApplicationIsQuitting(Notification notification)
            {
                if (!notification.TryGetInfo(QuittingNotificationKeys.ApplicationIsQuitting.Publisher, out object publisher))
                {
                    Assert.Fail();
                }

                if (publisher != this)
                {
                    Assert.Fail();
                }

                count++;
            }
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.Confirmation, this, (Callback)CallbackAskForConfirmation);
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.QuittingStarted, this, (Callback)CallbackApplicationIsQuitting);

            // --- New Test ---
            Quitting.instance.Quit(this, true);
            Assert.AreEqual(1, count);

            // --- New Test ---
            Quitting.instance.Quit(this, false);
            Assert.AreEqual(2, count);
        }
    }

    public class UnsubscribeForTest
    {
        [TearDown]
        public void TearDown()
        {
            Quitting.instance.Reset();
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.Confirmation, this);
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.QuittingStarted, this);
        }

        [Test]
        public void GivenNullSubscriber_WhenUnsubscribingFor_ThenLogError()
        {
            // --- New Test ---
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.Confirmation, null);
            string id = ID.FromType<QuittingNotificationKeys.AskForConfirmation>();
            LogAssert.Expect(LogType.Error, $"[NotificationHub.Unsubscribe] Error: subscriber is null.");

            // --- New Test ---
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.QuittingStarted, null);
            id = ID.FromType<QuittingNotificationKeys.ApplicationIsQuitting>();
            LogAssert.Expect(LogType.Error, $"[NotificationHub.Unsubscribe] Error: subscriber is null.");
        }

        [Test]
        public void GivenSubscription_WhenUnsubscribingFor_ThenSubscriberRemovedFromNotificationHub()
        {
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.Confirmation, this, (Callback)(() => { }));
            Quitting.instance.SubscribeFor(Quitting.SubscriptionType.QuittingStarted, this, (Callback)(() => { }));


            // --- New Test ---
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.Confirmation, this);
            string id = ID.FromType<QuittingNotificationKeys.AskForConfirmation>();
            IEnumerable<object> subscribers = NotificationHub.Default.GetSubscribersFor(id);
            Assert.AreEqual(0, subscribers.Count());

            // --- New Test ---
            Quitting.instance.UnsubscribeFor(Quitting.SubscriptionType.QuittingStarted, this);
            id = ID.FromType<QuittingNotificationKeys.ApplicationIsQuitting>();
            subscribers = NotificationHub.Default.GetSubscribersFor(id);
            Assert.AreEqual(0, subscribers.Count());
        }
    }
}
