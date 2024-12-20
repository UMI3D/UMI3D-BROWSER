using System.Collections;
using System.Collections.Generic;
using inetum.unityUtils.observation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NotificationTests
{
    public class ConstructorTest
    {
        [Test]
        public void GivenNullNullNull_WhenConstructor_ThenLogError()
        {
            string id = null;
            object publisher = null;
            Dictionary<string, object> info = null;

            LogAssert.Expect(LogType.Error, $"[Notification.Notification] Error: create a new notification with id null or empty.");
            string message = $"[Notification.Notification] Error: create a new notification with a null publisher.\n" +
                    $"Having a null publisher is a bad practice because it increase complexity while debugging.";
            LogAssert.Expect(LogType.Error, message);
            Notification notification = new(id, publisher, info);

            Assert.AreEqual(notification.ID, id);
            Assert.AreEqual(notification.Publisher, publisher);
            Assert.AreEqual(notification.Info, info);
        }

        [Test]
        public void GivenEmptyNullNull_WhenConstructor_ThenLogError()
        {
            string id = "";
            object publisher = null;
            Dictionary<string, object> info = null;

            LogAssert.Expect(LogType.Error, $"[Notification.Notification] Error: create a new notification with id null or empty.");
            string message = $"[Notification.Notification] Error: create a new notification with a null publisher.\n" +
                    $"Having a null publisher is a bad practice because it increase complexity while debugging.";
            LogAssert.Expect(LogType.Error, message);
            Notification notification = new(id, publisher, info);

            Assert.AreEqual(notification.ID, id);
            Assert.AreEqual(notification.Publisher, publisher);
            Assert.AreEqual(notification.Info, info);
        }

        [Test]
        public void GivenNullPublisherNull_WhenConstructor_ThenLogError()
        {
            string id = null;
            object publisher = this;
            Dictionary<string, object> info = null;

            LogAssert.Expect(LogType.Error, $"[Notification.Notification] Error: create a new notification with id null or empty.");
            Notification notification = new(id, publisher, info);

            Assert.AreEqual(notification.ID, id);
            Assert.AreEqual(notification.Publisher, publisher);
            Assert.AreEqual(notification.Info, info);
        }

        [Test]
        public void GivenEmptyPublisherNull_WhenConstructor_ThenLogError()
        {
            string id = "";
            object publisher = this;
            Dictionary<string, object> info = null;

            LogAssert.Expect(LogType.Error, $"[Notification.Notification] Error: create a new notification with id null or empty.");
            Notification notification = new(id, publisher, info);

            Assert.AreEqual(notification.ID, id);
            Assert.AreEqual(notification.Publisher, publisher);
            Assert.AreEqual(notification.Info, info);
        }

        [Test]
        public void GivenIdNullNull_WhenConstructor_ThenLogError()
        {
            string id = "My new id";
            object publisher = null;
            Dictionary<string, object> info = null;

            string message = $"[Notification.Notification] Error: create a new notification with a null publisher.\n" +
                    $"Having a null publisher is a bad practice because it increase complexity while debugging.";
            LogAssert.Expect(LogType.Error, message);
            Notification notification = new(id, publisher, info);

            Assert.AreEqual(notification.ID, id);
            Assert.AreEqual(notification.Publisher, publisher);
            Assert.AreEqual(notification.Info, info);
        }

        [Test]
        public void GivenIdPublisherNull_WhenConstructor_ThenGoodNotification()
        {
            string id = "My new id";
            object publisher = this;
            Dictionary<string, object> info = null;

            Notification notification = new(id, publisher, info);

            Assert.AreEqual(notification.ID, id);
            Assert.AreEqual(notification.Publisher, publisher);
            Assert.AreEqual(notification.Info, info);
        }

        [Test]
        public void GivenIdPublisherInfo_WhenConstructor_ThenGoodNotification()
        {
            string id = "My new id";
            object publisher = this;
            Dictionary<string, object> info = new() { { "An info", true } };

            Notification notification = new(id, publisher, info);

            Assert.AreEqual(notification.ID, id);
            Assert.AreEqual(notification.Publisher, publisher);
            Assert.AreEqual(notification.Info, info);
        }
    }

    public class TryGetInfoTest
    {
        [Test]
        public void GivenNotification_WhenTryGetInfoNullKey_ThenLogErrorAndNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            LogAssert.Expect(LogType.Error, $"[Notification.TryGetInfo] Error: key is null for notification 'id'.");
            bool result1 = notificationNullInfo.TryGetInfo(null, out object info1);
            LogAssert.Expect(LogType.Error, $"[Notification.TryGetInfo] Error: key is null for notification 'id'.");
            bool result2 = notificationEmptyInfo.TryGetInfo(null, out object info2);
            LogAssert.Expect(LogType.Error, $"[Notification.TryGetInfo] Error: key is null for notification 'id'.");
            bool result3 = notificationInfo.TryGetInfo(null, out object info3);

            //TODO
            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoKeyThatDoesNotExist_ThenLogErrorAndNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            string errorMessage = $"[Notification.TryGetInfo] Error: key 'Key that does not exist' not found for notification 'id'.\n" +
                        $"Reason: info is null.";
            LogAssert.Expect(LogType.Error, errorMessage);
            bool result1 = notificationNullInfo.TryGetInfo("Key that does not exist", out object info1);

            errorMessage = $"[Notification.TryGetInfo] Error: key 'Key that does not exist' not found for notification 'id'.\n" +
                        $"Reason: info is empty.";
            LogAssert.Expect(LogType.Error, errorMessage);
            bool result2 = notificationEmptyInfo.TryGetInfo("Key that does not exist", out object info2);

            errorMessage = $"[Notification.TryGetInfo] Error: key 'Key that does not exist' not found for notification 'id'.\n" +
                        $"Reason: info does not contain 'Key that does not exist'.";
            LogAssert.Expect(LogType.Error, errorMessage);
            bool result3 = notificationInfo.TryGetInfo("Key that does not exist", out object info3);

            //TODO
            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoNullKeyNoLogError_ThenNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            bool result1 = notificationNullInfo.TryGetInfo(null, out object info1, false);
            bool result2 = notificationEmptyInfo.TryGetInfo(null, out object info2, false);
            bool result3 = notificationInfo.TryGetInfo(null, out object info3, false);

            //TODO
            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoKeyThatDoesNotExistNoLogError_ThenNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            bool result1 = notificationNullInfo.TryGetInfo("Key that does not exist", out object info1, false);
            bool result2 = notificationEmptyInfo.TryGetInfo("Key that does not exist", out object info2, false);
            bool result3 = notificationInfo.TryGetInfo("Key that does not exist", out object info3, false);

            //TODO
            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoWithKeyThatExists_ThenValue()
        {
            bool value = true;
            Notification notification = new("id", this, new() { { "An info", value } });

            bool result = notification.TryGetInfo("An info", out object info);

            //TODO
            Assert.AreEqual(info, value);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoWithValueNull_ThenValueNull()
        {
            string value = null;
            Notification notification = new("id", this, new() { { "An info", value } });

            bool result = notification.TryGetInfo("An info", out object info);

            //TODO
            Assert.AreEqual(info, value);
        }
    }

    public class TryGetInfoTTest
    {
        [Test]
        public void GivenNotification_WhenTryGetInfoTNullKey_ThenLogErrorAndNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            LogAssert.Expect(LogType.Error, $"[Notification.TryGetInfo] Error: key is null for notification 'id'.");
            notificationNullInfo.TryGetInfoT(null, out object info1);
            LogAssert.Expect(LogType.Error, $"[Notification.TryGetInfo] Error: key is null for notification 'id'.");
            notificationEmptyInfo.TryGetInfoT(null, out object info2);
            LogAssert.Expect(LogType.Error, $"[Notification.TryGetInfo] Error: key is null for notification 'id'.");
            notificationInfo.TryGetInfoT(null, out object info3);

            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoTKeyThatDoesNotExist_ThenLogErrorAndNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            string errorMessage = $"[Notification.TryGetInfo] Error: key 'Key that does not exist' not found for notification 'id'.\n" +
                        $"Reason: info is null.";
            LogAssert.Expect(LogType.Error, errorMessage);
            notificationNullInfo.TryGetInfoT("Key that does not exist", out object info1);

            errorMessage = $"[Notification.TryGetInfo] Error: key 'Key that does not exist' not found for notification 'id'.\n" +
                        $"Reason: info is empty.";
            LogAssert.Expect(LogType.Error, errorMessage);
            notificationEmptyInfo.TryGetInfoT("Key that does not exist", out object info2);

            errorMessage = $"[Notification.TryGetInfo] Error: key 'Key that does not exist' not found for notification 'id'.\n" +
                        $"Reason: info does not contain 'Key that does not exist'.";
            LogAssert.Expect(LogType.Error, errorMessage);
            notificationInfo.TryGetInfoT("Key that does not exist", out object info3);

            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoTNullKeyNoLogError_ThenNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            notificationNullInfo.TryGetInfo(null, out object info1, false);
            notificationEmptyInfo.TryGetInfo(null, out object info2, false);
            notificationInfo.TryGetInfo(null, out object info3, false);

            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoTKeyThatDoesNotExistNoLogError_ThenNull()
        {
            Notification notificationNullInfo = new("id", this, null);
            Notification notificationEmptyInfo = new("id", this, new());
            Notification notificationInfo = new("id", this, new() { { "An info", true } });

            notificationNullInfo.TryGetInfo("Key that does not exist", out object info1, false);
            notificationEmptyInfo.TryGetInfo("Key that does not exist", out object info2, false);
            notificationInfo.TryGetInfo("Key that does not exist", out object info3, false);

            Assert.Null(info1);
            Assert.Null(info2);
            Assert.Null(info3);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoTWithKeyThatExistsButWrongType_ThenNullAndLogError()
        {
            bool value = true;
            Notification notification = new("id", this, new() { { "An info", value } });

            string errorMessage = $"[Notification.TryGetInfoT] Error: notification 'id' does not contain key 'An info' of type {typeof(string)}.\n" +
                        $"Type of the object is {typeof(bool)}.";
            LogAssert.Expect(LogType.Error, errorMessage);
            notification.TryGetInfoT("An info", out string info);

            Assert.Null(info);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoTWithKeyThatExistsAndValueNullButWrongType_ThenNullAndLogError()
        {
            string value = null;
            Notification notification = new("id", this, new() { { "An info", value } });

            string errorMessage = $"[Notification.TryGetInfoT] Error: notification 'id' does not contain key 'An info' of type {typeof(bool)}.\n" +
                        $"Type of the object is {typeof(string)}.";
            LogAssert.Expect(LogType.Error, errorMessage);
            bool result = notification.TryGetInfoT("An info", out bool info);

            Assert.False(result);
            Assert.Null(info);
        }

        [Test]
        public void GivenNotification_WhenTryGetInfoTWithKeyThatExistsAndValueNull_ThenValue()
        {
            string value = null;
            Notification notification = new("id", this, new() { { "An info", value } });

            //string errorMessage = $"[Notification.TryGetInfoT] Error: notification 'id' does not contain key 'An info' of type {typeof(bool)}.\n" +
            //            $"Type of the object is {typeof(string)}.";
            //LogAssert.Expect(LogType.Error, errorMessage);
            notification.TryGetInfoT("An info", out string info);

            Assert.Null(info);
        }
    }
}
