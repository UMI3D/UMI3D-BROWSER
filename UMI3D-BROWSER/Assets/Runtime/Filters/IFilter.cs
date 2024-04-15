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
limit
*/

namespace umi3d.runtimeBrowser.filter
{
    public interface IFilter<T>
    {
        /// <summary>
        /// Initialize the filter with a first value
        /// </summary>
        /// <param name="value">First value for the filter</param>
        void Initialize(T value);

        /// <summary>
        /// Filter the value
        /// </summary>
        /// <param name="value">Value to be add or filter</param>
        /// <param name="deltaTime">DeltaTime to use</param>
        /// <returns>Data filtered</returns>
        T Filter(T value, float deltaTime);
    }
}