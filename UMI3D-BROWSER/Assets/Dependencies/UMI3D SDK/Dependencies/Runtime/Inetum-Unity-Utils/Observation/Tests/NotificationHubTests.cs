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
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using inetum.unityUtils.observation;
using AutoFixture;
using System.Linq;

public class NotificationHubTests
{
    public class GetSubscribersForTest
    {
        class FooClass { }

        [TearDown]
        public void TearDown()
        {
            NotificationHub.Default.Clear();
        }

        [Test]
        public void GivenIdNull_WhenGettingSubscribersForId_ThenLogError()
        {
            IEnumerable<object> subscribers1 = NotificationHub.Default.GetSubscribersFor(null);
            IEnumerable<object> subscribers2 = NotificationHub.Default.GetSubscribersFor("");

            LogAssert.Expect(LogType.Error, "[NotificationHub.GetSubscribersFor] Error: id is null or empty.");
            Assert.NotNull(subscribers1);
            Assert.AreEqual(subscribers1.Count(), 0);
            LogAssert.Expect(LogType.Error, "[NotificationHub.GetSubscribersFor] Error: id is null or empty.");
            Assert.NotNull(subscribers2);
            Assert.AreEqual(subscribers2.Count(), 0);
        }

        [Test]
        public void GivenIdAndASubscriptions_WhenGettingSubscribersForId_ThenSubscribers()
        {
            Fixture fixture = new();
            object subscriber1 = this;
            object subscriber2 = new FooClass();
            ID id = fixture.Create<string>();
            NotificationHub.Default.Subscribe(subscriber1, id, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber2, id, (Callback)(() => { }));

            IEnumerable<object> subscribers = NotificationHub.Default.GetSubscribersFor(id);

            Assert.NotNull(subscribers);
            Assert.AreEqual(subscribers.Count(), 2);
            IEnumerator<object> subscribersEnumerator = subscribers.GetEnumerator();
            Assert.True(subscribersEnumerator.MoveNext());
            Assert.AreEqual(subscribersEnumerator.Current, subscriber1);
            Assert.True(subscribersEnumerator.MoveNext());
            Assert.AreEqual(subscribersEnumerator.Current, subscriber2);
        }

        [Test]
        public void GivenTwoSubscriptionsWithSameIDAndSubscriber_WhenGettingSubscribersForId_ThenOneSubscriber()
        {
            Fixture fixture = new();
            object subscriber = this;
            ID id = fixture.Create<string>();

            NotificationHub.Default.Subscribe(subscriber, id, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber, id, (Callback)(() => { }));

            IEnumerable<object> subscribers = NotificationHub.Default.GetSubscribersFor(id);
            IEnumerator<object> subscribersEnumerator = subscribers.GetEnumerator();
            Assert.AreEqual(1, subscribers.Count());
            Assert.True(subscribersEnumerator.MoveNext());
            Assert.AreEqual(subscribersEnumerator.Current, subscriber);
        }
    }

    public class GetIdsForTest
    {
        [TearDown]
        public void TearDown()
        {
            NotificationHub.Default.Clear();
        }

        [Test]
        public void GivenIdNull_WhenGettingIdsForSubscriber_ThenLogError()
        {
            IEnumerable<string> ids = NotificationHub.Default.GetIdsFor(null);

            LogAssert.Expect(LogType.Error, "[NotificationHub.GetIdsFor] Error: subscriber is null.");
            Assert.NotNull(ids);
            Assert.AreEqual(ids.Count(), 0);
        }

        [Test]
        public void GivenIdAndASubscriptions_WhenGettingIdsForSubscriber_ThenIds()
        {
            Fixture fixture = new();
            object subscriber = this;
            ID id1 = fixture.Create<string>();
            ID id2 = fixture.Create<string>();
            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber, id2, (Callback)(() => { }));

            IEnumerable<string> ids = NotificationHub.Default.GetIdsFor(subscriber);

            Assert.NotNull(ids);
            Assert.AreEqual(ids.Count(), 2);
            IEnumerator<object> idsEnumerator = ids.GetEnumerator();
            Assert.True(idsEnumerator.MoveNext());
            Assert.AreEqual(idsEnumerator.Current, id1.id);
            Assert.True(idsEnumerator.MoveNext());
            Assert.AreEqual(idsEnumerator.Current, id2.id);
        }

