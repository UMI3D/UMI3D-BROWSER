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

using inetum.unityUtils.systemIO;

namespace inetum.unityUtils.saveSystem
{
    internal class FileContainerDelegate : IContainerDelegate
    {
        public bool Exists(
            string directories,
            string fileName
        )
        {
            string filePath = Path.Combine(directories, fileName);
            return FileManager.Exists(filePath);
        }

        public bool Delete(
            string directories, 
            string fileName
        )
        {
            string filePath = Path.Combine(directories, fileName);
            return FileManager.Delete(filePath);
        }

        public bool LoadJson(
            string directories, 
            string fileName, 
            out string content
        )
        {
            return FileManager.LoadFromFile(directories, fileName, out _, out content);
        }

        public bool WriteToJson(
            string content, 
            string directories, 
            string fileName
        )
        {
            return FileManager.WriteToFile(content, directories, fileName, out _);
        }
    }
}