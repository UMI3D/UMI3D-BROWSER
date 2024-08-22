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
using UnityEngine;

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
        /// With these components you can track the translation and rotation transform.
        /// </summary>
        [Serializable]
        public struct FollowTargetComponents
        {
            [Tooltip("Translation of the target.")]
            public Vector3 TranslationTarget;

            [Tooltip("Rotation of the target.")]
            public Vector3 RotationTarget;
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
        public struct FollowRotationFilterComponents
        {
            [Tooltip("Rotation filter.")]
            public Vector3 Filter;
            [Tooltip("Rotation sequences count.")]
            public int Sequences;
        }


        /// <summary>
        /// The speed of this to translate toward <see cref="TranslationTarget"/>.
        /// </summary>
        float SmoothTranslationSpeed { get; set; }
        /// <summary>
        /// The speed of this to rotate toward <see cref="RotationTarget"/>.
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
        /// Translation target.
        /// 
        /// <para>
        /// The actual position of the target will be <see cref="TranslationTarget"/> + <see cref="Offset"/>.
        /// </para>
        /// </summary>
        Vector3 TranslationTarget { get; set; }
        /// <summary>
        /// Rotation target.
        /// </summary>
        Quaternion RotationTarget { get; set; }

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
        /// Rotate this by <paramref name="rotation"/> filtered with <paramref name="filter"/> if it matches the condition of the sequences.
        /// 
        /// <para>
        /// Divide a circle in <paramref name="sequences"/> sequences. Rotate toward the corresponding sequence.
        /// </para>
        /// <example>
        /// To rotate horizontally (Y axis) toward 4 positions, copy and past this piece of code.
        /// <code>
        ///     (this as IFollowable).Rotate(rotation, Vector3.up, 4);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="filter"></param>
        /// <param name="sequences"></param>
        void Rotate(Quaternion rotation, Vector3 filter, int sequences)
        {
            if (sequences <= 1)
            {
                // If 'sequences' is inferior to 1 then consider that there are no sequences.
                Rotate(rotation, filter);
                return;
            }

            // Length of a sequence.
            var sequence = 360f / (float)sequences;
            // Half of the length of a sequence.
            var halfSequence = sequence / 2f;

            var _rotation = Quaternion.identity;
            // Filter the rotation.
            _rotation.eulerAngles = Vector3.Scale(filter, rotation.eulerAngles);

            /// <summary>
            ///       |<- sequence->|
            /// ------|--]---0---]--|-------
            ///         min     max
            ///       |<->| = hysteresis
            /// </summary>
            void SetRotation(float angle, float current, Action<float> rotate, bool log)
            {
                // Portion of a sequence where the rotation ends up in the previous of next sequence.
                const float hysteresis = 20f;

                // The min value of the sequence. This value is excluded.
                var min = -halfSequence - hysteresis;
                // The max value of the sequence. This value is included.
                var max = halfSequence + hysteresis;
                // The middle of the sequence.
                var middle = 0f;

                // Loop over all the sequences.
                for (int i = 0; i < sequences; i++)
                {
                    if (log) UnityEngine.Debug.Log($"{i}: {min}, {max}, {angle}, {middle}");
                    if
                    (
                        angle <= max
                        && angle > min
                    )
                    {
                        // The rotation ends up in this sequence so rotate toward the 'middle' of the sequence.
                        // End the loop.
                        rotate(middle);
                        return;
                    }

                    // Increase the min, max and middle value to match the next sequence.
                    min += sequence;
                    max += sequence;
                    middle += sequence;
                }
                rotate(current);
            }

            SetRotation
            (
                _rotation.eulerAngles.x,
                RotationTarget.eulerAngles.x,
                middle =>
                {
                    _rotation.eulerAngles = new Vector3
                    (
                        middle,
                        _rotation.eulerAngles.y,
                        _rotation.eulerAngles.z
                    );
                }, false);

            SetRotation
            (
                _rotation.eulerAngles.y,
                RotationTarget.eulerAngles.y,
                middle =>
                {
                    _rotation.eulerAngles = new Vector3
                    (
                        _rotation.eulerAngles.x,
                        middle,
                        _rotation.eulerAngles.z
                    );
                }, false);

            SetRotation
            (
                _rotation.eulerAngles.z,
                RotationTarget.eulerAngles.z,
                middle =>
                {
                    _rotation.eulerAngles = new Vector3
                    (
                        _rotation.eulerAngles.x,
                        _rotation.eulerAngles.y,
                        middle
                    );
                }, false);

            Rotate(_rotation);
        }

        /// <summary>
        /// Rotate from <see cref="CurrentPosition"/> toward <paramref name="rotation"/> filtered with <paramref name="filter"/> at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// <br/>
        /// This method should be used in a monobehaviour's LateUpdate method.<br/>
        /// <br/>
        /// <example>
        /// To filtered the rotation on the Y axi copy and past the following piece of code:
        /// <code>
        ///     (this as IFollowable).Rotate(rotation, Vector3.up);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="filter">The filtering. If a coordinate equals 0 that means no rotation will be made around that axis.</param>
        void Rotate(Quaternion rotation, Vector3 filter)
        {
            var _rotation = Quaternion.identity;
            _rotation.eulerAngles = Vector3.Scale(filter, rotation.eulerAngles);

            Rotate(_rotation);
        }

        /// <summary>
        /// Rotate from <see cref="CurrentPosition"/> toward <paramref name="rotation"/> at <see cref="SmoothRotationSpeed"/> speed.<br/>
        /// <br/>
        /// This method should be used in a monobehaviour's LateUpdate method.
        /// </summary>
        /// <param name="rotation"></param>
        void Rotate(Quaternion rotation, bool withAnimation = true)
        {
            if (this is not MonoBehaviour mono) return;

            if (withAnimation)
            {
                mono.transform.localRotation = Quaternion.Lerp(
                    CurrentRotation, 
                    rotation, 
                    SmoothRotationSpeed * Time.deltaTime
                );
            } else
            {
                mono.transform.localRotation = RotationTarget;
            }
        }

        #endregion
    }
}