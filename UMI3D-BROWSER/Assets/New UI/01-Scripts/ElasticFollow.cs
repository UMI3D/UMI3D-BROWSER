using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.UX;
using UnityEngine;

namespace umi3dBrowsers.utils
{
    public class ElasticFollow : MonoBehaviour, IFollowable
    {
        [SerializeField] Transform targetTransform;

        [SerializeField] IFollowable.FollowSpeedComponents speedComponents;
        [SerializeField] IFollowable.RotationComponents filterComponents;

        public float SmoothTranslationSpeed
        {
            get => speedComponents.SmoothTranslationSpeed;
            set => speedComponents.SmoothTranslationSpeed = value;
        }
        public float SmoothRotationSpeed
        {
            get => speedComponents.SmoothRotationSpeed;
            set => speedComponents.SmoothRotationSpeed = value;
        }
        public Quaternion CurrentArcCenter
        {
            get => csc;
            set => csc = value;
        }
        public Vector3 CurrentGuardianCenter
        {
            get => Vector3.zero;
            set  { }
        }
        Quaternion csc;

        internal void SetTarget(Transform transform)
        {
            targetTransform = transform;
        }

        private void LateUpdate()
        {        
            if (targetTransform == null) return;

            (this as IFollowable).Translate(targetTransform.position);
            //(this as IFollowable).Rotate(targetTransform.rotation, filterComponents.Filter, filterComponents.Sequences);
        }
    }
}

