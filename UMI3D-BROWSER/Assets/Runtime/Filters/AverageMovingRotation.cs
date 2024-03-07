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
    /// Moving Average Filter apply on a Rotation
    /// </summary>
    /// <remarks>
    /// It need a tuple of two <see cref="Vector3"/> representing the forward and up vector of the rotation.
    /// </remarks>
    [CreateAssetMenu(menuName = "FilterAlgo/Moving Average Rotation")]
    public class AverageMovingRotation : FilterAlgoRotation
    {
        [SerializeField] private int m_WindowSize;

        private Cache<(Vector3 forward, Vector3 up)> m_Cache;

        /// <inheritdoc />
        public override void Initialize((Vector3 forward, Vector3 up) value)
        {
            m_Cache = new(m_WindowSize);
            m_Cache.Add(value);
        }

        /// <inheritdoc />
        public override (Vector3 forward, Vector3 up) Filter((Vector3 forward, Vector3 up) value, float deltaTime)
        {
            m_Cache.Add(value);

            Vector3 newForward = m_Cache[0].forward;
            for (var i = 1; i < m_Cache.Count; i++)
            {
                newForward += m_Cache[i].forward;
            }
            Vector3 newUp = m_Cache[0].up;
            for (var i = 1; i < m_Cache.Count; i++)
            {
                newUp += m_Cache[i].up;
            }
            return (newForward / m_Cache.Count, newUp / m_Cache.Count);
        }
    }
}