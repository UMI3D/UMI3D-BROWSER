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
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace umi3d.runtimeBrowser.filter
{
    /// <summary>
    /// Moving Average Filter apply on a <see cref="Vector3"/>
    /// </summary>
    [CreateAssetMenu(menuName = "FilterAlgo/Average Moving Vector3")]
    public class AverageMovingVector3 : FilterAlgoVector3
    {
        [SerializeField] private int m_WindowSize;

        private Cache<Vector3> m_Cache;

        /// <inheritdoc />
        public override void Initialize(Vector3 value)
        {
            m_Cache = new(m_WindowSize);
            m_Cache.Add(value);
        }

        /// <inheritdoc />
        public override Vector3 Filter(Vector3 value, float deltaTime)
        {
            m_Cache.Add(value);

            Vector3 vec = m_Cache[0];
            for (var i = 1; i < m_Cache.Count; i++)
            {
                vec += m_Cache[i];
            }
            return vec / m_Cache.Count;
        }
    }
}