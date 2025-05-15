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
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class SliderFactory : MonoBehaviour
    {
        [SerializeField] SliderController _prefab;

        Queue<SliderController> _lstSlidersAvailable = new();

        public int AvailableSlider => _lstSlidersAvailable.Count;

        public bool TryToGetOrCreate(out GameObject control, out SliderModel model, Transform parent)
        {
            if (!_lstSlidersAvailable.TryDequeue(out SliderController controller))
            {
                controller = GameObject.Instantiate(_prefab);
            }
            controller.transform.SetParent(parent.transform, false);
            control = controller.gameObject;
            model = controller.model;

            return true;
        }

        /// <summary>
        /// This method returns a slider GameObject to the pool, deactivates it, and reassigns its parent.<br/>
        /// <br/>
        /// <example>
        /// Given a slider GameObject, when calling Return, then the slider is added to the appropriate queue and deactivated.
        /// <code>
        /// GameObject slider = GetOrCreateSlider(parentTransform, "Volume", 5, 0, 10, true);
        /// Return(slider);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="control">The slider GameObject to be returned to the pool.</param>
        public void Return(GameObject control)
        {
            var controller = control.GetComponent<SliderController>();
            if (!controller) { return; }

            _lstSlidersAvailable.Enqueue(controller);

            controller.gameObject.SetActive(false);
            controller.transform.SetParent(transform, false);
            controller.Clear();
        }
    }
}