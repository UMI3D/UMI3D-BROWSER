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
using UnityEngine;

namespace inetum.unityUtils.saveSystem
{
    public class ModelContainer<Model>
        where Model : class, IModel<Model>
    {
        static readonly object _lockObject = new object();

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
        static readonly Lazy<ModelContainer<Model>> _instance = new(() => new());
        ModelContainer() 
        {
            readOnlyData = _data.AsReadOnly();
        }

        public bool hasChanged { get; private set; } = false;
        public IReadOnlyList<Model> readOnlyData;
        List<Model> _data = new();

        IContainerDelegate containerDelegate;

        public ContainerScope scope { get; private set; } = ContainerScope.Persistent;
        public string fileName {  get; private set; } = typeof(Model).Name;
        public string directories { get; private set; } = null;

        public static void Init(
            ContainerScope scope, 
            string fileName = null, 
            string directories = null
        )
        {
            lock (_lockObject)
            {
                if (hasBeenInitialized)
                {
                    UnityEngine.Debug.LogError($"[ModelContainer.Init] Error: container already initialized for type : {typeof(Model).FullName}");
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

        public bool Add(Model item)
        {
            lock (_lockObject)
            {
                if (instance._data.Contains(item))
                {
                    return false;
                }

                instance._data.Add(item);
                hasChanged = true;
                return true;
            }
        }

        public bool Remove(Model item)
        {
            lock (_lockObject)
            {
                if (instance._data.Remove(item)) 
                {
                    hasChanged = true;
                    return true;
                }

                return false;
            }
        }

        public bool Exists()
        {
            return containerDelegate.Exists(directories, fileName);
        }

        public bool LoadFromFile()
        {
            bool hasLoadedJson = containerDelegate.LoadJson(
                directories, 
                fileName,
                out string json
            );

            if (!hasLoadedJson) { return false; }

            try
            {
                lock (_lockObject)
                {
                    JsonUtility.FromJsonOverwrite(json, _data);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[ModelContainer.LoadFromFile] Cannot override the data of type '{typeof(Model).FullName}' from its json.");
                UnityEngine.Debug.LogException(e);
                return false;
            }

            hasChanged = false;
            return true;
        }

        public bool WriteToFile()
        {
            string json = null;
            try
            {
                lock ( _lockObject)
                {
                    json = JsonUtility.ToJson(_data);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[PersistentScriptableModel] Cannot convert the data of type '{typeof(Model).FullName}' to its json.");
                UnityEngine.Debug.LogException(e);
                return false;
            }

            hasChanged = false;

            return containerDelegate.WriteToJson(
                json, 
                directories, 
                fileName
            );
        }
    }
}