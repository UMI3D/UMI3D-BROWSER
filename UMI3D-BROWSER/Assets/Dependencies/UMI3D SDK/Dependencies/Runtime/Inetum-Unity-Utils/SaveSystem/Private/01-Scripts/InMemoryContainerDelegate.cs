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

namespace inetum.unityUtils.saveSystem
{
    internal class InMemoryContainerDelegate : IContainerDelegate
    {
        IContainerDelegate loadContainerDelegate;
        bool loadPersistentData;

        public InMemoryContainerDelegate(IContainerDelegate loadContainerDelegate, bool loadPersistentData)
        {
            this.loadContainerDelegate = loadContainerDelegate;
            this.loadPersistentData = loadPersistentData;
        }

        public bool Exists(string directories, string fileName)
        {
            if (loadPersistentData)
            {
                return loadContainerDelegate.Exists(directories, fileName);
            }

            return true;
        }

        public bool LoadJson(string directories, string fileName, out string content)
        {
            if (loadPersistentData)
            {
                return loadContainerDelegate.LoadJson(directories, fileName, out content);
            }

            content = null;
            return true;
        }

        public bool WriteToJson(string content, string directories, string fileName)
        {
            return true;
        }
    }
}