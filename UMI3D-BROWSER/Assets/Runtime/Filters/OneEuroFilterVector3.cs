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
    /// One Euro Filter apply on a <see cref="Vector3"/>
    /// </summary>
    [CreateAssetMenu(menuName = "FilterAlgo/One Euro Vector3")]
    public class OneEuroFilterVector3 : FilterAlgoVector3
    {
        [SerializeField] private float m_MinCutoff = 0.1f;
        [SerializeField] private float m_Beta = 0.02f;

        private Vector3 m_LastRawValue;
        private Vector3 m_LastFilteredValue;

        /// <inheritdoc />
        public override void Initialize(Vector3 value)
        {
            m_LastRawValue = value;
            m_LastFilteredValue = value;
        }

        /// <inheritdoc />
        public override Vector3 Filter(Vector3 value, float deltaTime) => Filter(value, deltaTime, m_MinCutoff, m_Beta);

        private Vector3 Filter(Vector3 rawValue, float deltaTime, float minCutoff, float beta)
        {
            var speed = (rawValue - m_LastRawValue) / deltaTime;

            var cutoffs = new Vector3(minCutoff, minCutoff, minCutoff);
            var betaValues = new Vector3(beta, beta, beta);

            var combinedCutoffs = cutoffs + Vector3.Scale(betaValues, speed);

            BurstMathUtility.FastSafeDivide(Vector3.one, Vector3.one + combinedCutoffs, out Vector3 alpha);

            var rawFiltered = Vector3.Scale(alpha, rawValue);
            var lastFiltered = Vector3.Scale(Vector3.one - alpha, m_LastFilteredValue);

            var filteredValue = rawFiltered + lastFiltered;

            m_LastRawValue = rawValue;
            m_LastFilteredValue = filteredValue;

            return filteredValue;
        }
    }
}