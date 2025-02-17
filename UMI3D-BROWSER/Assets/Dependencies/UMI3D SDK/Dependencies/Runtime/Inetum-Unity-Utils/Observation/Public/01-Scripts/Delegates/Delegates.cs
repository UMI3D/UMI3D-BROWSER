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

using System;
using System.Collections;
using System.Collections.Generic;

namespace inetum.unityUtils.observation
{
    public enum Flow
    {
        /// <summary>
        /// Continue the foreach loop with the next element.
        /// </summary>
        Continue,
        /// <summary>
        /// Stop the foreach loop.
        /// </summary>
        Break,
    }

    public class Delegates<DelegateInterface> : IEnumerable, IEnumerable<DelegateInterface>, IReadOnlyList<DelegateInterface>
        where DelegateInterface : class
    {
        internal List<DelegateInterface> _delegates;

        public int Count => _delegates.Count;

        public DelegateInterface this[int index] => _delegates[index];

        #region Initialization

        public Delegates() 
        {
            _delegates = new List<DelegateInterface>();
        }

        public Delegates(List<DelegateInterface> delegates)
        {
            _delegates = delegates;
        }

        public Delegates(Delegates<DelegateInterface> delegates)
        {
            _delegates = delegates._delegates;
        }

        #endregion

        #region Data management

        public void Add(DelegateInterface @delegate)
        {
            if (_delegates.Contains(@delegate))
            {
                return;
            }

            _delegates.Add(@delegate);
        }

        public bool Remove(DelegateInterface @delegate)
        {
            return _delegates.Remove(@delegate);
        }

        public void Insert(int index, DelegateInterface @delegate)
        {
            if (_delegates.Contains(@delegate))
            {
                _delegates.Remove(@delegate);
            }

            _delegates.Insert(index, @delegate);
        }

        public void RemoveAt(int index)
        {
            _delegates.RemoveAt(index);
        }

        #endregion

        public void ForEach(Func<DelegateInterface, Flow> action)
        {
            foreach (DelegateInterface @delegate in _delegates)
            {
                try
                {
                    Flow? flow = action?.Invoke(@delegate);

                    if (flow.HasValue && flow.Value == Flow.Break)
                    {
                        return;
                    }
                }
                catch (NotImplementedException)
                {
                    UnityEngine.Debug.LogWarning($"[Delegates<{typeof(DelegateInterface).Name}>] Warning: a delegate raise a NotImplementedException");
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"[Delegates<{typeof(DelegateInterface).Name}>] Error: a delegate raise an exception");
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        public List<ReturnType> ForEach<ReturnType>(Func<DelegateInterface, ReturnType> action)
        {
            List<ReturnType> result = new();

            foreach (DelegateInterface @delegate in _delegates)
            {
                try
                {
                    ReturnType _result;
                    if (action != null)
                    {
                        _result = action.Invoke(@delegate);
                    }
                    else { _result = default; }
                    result.Add(_result);
                    continue;
                }
                catch (NotImplementedException)
                {
                    UnityEngine.Debug.LogWarning($"[Delegates<{typeof(DelegateInterface).Name}>] Warning: a delegate raise a NotImplementedException");
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"[Delegates<{typeof(DelegateInterface).Name}>] Error: a delegate raise an exception");
                    UnityEngine.Debug.LogException(e);
                }
                result.Add(default);
            }

            return result;
        }

        public IEnumerator<DelegateInterface> GetEnumerator()
        {
            return _delegates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _delegates.GetEnumerator();
        }
    }
}