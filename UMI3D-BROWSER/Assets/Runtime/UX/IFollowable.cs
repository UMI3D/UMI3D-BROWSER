/*
Copyright 2019 - 2023 Inetum

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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserRuntime.UX
{
    /// <summary>
    /// This interface allows the classes that implement it to follow a target (translation and rotation).
    /// 
    /// <para>
    /// <exemple>
    /// Follow a target located at position with an offset.
    /// <code>
    ///     (this as IFollowable).Translate(position, offset);
    /// </code>
    /// </exemple>
    /// <exemple>
    /// Follow the rotation of a target but only in the Y axis.
    /// <code>
    ///     (this as IFollowable).Rotate(rotation, Vector3.up);
    /// </code>
    /// </exemple>
    /// </para>
    /// </summary>
    public interface IFollowable
    {
        /// <summary>
        /// Editor container for organizing purpose.<br/>
        /// <br/>
        /// With these components you can track the translation and rotation speed.
        /// </summary>
        [Serializable]
        public struct FollowSpeedComponents
        {
            [Tooltip("Translation speed of the follower.")]
            public float SmoothTranslationSpeed;

            [Tooltip("Rotation speed of the follower.")]
            public float SmoothRotationSpeed;
        }

        /// <summary>
        /// Editor container for organizing purpose.<br/>
        /// <br/>
        /// With these components you can track the translation offset distance and rotation.
        /// </summary>
        [Serializable]
        public struct FollowOffsetDistanceRotationComponents
        {
            [Tooltip("Follower's offset distance from the target.")]
            public float OffsetDistance;
            [Tooltip("Follower's offset rotation from the target.")]
            public Vector3 OffsetRotation;

            /// <summary>
            /// Compute the offset position via <see cref="OffsetDistance"/> and <see cref="OffsetRotation"/>.
            /// </summary>
            public Vector3 Offset
            {
                get => Quaternion.Euler(OffsetRotation) * new Vector3(0f, 0f, OffsetDistance);
                set
                {
                    OffsetDistance = value.magnitude;
                    OffsetRotation = Quaternion
                        .FromToRotation(new Vector3(0f, 0f, value.magnitude), value)
                        .eulerAngles;
                }
            }
        }

        /// <summary>
        /// Editor container for organizing purpose.<br/>
        /// <br/>
        /// With these components you can filter the rotation.
        /// </summary>
        [Serializable]
        public struct RotationComponents
        {
            [Tooltip("Rotation filter.")]
            public Vector3 filter;
            [Tooltip("Lazy rotation arc percentage.")]
            public float lazyArcPct;
        }


        /// <summary>
        /// The speed of the translate animation.
        /// </summary>
        float SmoothTranslationSpeed { get; set; }
        /// <summary>
        /// The speed of the rotate animation.
        /// </summary>
        float SmoothRotationSpeed { get; set; }

        /// <summary>
        /// The offset added to <see cref="TranslationTarget"/> when translating.
        /// </summary>
        Vector3 Offset { get; set; }
        /// <summary>
        /// The <see cref="Offset"/> as a distance and a rotation.
        /// 
        /// <para>
        /// <code>
        ///     ^
        ///     |   x : Offset
        ///     |  /                    
        ///     |a/                     / = Distance
        ///     |/                      a = rotation
        ///     o -------------> 
        /// Translation Target
        /// </code>
        /// </para>
        /// </summary>
        (float distance, Vector3 rotation) OffsetDistanceRotation
        {
            get =>
                (
                    Offset.magnitude,
                    Quaternion
                        .FromToRotation(new Vector3(0f, 0f, Offset.magnitude), Offset)
                        .eulerAngles
            );
            set => Offset = Quaternion.Euler(value.rotation) * new Vector3(0f, 0f, value.distance);
        }

        /// <summary>
        /// Current position of this.
        /// </summary>
        Vector3 CurrentPosition
        {
            get
            {
                if (this is not MonoBehaviour mono) return Vector3.zero;
                return mono.transform.localPosition;
            }
        }

        /// <summary>
        /// Current rotation of this.
        /// </summary>
        Quaternion CurrentRotation
        {
            get
            {
                if (this is not MonoBehaviour mono) return Quaternion.identity;
                return mono.transform.localRotation;
            }
        }

        /// <summary>
        /// Current center of the current lazy rotation arc.
        /// </summary>
        Quaternion CurrentArcCenter { get; set; }




        /// <summary>
        /// Editor container for organizing purpose.<br/>
        /// <br/>
        /// With these components you can track the translation and rotation transform.
        /// </summary>
        [Serializable]
        public struct FollowTargetComponents
        {
            [Tooltip("Translation of the target.")]
            public Vector3 TranslationTarget;
        }

        /// <summary>
        /// Translation target.
        /// 
        /// <para>
        /// The actual position of the target will be <see cref="TranslationTarget"/> + <see cref="Offset"/>.
        /// </para>
        /// </summary>
        Vector3 TranslationTarget { get; set; }




        #region Translation

        /// <summary>
        /// Translate this toward <paramref name="translation"/> with an offset of <paramref name="offset"/> at <see cref="SmoothTranslationSpeed"/> speed.
        /// 
        /// <para>
        /// Set <see cref="TranslationTarget"/> with <paramref name="translation"/> and <see cref="Offset"/> with <paramref name="offset"/>.
        /// </para>
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="offset"></param>
        void Translate(Vector3 translation, Vector3 offset)
        {
            if (this is not MonoBehaviour mono) return;

            TranslationTarget = translation;
            Offset = offset;
            Translate();
        }

        /// <summary>
        /// Translate this toward <paramref name="translation"/> with an offset of <see cref="Offset"/> at <see cref="SmoothTranslationSpeed"/> speed.
        /// </summary>
        /// <param name="translation"></param>
        void Translate(Vector3 translation)
        {
            if (this is not MonoBehaviour mono) return;

            TranslationTarget = translation;
            Translate();
        }

        /// <summary>
        /// Translate this toward <see cref="TranslationTarget"/> + <see cref="Offset"/> at <see cref="SmoothTranslationSpeed"/> speed.
        /// <para>
        /// This method should be used in a monobehaviour's LateUpdate method.
        /// </para>
        /// </summary>
        void Translate()
        {
            if (this is not MonoBehaviour mono) return;

            mono.transform.localPosition = Vector3.Lerp
            (
                CurrentPosition,
                TranslationTarget + Offset,
                SmoothTranslationSpeed * Time.deltaTime
            );
        }

        /// <summary>
        /// Translate this toward <see cref="TranslationTarget"/> without delay.
        /// </summary>
        void TranslateImmediately()
        {
            if (this is not MonoBehaviour mono) return;

            mono.transform.localPosition = TranslationTarget + Offset;
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotate from <see cref="CurrentPosition"/> toward <paramref name="rotation"/>.<br/>
        /// <br/>
        /// If <paramref name="withAnimation"/> is true then the rotation animate at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// This method should be used in a monobehaviour's LateUpdate method if <paramref name="withAnimation"/> is true.<br/>
        /// <br/>
        /// The rotation is filtered with <paramref name="filter"/>. A filter of x = 0, y = 1, z = 0 means that the only allowed rotation is on the y axis.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="withAnimation"></param>
        /// <param name="filter"></param>
        void Rotate(Quaternion rotation, bool withAnimation = true, Vector3? filter = null)
        {
            if (this is not MonoBehaviour mono) return;

            if (filter.HasValue)
            {
                rotation.eulerAngles = Vector3.Scale(filter.Value, rotation.eulerAngles);
            }

            if (withAnimation)
            {
                mono.transform.localRotation = Quaternion.Lerp(
                    CurrentRotation,
                    rotation,
                    SmoothRotationSpeed * Time.deltaTime
                );
            }
            else
            {
                mono.transform.localRotation = rotation;
            }
        }

        /// <summary>
        /// Whether the <paramref name="rotation"/> is greater enough.<br/>
        /// <br/>
        /// If after the <paramref name="rotation"/> this object ends up in the current arc then it should not have rotate.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="arcPct"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool ShouldLazyRotate(ref Vector3 rotation, float arcPct, Vector3? filter = null)
        {
            if (filter.HasValue)
            {
                rotation = Vector3.Scale(filter.Value, rotation);
            }

            // Length of the arc.
            float arcLength = 360f * arcPct;
            // Half of the length of a arcLength.
            float halfArcLength = arcLength / 2f;

            bool shouldRotate = false;

            Vector3 currentSequenceCenter = CurrentArcCenter.eulerAngles;

            Vector3 min = new(
                -halfArcLength + currentSequenceCenter.x,
                -halfArcLength + currentSequenceCenter.y,
                -halfArcLength + currentSequenceCenter.z
            );

            Vector3 max = new(
                halfArcLength + currentSequenceCenter.x,
                halfArcLength + currentSequenceCenter.y,
                halfArcLength + currentSequenceCenter.z
            );

            Vector3 middle = new(
               currentSequenceCenter.x,
               currentSequenceCenter.y,
               currentSequenceCenter.z
            );

            for (int i = 0; i < 3; i++)
            {
                if (rotation[i] < min[i] || max[i] < rotation[i])
                {
                    shouldRotate = true;
                }
                else
                {
                    rotation[i] = currentSequenceCenter[i];
                }
            }

            return shouldRotate;
        }

        #endregion
    }
}