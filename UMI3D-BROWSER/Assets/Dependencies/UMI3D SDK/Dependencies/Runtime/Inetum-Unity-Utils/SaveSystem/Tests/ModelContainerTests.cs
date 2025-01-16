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
using inetum.unityUtils.saveSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ModelContainerTests
{
    public class InitTest
    {
        public class FooClass
        {
            public string name;
        }

        public class Model: IModel<Model>
        {
            public int id;
            public FooClass foo;

            string privateName;
        }

        public class Model2 : IModel<Model2>
        {
            public int id;
            public FooClass foo;

            string privateName;
        }

        [TearDown]
        public void TearDown()
        {
            ModelContainer<Model>.Rest();
            ModelContainer<Model2>.Rest();
        }

        [Test]
        public void WhenInitializeWithoutArgument_ThenPersistentAndNoDirectoriesAndFileNameIsModelName()
        {
            // -- New Test ---
            Assert.False(ModelContainer<Model>.hasBeenInitialized);

            // -- New Test ---
            ModelContainer<Model>.Init();
            Assert.True(ModelContainer<Model>.hasBeenInitialized);
            Assert.AreEqual(ContainerScope.Persistent, ModelContainer<Model>.instance.scope);
            Assert.AreEqual("Model.save", ModelContainer<Model>.instance.fileName);
            Assert.Null(ModelContainer<Model>.instance.directories);

            // -- New Test ---
            ModelContainer<Model>.Init();
            LogAssert.Expect(LogType.Error, $"[ModelContainer.Init] Error: container already initialized for type : {typeof(Model).FullName}.");
        }

        [Test]
        public void WhenInitializeWithArguments_ThenScopeAndDirectoriesAndFileName()
        {
            // -- New Test ---
            ModelContainer<Model>.Init(ContainerScope.MemoryAndLoad);
            Assert.True(ModelContainer<Model>.hasBeenInitialized);
            Assert.AreEqual(ContainerScope.MemoryAndLoad, ModelContainer<Model>.instance.scope);
            Assert.AreEqual("Model.save", ModelContainer<Model>.instance.fileName);
            Assert.Null(ModelContainer<Model>.instance.directories);

            // -- New Test ---
            ModelContainer<Model2>.Init(ContainerScope.Memory, "TestName", "Test/Directories/Name");
            Assert.True(ModelContainer<Model2>.hasBeenInitialized);
            Assert.AreEqual(ContainerScope.Memory, ModelContainer<Model2>.instance.scope);
            Assert.AreEqual("TestName", ModelContainer<Model2>.instance.fileName);
            Assert.AreEqual("Test/Directories/Name", ModelContainer<Model2>.instance.directories);
        }
    }

    public class AddTest
    {
        public class FooClass
        {
            public string name;
        }

        public class Model : IModel<Model>
        {
            public int id;
            public FooClass foo;

            string privateName;
        }

        [TearDown]
        public void TearDown()
        {
            ModelContainer<Model>.Rest();
        }

        [Test]
        public void GivenModels_WhenAddingThemToTheContainer_ThenModelAreAddedIfNotAlreadyPresent()
        {
            Model model = new Model();
            Model model2 = new Model();
            Model model3 = new Model();
            ModelContainer<Model>.Init();

            // -- New Test ---
            Assert.AreEqual(0, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.False(ModelContainer<Model>.instance.hasChanged);

            // -- New Test ---
            bool result = ModelContainer<Model>.instance.Add(model);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(1, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model));

            // -- New Test ---
            result = ModelContainer<Model>.instance.Add(model2);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(2, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model));
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model2));

            // -- New Test ---
            result = ModelContainer<Model>.instance.Add(model3);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(3, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model));
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model2));
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model3));

            // -- New Test ---
            result = ModelContainer<Model>.instance.Add(model2);
            Assert.False(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(3, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model));
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model2));
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model3));
        }
    }

    public class RemoveTest
    {
        public class FooClass
        {
            public string name;
        }

        public class Model : IModel<Model>
        {
            public int id;
            public FooClass foo;

            string privateName;
        }

        [TearDown]
        public void TearDown()
        {
            ModelContainer<Model>.Rest();
        }

        [Test]
        public void GivenModelsAddedToTheContainer_WhenRemovingThemFromTheContainer_ThenModelAreRemoved()
        {
            Model model = new Model();
            Model model2 = new Model();
            Model model3 = new Model();
            ModelContainer<Model>.Init();

            // -- New Test ---
            bool result = ModelContainer<Model>.instance.Remove(model);
            Assert.False(result);
            Assert.False(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(0, ModelContainer<Model>.instance.readOnlyData.Count);

            // -- New Test ---
            ModelContainer<Model>.instance.Add(model);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(1, ModelContainer<Model>.instance.readOnlyData.Count);
            result = ModelContainer<Model>.instance.Remove(model);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(0, ModelContainer<Model>.instance.readOnlyData.Count);

            // -- New Test ---
            ModelContainer<Model>.instance.Add(model);
            ModelContainer<Model>.instance.Add(model2);
            ModelContainer<Model>.instance.Add(model3);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(3, ModelContainer<Model>.instance.readOnlyData.Count);
            result = ModelContainer<Model>.instance.Remove(model3);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(2, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model));
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model2));
            result = ModelContainer<Model>.instance.Remove(model2);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(1, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model));
            result = ModelContainer<Model>.instance.Remove(model);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(0, ModelContainer<Model>.instance.readOnlyData.Count);

            // -- New Test ---
            ModelContainer<Model>.instance.Add(model);
            ModelContainer<Model>.instance.Add(model2);
            ModelContainer<Model>.instance.Add(model3);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(3, ModelContainer<Model>.instance.readOnlyData.Count);
            result = ModelContainer<Model>.instance.Remove(model2);
            Assert.True(result);
            Assert.True(ModelContainer<Model>.instance.hasChanged);
            Assert.AreEqual(2, ModelContainer<Model>.instance.readOnlyData.Count);
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model));
            Assert.True(ModelContainer<Model>.instance.readOnlyData.Contains(model3));
        }
    }

    public class LoadFromFileTest
    {
        public class FooClass
        {
            public string name;
        }

        public class Model : IModel<Model>
        {
            public int id;
            public FooClass foo;

            string privateName;
        }

        [TearDown]
        public void TearDown()
        {
            ModelContainer<Model>.Rest();
        }

        [Test]
        public void GivenNoSave_WhenLoadingFromFile_ThenEmptyList()
        {
            ModelContainer<Model>.Init();

            bool result = ModelContainer<Model>.instance.LoadFromFile();

            Assert.False(result);
            Assert.AreEqual(0, ModelContainer<Model>.instance.readOnlyData.Count);
        }
    }

    public class WriteToFileTest
    {
        public class FooClass
        {
            public string name;
        }

        public class Model : IModel<Model>
        {
            public int id;
            public FooClass foo;

            string privateName;
        }

        [TearDown]
        public void TearDown()
        {
            ModelContainer<Model>.instance.containerDelegate.Delete(
                ModelContainer<Model>.instance.directories,
                ModelContainer<Model>.instance.fileName
            );
            ModelContainer<Model>.Rest();
        }

        [Test]
        public void GivenNoSave_WhenWritingToFile_ThenSaved()
        {
            ModelContainer<Model>.Init();

            bool result = ModelContainer<Model>.instance.WriteToFile();

            Assert.True(result);
            Assert.False(ModelContainer<Model>.instance.hasChanged);
            bool exists = ModelContainer<Model>.instance.containerDelegate.Exists(
                ModelContainer<Model>.instance.directories, 
                ModelContainer<Model>.instance.fileName
            );
            Assert.True(exists);
        }
    }
}
