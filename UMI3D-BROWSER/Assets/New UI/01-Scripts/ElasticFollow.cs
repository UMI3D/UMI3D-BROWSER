using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.UX;
using UnityEngine;

namespace umi3dBrowsers.utils
{
    public class ElasticFollow : MonoBehaviour, IFollowable
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Vector3 filter;
        [SerializeField] private int sequence;
        [SerializeField] private IFollowable.FollowTranslationByDistanceAndRotationComponents followTranslationByDistanceAndRotation;
        [SerializeField] private IFollowable.FollowRotationComponents followRotation;

        public float SmoothTranslationSpeed { get => followTranslationByDistanceAndRotation.SmoothTranslationSpeed;
            set => followTranslationByDistanceAndRotation.SmoothTranslationSpeed = value; }
        public float SmoothRotationSpeed { get => followRotation.SmoothRotationSpeed; set => followRotation.SmoothRotationSpeed = value; }
        public Vector3 Offset { get => followTranslationByDistanceAndRotation.Offset; 
            set => followTranslationByDistanceAndRotation.Offset = value; }
        public Vector3 TranslationTarget { get => followTranslationByDistanceAndRotation.TranslationTarget; 
            set => followTranslationByDistanceAndRotation.TranslationTarget = value; }
        public Quaternion RotationTarget { get => Quaternion.Euler(followRotation.RotationTarget); set => followRotation.RotationTarget = value.eulerAngles; }

        internal void SetTarget(Transform transform)
        {
            targetTransform = transform;
        }

        private void LateUpdate()
        {        
            if (targetTransform == null) return;

            (this as IFollowable).Translate(targetTransform.position);
            (this as IFollowable).Rotate(targetTransform.rotation, filter, sequence);
        }
    }
}