        [Test]
        public void GivenTwoSubscriptionsWithSameIDAndSubscriber_WhenGettingIdsForSubscriber_ThenOneId()
        {
            Fixture fixture = new();
            object subscriber = this;
            ID id = fixture.Create<string>();

            NotificationHub.Default.Subscribe(subscriber, id, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber, id, (Callback)(() => { }));

            IEnumerable<string> ids = NotificationHub.Default.GetIdsFor(subscriber);
            IEnumerator<string> idsEnumerator = ids.GetEnumerator();
            Assert.AreEqual(1, ids.Count());
            Assert.True(idsEnumerator.MoveNext());
            Assert.AreEqual(idsEnumerator.Current, id.id);
        }
    }

    public class NumberOfSubscriptionsForTest
    {
        [TearDown]
        public void TearDown()
        {
            NotificationHub.Default.Clear();
        }

        [Test]
        public void GivenNoSubscriptionAndSubscriberOrIdNull_WhenGettingTheNumberOfSubscriptionForASubscriber_ThenLogError()
        {
            Fixture fixture = new Fixture();
            string id = fixture.Create<string>();

            int count1 = NotificationHub.Default.NumberOfSubscriptionsFor(null);
            int count2 = NotificationHub.Default.NumberOfSubscriptionsFor(null, id);
            int count3 = NotificationHub.Default.NumberOfSubscriptionsFor(this);
            int count4 = NotificationHub.Default.NumberOfSubscriptionsFor(this, id);

            LogAssert.Expect(LogType.Error, "[NotificationHub.NumberOfSubscriptionsFor] Error: subscriber is null.");
            LogAssert.Expect(LogType.Error, "[NotificationHub.NumberOfSubscriptionsFor] Error: subscriber is null.");
            Assert.AreEqual(0, count1);
            Assert.AreEqual(0, count2);
            Assert.AreEqual(0, count3);
            Assert.AreEqual(0, count4);
        }

        [Test]
        public void GivenTwoSubscriptionsForOneSubscriber_WhenGettingTheNumberOfSubscriptionForASubscriber_Then2Subscriptions()
        {
            Fixture fixture = new();
            object subscriber = this;
            ID id1 = fixture.Create<string>();
            ID id2 = fixture.Create<string>();
            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber, id2, (Callback)(() => { }));

            int count1 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber);
            int count2 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber, id1);

            Assert.AreEqual(2, count1);
            Assert.AreEqual(1, count2);
        }
    }

    public class SubscribeTest
    {
        Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            fixture = null;
            NotificationHub.Default.Clear();
        }

        [Test]
        public void GivenIdNull_WhenSubscribing_ThenLogError()
        {
            NotificationHub.Default.Subscribe(null, null, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(this, null, (Callback)(() => { }));

            LogAssert.Expect(LogType.Error, "[NotificationHub.Subscribe] Error: id is null or empty.");
            LogAssert.Expect(LogType.Error, "[NotificationHub.Subscribe] Error: id is null or empty.");
        }

        [Test]
        public void GivenSubscriberNull_WhenSubscribing_ThenLogError()
        {
            ID id = fixture.Create<string>();

            NotificationHub.Default.Subscribe(null, id, (Callback)(() => { }));

            LogAssert.Expect(LogType.Error, $"[NotificationHub.Subscribe] Error: subscriber is null for id '{id}'.");
        }

        [Test]
        public void GivenSubscriberAndId_WhenSubscribing_ThenASubscriptionHasBeenAdded()
        {
            object subscriber = this;
            ID id = fixture.Create<string>();

            NotificationHub.Default.Subscribe(subscriber, id, (Callback)(() => { }));

            int count = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber, id);
            Assert.AreEqual(1, count);
            IEnumerable<object> subscribers = NotificationHub.Default.GetSubscribersFor(id);
            IEnumerator<object> subscribersEnumerator = subscribers.GetEnumerator();
            IEnumerable<string> ids = NotificationHub.Default.GetIdsFor(subscriber);
            IEnumerator<string> idsEnumerator = ids.GetEnumerator();
            Assert.AreEqual(1, subscribers.Count());
            Assert.True(subscribersEnumerator.MoveNext());
            Assert.AreEqual(subscribersEnumerator.Current, subscriber);
            Assert.AreEqual(1, ids.Count());
            Assert.True(idsEnumerator.MoveNext());
            Assert.AreEqual(idsEnumerator.Current, id.id);
        }

        [Test]
        public void GivenSubscriberAndId_WhenSubscribingThreeTimes_ThenThreeSubscriptionsHaveBeenAdded()
        {
            object subscriber = this;
            ID id1 = fixture.Create<string>();
            ID id2 = fixture.Create<string>();

            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber, id2, (Callback)(() => { }));

            int count1 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber, id1);
            Assert.AreEqual(2, count1);
            int count2 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber);
            Assert.AreEqual(3, count2);
        }
    }

    public class UnsubscribeTest
    {
        class FooClass { }

        Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            fixture = null;
            NotificationHub.Default.Clear();
        }

        [Test]
        public void GivenNoSubscriptionAndNullSubscriber_WhenUnsubscribing_ThenLogError()
        {
            ID id = fixture.Create<string>();

            NotificationHub.Default.Unsubscribe(null);
            NotificationHub.Default.Unsubscribe(null, id);

            LogAssert.Expect(LogType.Error, "[NotificationHub.Unsubscribe] Error: subscriber is null.");
            LogAssert.Expect(LogType.Error, "[NotificationHub.Unsubscribe] Error: subscriber is null.");
        }

        [Test]
        public void GivenNoSubscription_WhenUnsubscribing_ThenLogWarning()
        {
            ID id = fixture.Create<string>();

            NotificationHub.Default.Unsubscribe(this);
            NotificationHub.Default.Unsubscribe(this, id);

            LogAssert.Expect(LogType.Warning, $"[NotificationHub.Unsubscribe] Warning: no subscription for '{this.GetType().FullName}'.");
            LogAssert.Expect(LogType.Warning, $"[NotificationHub.Unsubscribe] Warning: no subscription for '{this.GetType().FullName}'.");
        }

        [Test]
        public void GivenSubscription_WhenUnsubscribing_ThenSubscriptionIsRemoved()
        {
            object subscriber1 = this;
            object subscriber2 = new FooClass();
            ID id1 = fixture.Create<string>();
            ID id2 = fixture.Create<string>();
            NotificationHub.Default.Subscribe(subscriber1, id1, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber1, id2, (Callback)(() => { }));
            NotificationHub.Default.Subscribe(subscriber2, id1, (Callback)(() => { }));

            // First test ---- Start ----.
            NotificationHub.Default.Unsubscribe(subscriber2);

            int count1 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber2);
            Assert.AreEqual(0, count1);
            // First test ---- End ----.

            // Second test ---- Start ----.
            NotificationHub.Default.Unsubscribe(subscriber1, id1);

            int count2 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber1);
            Assert.AreEqual(1, count2);
            string id = NotificationHub.Default.GetIdsFor(subscriber1).First();
            Assert.AreEqual(id2, id);
            // Second test ---- End ----.
        }
    }

    public class NotifyTest
    {
        class FooClass { }

        Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            fixture = null;
            NotificationHub.Default.Clear();
        }

        [Test]
        public void GivenNoSubscriptionAndNullPublisherOrId_WhenNotifying_ThenLogErrorAndWarning()
        {
            ID id = fixture.Create<string>();

            // First test ---- Start ----.
            NotificationHub.Default.Notify(null, null);

            LogAssert.Expect(LogType.Warning, "[NotificationHub.Notify] Warning: publisher is null. A null publisher makes debugging difficult.");
            LogAssert.Expect(LogType.Error, $"[NotificationHub.Notify] Error: id is null or empty when publisher 'Unknown' try to notify.");
            // First test ---- End ----.

            // Second test ---- Start ----.
            NotificationHub.Default.Notify(this, null);

            LogAssert.Expect(LogType.Error, $"[NotificationHub.Notify] Error: id is null or empty when publisher '{this.GetType().FullName}' try to notify.");
            // Second test ---- End ----.

            // Third test ---- Start ----.
            NotificationHub.Default.Notify(null, id);

            LogAssert.Expect(LogType.Warning, "[NotificationHub.Notify] Warning: publisher is null. A null publisher makes debugging difficult.");
            LogAssert.Expect(LogType.Warning, $"[NotificationHub.Notify] Warning: 'Unknown' try to notify with id '{id}' but no one is listening.");
            // Third test ---- End ----.

            // Fourth test ---- Start ----.
            NotificationHub.Default.Notify(this, id);

            LogAssert.Expect(LogType.Warning, $"[NotificationHub.Notify] Warning: '{this.GetType().FullName}' try to notify with id '{id}' but no one is listening.");
            // Fourth test ---- End ----.
        }

        [Test]
        public void GivenSubscription_WhenNotifyingForAnotherID_ThenLogWarning()
        {
            object subscriber1 = this;
            ID id1 = fixture.Create<string>();
            ID id2 = fixture.Create<string>();
            NotificationHub.Default.Subscribe(subscriber1, id1, (Callback)(() => { }));

            NotificationHub.Default.Notify(subscriber1, id2);
            object publisher = new FooClass();
            NotificationHub.Default.Notify(publisher, id2);

            LogAssert.Expect(LogType.Warning, $"[NotificationHub.Notify] Warning: '{subscriber1.GetType().FullName}' try to notify with id '{id2}' but no one is listening.");
            LogAssert.Expect(LogType.Warning, $"[NotificationHub.Notify] Warning: '{publisher.GetType().FullName}' try to notify with id '{id2}' but no one is listening.");
        }

        [Test]
        public void GivenTwoSubscriptionsAndASubscriptionInAnotherSubscription_WhenNotifying_ThenOnlyTwoCallbackAndANewSubscription()
        {
            object subscriber = this;
            object publisher = new FooClass();
            ID id1 = fixture.Create<string>();
            ID id2 = fixture.Create<string>();
            int count = 0;
            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() =>
            {
                count++;
                NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => 
                {
                    Assert.Fail();
                }));
            }));
            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() => 
            {
                count++;
            }));
            int numberOfSubscriptions = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber, id1);
            Assert.AreEqual(2, numberOfSubscriptions);

            NotificationHub.Default.Notify(publisher, id1);

            Assert.AreEqual(2, count);
            numberOfSubscriptions = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber);
            Assert.AreEqual(3, numberOfSubscriptions);
        }

        [Test]
        public void GivenTwoSubscriptionsAndASubscriptionInAnotherSubscription_WhenNotifyingTwoTimes_ThenThreeCallbackAndTwoNewSubscriptions()
        {
            object subscriber = this;
            object publisher = new FooClass();
            ID id1 = fixture.Create<string>();
            ID id2 = fixture.Create<string>();
            int count = 0;
            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() =>
            {
                count++;
                NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() =>
                {
                    count++;
                }));
            }));
            NotificationHub.Default.Subscribe(subscriber, id1, (Callback)(() =>
            {
                count++;
            }));

            NotificationHub.Default.Notify(publisher, id1);
            count = 0;
            NotificationHub.Default.Notify(publisher, id1);

            Assert.AreEqual(3, count);
            int numberOfSubscriptions = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber);
            Assert.AreEqual(4, numberOfSubscriptions);
        }

        [Test]
        public void GivenTwoSubscriptionsAndARejectionInOneSubscription_WhenNotifyingTwoTimes_ThenAtFirstTwoCallbackAndAtLastOneCallback()
        {
            object subscriber1 = this;
            object subscriber2 = new FooClass();
            object publisher = new FooClass();
            ID id1 = fixture.Create<string>();
            int count = 0;
            NotificationHub.Default.Subscribe(subscriber1, id1, (Callback)(() =>
            {
                count++;
                NotificationHub.Default.Unsubscribe(subscriber1);
            }));
            NotificationHub.Default.Subscribe(subscriber2, id1, (Callback)(() =>
            {
                count++;
            }));

            // Test 1 ---- Start ----
            NotificationHub.Default.Notify(publisher, id1);

            Assert.AreEqual(2, count);
            // Test 1 ---- End ----

            // Test 2 ---- Start ----
            count = 0;
            NotificationHub.Default.Notify(publisher, id1);

            Assert.AreEqual(1, count);
            // Test 2 ---- End ----
        }

        //[Test]
        //public void GivenSubscription_WhenUnsubscribing_ThenSubscriptionIsRemoved()
        //{
        //    object subscriber1 = this;
        //    object subscriber2 = new FooClass();
        //    ID id1 = fixture.Create<string>();
        //    ID id2 = fixture.Create<string>();
        //    NotificationHub.Default.Subscribe(subscriber1, id1, (Callback)(() => { }));
        //    NotificationHub.Default.Subscribe(subscriber1, id2, (Callback)(() => { }));
        //    NotificationHub.Default.Subscribe(subscriber2, id1, (Callback)(() => { }));

        //    // First test ---- Start ----.
        //    NotificationHub.Default.Notify(subscriber2);

        //    int count1 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber2);
        //    Assert.AreEqual(0, count1);
        //    // First test ---- End ----.

        //    // Second test ---- Start ----.
        //    NotificationHub.Default.Unsubscribe(subscriber1, id1);

        //    int count2 = NotificationHub.Default.NumberOfSubscriptionsFor(subscriber1);
        //    Assert.AreEqual(1, count2);
        //    string id = NotificationHub.Default.GetIdsFor(subscriber1).First();
        //    Assert.AreEqual(id2, id);
        //    // Second test ---- End ----.
        //}
    }
}
