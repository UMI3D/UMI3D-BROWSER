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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.runtimeBrowser.filter
{
    /// <summary>
    /// Moving Average Filter.
    /// </summary>
    public abstract class AbstractAverageMovingFilter<T> : IFilter<T>
    {
        public int WindowSize => windowSize;
        protected int windowSize;

        protected IEnumerable<T> Window => cache.Count > windowSize ? cache.Values.Skip(cache.Count - windowSize) : cache.Values;

        private protected Cache<T> cache;

        protected AbstractAverageMovingFilter(int windowSize = 10)
        {
            this.windowSize = windowSize <= 0 ? 1 : windowSize;
            cache = new Cache<T>(this.windowSize);
        }

        /// <inheritdoc />
        public void Initialize(T value)
        {
            cache.Add(value);
        }

        /// <inheritdoc />
        public abstract T Filter(T value, float deltaTime);

        public void ChangeSensitivity(int newWindowSize)
        {
            windowSize = newWindowSize <= 0 ? 1 : newWindowSize;

            if (windowSize > cache.MaxElements)
            {
                Cache<T> newCache = new Cache<T>(windowSize * 2);
                foreach (T value in cache)
                {
                    newCache.Add(value);
                }
                cache = newCache;
            }
        }
    }

    public class AverageMovingFilterPosition : AbstractAverageMovingFilter<Vector3>
    {
        public AverageMovingFilterPosition(int windowSize = 15) : base(windowSize)
        {
        }

        public override Vector3 Filter(Vector3 value, float deltaTime)
        {
            cache.Add(value);

            Vector3 vec = Vector3.zero;

            foreach (Vector3 cachedValue in Window)
                vec += cachedValue;
            
            return vec / windowSize;
        }
    }

    public class AverageMovingFilterRotation : AbstractAverageMovingFilter<(Vector3 forward, Vector3 up)>
    {
        public AverageMovingFilterRotation(int windowSize = 15) : base(windowSize)
        {
        }

        public override (Vector3 forward, Vector3 up) Filter((Vector3 forward, Vector3 up) value, float deltaTime)
        {
            cache.Add(value);

            Vector3 newForward = Vector3.zero;
            Vector3 newUp = Vector3.zero;

            foreach (var (forward, up) in Window)
            {
                newForward += forward;
                newUp += up;
            }

            return (newForward / windowSize, newUp / windowSize);
        }
    }
}