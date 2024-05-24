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

using System;
using UnityEngine;
using System.Threading.Tasks;

namespace inetum.unityUtils.async
{
    public static class AsyncExtensions 
    {
        /// <summary>
        /// Run the action callback if the task is completed.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="action"></param>
        public static void IfCompleted(this Task task, Action action)
        {
            if (task?.IsCompleted ?? false)
            {
                action?.Invoke();
            }
        }

        /// <summary>
        /// Run the action callback if the task is completed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="action"></param>
        public static void IfCompleted<T>(this Task<T> task, Action<T> action)
        {
            if (task?.IsCompleted ?? false)
            {
                action?.Invoke(task.Result);
            }
        }

        public static bool TryGet<T>(this Task<T> task, out T variable)
        {
            if (task?.IsCompleted ?? false)
            {
                variable = task.Result;
                return true;
            }
            else
            {
                variable = default;
                return false;
            }
        }
    }
}