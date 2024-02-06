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
using UnityEngine;

namespace umi3dBrowsers.utils
{
    [RequireComponent(typeof(BoxCollider))]
    public class UIColliderScaller : MonoBehaviour
    {
        private BoxCollider _collider;
        private RectTransform _transform;

        public static List<UIColliderScaller> uiColliderScalers = new();

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _transform = GetComponent<RectTransform>();

            if (!uiColliderScalers.Contains(this))
            {
                uiColliderScalers.Add(this);
            }
        }

        [ContextMenu("ScaleCollider")]
        public void ScaleCollider()
        {
            if(_transform == null) _transform = GetComponent<RectTransform>();
            if (_collider == null) _collider = GetComponent<BoxCollider>();

            Vector3 size = new Vector3(_transform.rect.width, _transform.rect.height, 0.01f);
            _collider.size = size;
        }
    }
}

