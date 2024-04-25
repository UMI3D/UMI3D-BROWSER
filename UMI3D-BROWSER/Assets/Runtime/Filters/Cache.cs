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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace umi3d.runtimeBrowser.filter
{
    /// <summary>
    /// A list with the comportement of a queue with a max Size.
    /// </summary>
    /// <typeparam name="T">Type of the elements inside the cache</typeparam>
    public class Cache<T> : IEnumerable<T>
    {
        private readonly int m_MaxElements;
        protected List<T> m_Values;

        public IReadOnlyList<T> Values => m_Values;

        /// <summary>
        /// Max elements this cache can contain.
        /// </summary>
        public int MaxElements => m_MaxElements;

        /// <summary>
        /// Actual number of element inside the cache
        /// </summary>
        public int Count => m_Values.Count;

        public T this[int index] => m_Values.ElementAt(index);

        /// <summary>
        /// Create a cache of <see cref="MaxElements"/> elements max.
        /// </summary>
        /// <param name="maxElements">Maximum number of element inside the cache</param>
        public Cache(int maxElements = 10)
        {
            m_MaxElements = maxElements;
            m_Values = new List<T>();
        }

        /// <summary>
        /// Add a value to the cache.
        /// The latest element of the cache will be removed if there is no more room.
        /// </summary>
        /// <param name="value">Object to be added to the cache</param>
        public void Add(T value)
        {
            if (m_Values.Count + 1 > m_MaxElements && m_Values.Count > 0)
                m_Values.RemoveAt(0);
            m_Values.Add(value);
        }

        /// <summary>
        /// Clear the cache
        /// </summary>
        public void Clear() => m_Values = new List<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return m_Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Values.GetEnumerator();
        }

        public static implicit operator List<T>(Cache<T> cache) => cache.m_Values;
    }

}