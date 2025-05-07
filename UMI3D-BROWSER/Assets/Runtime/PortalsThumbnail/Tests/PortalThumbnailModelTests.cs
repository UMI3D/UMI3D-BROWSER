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
using NUnit.Framework;
using umi3d.browserRuntime.portalsThumbnails;
using UnityEngine;

public class PortalThumbnailModelTests
{
    public class SetPortalTests
    {
        PortalThumbnailModelContainer _modelContainer;

        [SetUp]
        public void SetUp()
        {
            _modelContainer = new GameObject().AddComponent<PortalThumbnailModelContainer>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_modelContainer);
        }

        [Test]
        public void GivenPortal_WhenSettingPortal_ThenPortal()
        {
            var portals = new VirtualWorlds();
            var portal = new VirtualWorldData();
            var model = _modelContainer.Model;
            model.SetPortal(portal, portals);

            Assert.AreEqual(portal, model.Portal);
            Assert.AreEqual(portals, model.Portals);
        }
    }

    public class ToggleFavoriteTests
    {
        PortalThumbnailModelContainer _modelContainer;

        [SetUp]
        public void SetUp()
        {
            _modelContainer = new GameObject().AddComponent<PortalThumbnailModelContainer>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_modelContainer);
        }

        [Test]
        public void GivenFalse_WhenToggleFavorite_ThenTrue()
        {
            var portals = new VirtualWorlds();
            var portal = new VirtualWorldData();
            var model = _modelContainer.Model;
            model.SetPortal(portal, portals);

            Assert.IsFalse(portal.isFavorite);
            model.ToggleFavorite();
            Assert.IsTrue(portal.isFavorite);
        }
    }

    public class DeleteTests
    {
        PortalThumbnailModelContainer _modelContainer;

        [SetUp]
        public void SetUp()
        {
            _modelContainer = new GameObject().AddComponent<PortalThumbnailModelContainer>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_modelContainer);
        }

        [Test]
        public void Given_WhenDeletingPortal_ThenPortalDeleted()
        {
            var portals = new VirtualWorlds();
            var portal = new VirtualWorldData();
            var model = _modelContainer.Model;
            model.SetPortal(portal, portals);

            model.Delete(false);
            Assert.IsNull(model.Portal);
        }
    }
}