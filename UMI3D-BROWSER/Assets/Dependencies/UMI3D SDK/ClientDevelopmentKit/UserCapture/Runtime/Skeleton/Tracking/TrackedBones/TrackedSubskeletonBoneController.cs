/*
Copyright 2019 - 2021 Inetum

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
using umi3d.common.core;
using umi3d.common.userCapture.description;
using UnityEngine;

namespace umi3d.cdk.userCapture.tracking
{

    [Obsolete()]
    public class TrackedSubskeletonBoneController : TrackedSubskeletonBone, IController
    {
        private UnityTransformation _transformation;

        private void Start()
        {
            _transformation = new(transform);
        }

        public ITransformation transformation => _transformation;

        public Vector3 position
        {
            get
            {
                return this.transform.position;
            }

            set
            {
                this.transform.position = value;
            }
        }

        public Quaternion rotation
        {
            get
            {
                return this.transform.localRotation;
            }
            set
            {
                this.transform.rotation = value;
            }
        }

        public Vector3 scale
        {
            get
            {
                return this.transform.localScale;
            }
            set
            {
                this.transform.localScale = value;
            }
        }

        public bool isActive { get; set; }

        uint IController.boneType => boneType;

        public bool isOverrider { get; set; } = false;

        public event System.Action Destroyed;

        public void Destroy()
        {
            GameObject.Destroy(this);
        }

        protected void OnDestroy()
        {
            Destroyed?.Invoke();
        }

        public override ControllerDto ToControllerDto()
        {
            var dto = base.ToControllerDto();
            if (dto == null)
                return null;
            dto.isOverrider = true;
            return dto;
        }
    }
}