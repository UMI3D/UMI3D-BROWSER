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
using System.Collections.Generic;

namespace inetum.unityUtils.saveSystem
{
    public class ModelContext 
    {
        public static ModelContext instance => _instance.Value;
        /// <summary>
        /// A thread safe lazy initialisation of a model context.
        /// </summary>
        static readonly Lazy<ModelContext> _instance = new(() => new());

        public IReadOnlyList<Model> Fetch<Model>() where Model : class, IModel<Model>
        {
            return ModelContainer<Model>.instance.readOnlyData;
        }

        public bool Add<Model>(Model item) where Model : class, IModel<Model>
        {
            return ModelContainer<Model>.instance.Add(item);
        }

        public bool Remove<Model>(Model item) where Model : class, IModel<Model>
        {
            return ModelContainer<Model>.instance.Remove(item);
        }

        public bool HasChanged<Model>() where Model : class, IModel<Model>
        {
            return ModelContainer<Model>.instance.hasChanged;
        }

        public void Save<Model>() where Model : class, IModel<Model>
        {
            ModelContainer<Model>.instance.WriteToFile();
        }
    }
}