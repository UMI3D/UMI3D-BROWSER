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

namespace umi3d.browserEditor.BuildTool
{
    public enum E_Feature
    {
        Meta,
        Pico,
        Vive
    }

    public static class FeatureExt
    {
        internal const string FEATURE_META_QUEST = "com.unity.openxr.feature.metaquest";
        internal const string FEATURE_PICO_SUPPORT = "com.unity.openxr.feature.pico";
        internal const string FEATURE_PICO_OPENXR = "com.unity.openxr.pico.features";
        internal const string FEATURE_VIVE_SUPPORT = "com.unity.openxr.feature.vivefocus3";

        internal const string INPUT_OCULUS_TOUCH = "com.unity.openxr.feature.input.oculustouch";
        internal const string INPUT_METAQUEST_PRO = "com.unity.openxr.feature.input.metaquestpro";
        internal const string INPUT_PICO4_TOUCH = "com.unity.openxr.feature.input.PICO4touch";
        internal const string INPUT_PICONeo3_TOUCH = "com.unity.openxr.feature.input.PICONeo3touch";
        internal const string INPUT_VIVEFocus3 = "vive.wave.openxr.feature.focus3controller";

        /// <summary>
        /// This method returns an array of feature strings based on the provided E_Feature enum value.<br/>
        /// <br/>
        /// <example>
        /// Given an E_Feature enum value when calling GetFeatures then it returns the corresponding feature strings.<br/>
        /// <br/>
        /// <code>
        /// string[] result = E_Feature.Meta.GetFeatures();
        /// // result contains <see cref="FEATURE_META_QUEST"/>, <see cref="INPUT_METAQUEST_PRO"/>, <see cref="INPUT_OCULUS_TOUCH"/>.
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="features">The E_Feature enum value.</param>
        /// <returns>An array of feature strings corresponding to the provided E_Feature enum value.</returns>
        public static string[] GetFeatures(this E_Feature features)
        {
            return features switch
            {
                E_Feature.Meta => new[] 
                { 
                    FEATURE_META_QUEST, 
                    INPUT_METAQUEST_PRO, 
                    INPUT_OCULUS_TOUCH 
                },
                E_Feature.Pico => new[] 
                { 
                    FEATURE_PICO_OPENXR, 
                    FEATURE_PICO_SUPPORT, 
                    INPUT_PICO4_TOUCH, 
                    INPUT_PICONeo3_TOUCH
                },
                E_Feature.Vive => new[] 
                { 
                    FEATURE_VIVE_SUPPORT, 
                    INPUT_VIVEFocus3 
                },
                _ => null
            };
        }

        /// <summary>
        /// This method returns an array of feature strings for all E_Feature enum values except the provided one.<br/>
        /// <br/>
        /// <example>
        /// Given an E_Feature enum value when calling GetAllFeaturesExcept then it returns the feature strings for all other enum values.<br/>
        /// <br/>
        /// <code>
        /// string[] result = E_Feature.Meta.GetAllFeaturesExcept();
        /// // result contains all features except those of Meta.
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="feature">The E_Feature enum value to exclude.</param>
        /// <returns>An array of feature strings for all E_Feature enum values except the provided one.</returns>
        public static string[] GetAllFeaturesExcept(this E_Feature feature)
        {
            List<string> features = new();
            switch (feature)
            {
                case E_Feature.Meta:
                    features.AddRange(E_Feature.Pico.GetFeatures());
                    features.AddRange(E_Feature.Vive.GetFeatures());
                    break;

                case E_Feature.Pico:
                    features.AddRange(E_Feature.Meta.GetFeatures());
                    features.AddRange(E_Feature.Vive.GetFeatures());
                    break;

                case E_Feature.Vive:
                    features.AddRange(E_Feature.Pico.GetFeatures());
                    features.AddRange(E_Feature.Meta.GetFeatures());
                    break;

                default:
                    break;
            }

            return features.ToArray();
        }
    }
}
