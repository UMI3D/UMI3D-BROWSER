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
    }

    public class GetIdsForTest
    {
        [Test]
        public void GivenIdNull_WhenGettingIdsForSubscriber_ThenLogError()
        {
            IEnumerable<string> ids = NotificationHub.Default.GetIdsFor(null);

            LogAssert.Expect(LogType.Error, "[NotificationHub.GetIdsFor] Error: subscriber is null.");
            Assert.NotNull(ids);
            Assert.AreEqual(ids.Count(), 0);
        }

        [Test]
        public void GivenIdAndASubscriptions_WhenGettingIdsForSubscriber_ThenSubscribers()
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
        public void GivenSubscriberAndId_WhenSubscribing_ThenLogError()
        {
            object subscriber = this;
            ID id = fixture.Create<string>();

            NotificationHub.Default.Subscribe(subscriber, id, (Callback)(() => { }));


        }
    }


    

}
