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
    /// One Euro Filter
    /// </summary>
    public abstract class AbstractOneEuroFilter<T> : IFilter<T>
    {
        /// <summary>
        /// Minimum cutoff frequency.
        /// </summary>
        [SerializeField] protected float m_MinCutoff = 0.1f;

        /// <summary>
        /// Cutoff slope.
        /// </summary>
        [SerializeField] protected float m_Beta = 0.02f;

        protected T m_LastRawValue;
        protected T m_LastFilteredValue;

        /// <inheritdoc />
        public void Initialize(T value)
        {
            m_LastRawValue = value;
            m_LastFilteredValue = value;
        }

        /// <inheritdoc />
        public T Filter(T value, float deltaTime) => Filter(value, deltaTime, m_MinCutoff, m_Beta);

        protected abstract T Filter(T rawValue, float deltaTime, float minCutoff, float beta);
    }

    public class OneEuroFilterPosition : AbstractOneEuroFilter<Vector3>
    {
        protected override Vector3 Filter(Vector3 rawValue, float deltaTime, float minCutoff, float beta)
        {
            Vector3 speed = (rawValue - m_LastRawValue) / deltaTime;

            Vector3 cutoffs = new(minCutoff, minCutoff, minCutoff);
            Vector3 betaValues = new(beta, beta, beta);

            Vector3 combinedCutoffs = cutoffs + Vector3.Scale(betaValues, speed);

            BurstMathUtility.FastSafeDivide(Vector3.one, Vector3.one + combinedCutoffs, out Vector3 alpha);

            Vector3 rawFiltered = Vector3.Scale(alpha, rawValue);
            Vector3 lastFiltered = Vector3.Scale(Vector3.one - alpha, m_LastFilteredValue);

            Vector3 filteredValue = rawFiltered + lastFiltered;

            m_LastRawValue = rawValue;
            m_LastFilteredValue = filteredValue;

            return filteredValue;
        }
    }

    public class OneEuroFilterRotation : AbstractOneEuroFilter<(Vector3 forward, Vector3 up)>
    {
        protected override (Vector3 forward, Vector3 up) Filter((Vector3 forward, Vector3 up) rawValue, float deltaTime, float minCutoff, float beta)
        {
            Vector3 speedForward = (rawValue.forward - m_LastRawValue.forward) / deltaTime;
            Vector3 speedUp = (rawValue.up - m_LastRawValue.up) / deltaTime;

            Vector3 cutoffs = new(minCutoff, minCutoff, minCutoff);
            Vector3 betaValues = new(beta, beta, beta);

            Vector3 combinedCutoffsForward = cutoffs + Vector3.Scale(betaValues, speedForward);
            Vector3 combinedCutoffsUp = cutoffs + Vector3.Scale(betaValues, speedUp);

            BurstMathUtility.FastSafeDivide(Vector3.one, Vector3.one + combinedCutoffsForward, out Vector3 alphaForward);
            BurstMathUtility.FastSafeDivide(Vector3.one, Vector3.one + combinedCutoffsUp, out Vector3 alphaUp);

            Vector3 rawFilteredForward = Vector3.Scale(alphaForward, rawValue.forward);
            Vector3 rawFilteredUp = Vector3.Scale(alphaUp, rawValue.up);
            Vector3 lastFilteredForward = Vector3.Scale(Vector3.one - alphaForward, m_LastFilteredValue.forward);
            Vector3 lastFilteredUp = Vector3.Scale(Vector3.one - alphaUp, m_LastFilteredValue.up);

            Vector3 filteredValueForward = rawFilteredForward + lastFilteredForward;
            Vector3 filteredValueUp = rawFilteredUp + lastFilteredUp;

            m_LastRawValue = rawValue;
            m_LastFilteredValue = (filteredValueForward, filteredValueUp);

            return (filteredValueForward, filteredValueUp);
        }
    }
}