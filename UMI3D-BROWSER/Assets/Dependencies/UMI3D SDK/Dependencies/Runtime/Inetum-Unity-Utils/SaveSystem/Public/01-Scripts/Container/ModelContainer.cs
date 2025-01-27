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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Newtonsoft.Json;

namespace inetum.unityUtils.saveSystem
{

    public sealed class ModelContainer<Model>
        where Model : class, IModel<Model>
    {
        static readonly object _lockObject = new object();

        /// <summary>
        /// Indicates whether the ModelContainer has been initialized.<br/>
        /// This property is set to true when the Init method is called for the first time.<br/>
        /// <br/>
        /// <example>
        /// Given a container scope, file name, and directories when initializing the container then the container is set up accordingly.<br/>
        /// <code>
        /// // Example 1: Not initialization
        /// ModelContainer&lt;Model&gt;.hasBeenInitialized == false.
        /// 
        /// // Example 2: Initialization
        /// ModelContainer&lt;Model&gt;.Init();
        /// ModelContainer&lt;Model&gt;.hasBeenInitialized == true.
        /// </code>
        /// </example>
        /// </summary>
        /// <value>True if the container has been initialized, otherwise false.</value>
        public static bool hasBeenInitialized {  get; private set; }
        internal static ModelContainer<Model> instance
        {
            get
            {
                if (!hasBeenInitialized)
                {
                    UnityEngine.Debug.LogError($"[ModelContainer.Init] Error: container has not been initialized for type : {typeof(Model)}");
                    throw new NotInitializedContainerException();
                }
                return _instance.Value;
            }
        }
        /// <summary>
        /// A thread safe lazy initialisation of a model container.
        /// </summary>
        static Lazy<ModelContainer<Model>> _instance = new(() => new());
        ModelContainer() 
        {
            readOnlyData = _data.AsReadOnly();
        }

        /// <summary>
        /// Initializes the ModelContainer with the specified scope, file name, and directories.<br/>
        /// If the container has already been initialized, an error message is logged.<br/>
        /// <br/>
        /// <example>
        /// Given a container scope, file name, and directories when initializing the container then the container is set up accordingly.
        /// <code>
        /// // Example 1: Initialize without arguments
        /// ModelContainer&lt;Model>.Init();
        /// // ModelContainer&lt;Model>.instance.scope == ContainerScope.Persistent.
        /// // ModelContainer&lt;Model>.instance.fileName == "Model.save".
        /// // ModelContainer&lt;Model>.instance.directories == null.
        /// // ModelContainer&lt;Model>.hasBeenInitialized == true.
        ///
        /// // Example 2: Initialize with arguments
        /// ModelContainer&lt;Model2>.Init(ContainerScope.Memory, "TestName", "Directories/DirectoryName");
        /// // ModelContainer&lt;Model2>.instance.scope == ContainerScope.Memory.
        /// // ModelContainer&lt;Model2>.instance.fileName == "TestName".
        /// // ModelContainer&lt;Model2>.instance.directories == "Directories/DirectoryName".
        /// // ModelContainer&lt;Model2>.hasBeenInitialized == true.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="scope">The scope of the container. Default is ContainerScope.Persistent.</param>
        /// <param name="fileName">The file name to be used by the container. Default is null.</param>
        /// <param name="directories">The directories to be used by the container. Default is null.</param>
        public static void Init(
            ContainerScope scope = ContainerScope.Persistent,
            string fileName = null,
            string directories = null
        )
        {
            lock (_lockObject)
            {
                if (hasBeenInitialized)
                {
                    UnityEngine.Debug.LogError($"[ModelContainer.Init] Error: container already initialized for type : {typeof(Model).FullName}.");
                    return;
                }
                hasBeenInitialized = true;

                instance.scope = scope;
                if (!string.IsNullOrEmpty(fileName))
                {
                    instance.fileName = fileName;
                }
                if (!string.IsNullOrEmpty(directories))
                {
                    instance.directories = directories;
                }

                switch (scope)
                {
                    case ContainerScope.Persistent:
#if UNITY_ANDROID && !UNITY_EDITOR
                        instance.containerDelegate = new PlayerPrefsContainerDelegate();
#else
                        instance.containerDelegate = new FileContainerDelegate();
#endif
                        break;

                    case ContainerScope.Memory:
#if UNITY_ANDROID && !UNITY_EDITOR
                        IContainerDelegate innerDelegate = new PlayerPrefsContainerDelegate();
                        instance.containerDelegate = new InMemoryContainerDelegate(innerDelegate, false);
#else
                        IContainerDelegate innerDelegate = new FileContainerDelegate();
                        instance.containerDelegate = new InMemoryContainerDelegate(innerDelegate, false);
#endif
                        break;

                    case ContainerScope.MemoryAndLoad:
#if UNITY_ANDROID && !UNITY_EDITOR
                        innerDelegate = new PlayerPrefsContainerDelegate();
                        instance.containerDelegate = new InMemoryContainerDelegate(innerDelegate, true);
#else
                        innerDelegate = new FileContainerDelegate();
                        instance.containerDelegate = new InMemoryContainerDelegate(innerDelegate, true);
#endif
                        break;

                    default:
                        UnityEngine.Debug.LogError($"[ModelContainer.Init] Error: unhandled case '{scope}'.");
#if UNITY_ANDROID && !UNITY_EDITOR
                        instance.containerDelegate = new PlayerPrefsContainerDelegate();
#else
                        instance.containerDelegate = new FileContainerDelegate();
#endif
                        break;
                }
            }
        }

        internal bool hasChanged { get; set; } = false;
        public IReadOnlyList<Model> readOnlyData;
        List<Model> _data = new();

        internal IContainerDelegate containerDelegate;

        internal ContainerScope scope { get; private set; } = ContainerScope.Persistent;
        internal string fileName {  get; private set; } = $"{typeof(Model).Name}.save";
        internal string directories { get; private set; } = null;

        /// <summary>
        /// This method adds a model item to the container if it is not already present.<br/>
        /// It ensures thread safety by locking the operation.<br/>
        /// If the item was added then <see cref="hasChanged"/> is true.<br/>
        /// <br/>
        /// <example>
        /// Given models when adding them to the container then models are added if not already present.<br/>
        /// <code>
        /// ModelContainer&lt;Model&gt;.instance.Add(item1); // Return true.
        /// ModelContainer&lt;Model&gt;.instance.Add(item1); // Return false.
        /// ModelContainer&lt;Model&gt;.instance.Add(item2); // Return true.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="item">The model item to be added.</param>
        /// <returns>True if the item was added, otherwise false.</returns>
        internal bool Add(Model item)
        {
            lock (_lockObject)
            {
                if (_data.Contains(item))
                {
                    return false;
                }

                _data.Add(item);
                hasChanged = true;
                return true;
            }
        }

        /// <summary>
        /// This method removes a model item from the container if it is present.<br/>
        /// It ensures thread safety by locking the operation.<br/>
        /// If the item was removed then <see cref="hasChanged"/> is true.<br/>
        /// <br/>
        /// <example>
        /// Given models added to the container when removing them from the container then models are removed.<br/>
        /// <code>
        /// ModelContainer&lt;Model&gt;.instance.Remove(item1); // Return false.
        /// 
        /// ModelContainer&lt;Model&gt;.instance.Add(item1);
        /// ModelContainer&lt;Model&gt;.instance.Remove(item1); // Return true.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="item">The model item to be removed.</param>
        /// <returns>True if the item was removed, otherwise false.</returns>
        internal bool Remove(Model item)
        {
            lock (_lockObject)
            {
                if (!_data.Remove(item)) 
                {
                    return false;
                }

                hasChanged = true;
                return true;
            }
        }

        /// <summary>
        /// This method checks if a file '<see cref="fileName"/>' exists in the directory '<see cref="directories"/>'.<br/>
        /// <br/>
        /// <example>
        /// Given no save when checking if file exists then false.<br/>
        /// <code>
        /// ModelContainer&lt;Model&gt;.instance.FileExists(); // Return false.
        /// </code>
        /// 
        /// Given save when checking if file exists then true.<br/>
        /// <code>
        /// ModelContainer&lt;Model&gt;.instance.WriteToFile();
        /// ModelContainer&lt;Model&gt;.instance.FileExists(); // Return true.
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>True if the file exists, otherwise false.</returns>
        internal bool FileExists()
        {
            return containerDelegate.Exists(directories, fileName);
        }

        internal bool LoadFromFile()
        {
            if (!FileExists()) { return false; }

            bool hasLoadedJson = containerDelegate.LoadJson(
                directories, 
                fileName,
                out string json
            );
            if (!hasLoadedJson) { return false; }

            List<Model> models;
            try
            {
                lock (_lockObject)
                {
                    models = JsonConvert.DeserializeObject<List<Model>>(json);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[ModelContainer.LoadFromFile] Cannot override the data of type '{typeof(Model).FullName}' from its json.");
                UnityEngine.Debug.LogException(e);
                return false;
            }

            _data.Clear();
            _data.AddRange(models);
            hasChanged = false;
            return true;
        }

        internal bool WriteToFile()
        {
            string json = null;
            try
            {
                lock ( _lockObject)
                {
                    json = JsonConvert.SerializeObject(_data);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[ModelContainer.WriteToFile] Cannot convert the data of type '{typeof(Model).FullName}' to its json.");
                UnityEngine.Debug.LogException(e);
                return false;
            }

            bool hasSucceeded = containerDelegate.WriteToJson(
                json,
                directories,
                fileName
            );

            if (hasSucceeded)
            {
                hasChanged = false;
            }

            return hasSucceeded;
        }

        [Conditional("UNITY_EDITOR")]
        internal static void Rest()
        {
            _instance = new(() => new());
            hasBeenInitialized = false;
        }
    }
}