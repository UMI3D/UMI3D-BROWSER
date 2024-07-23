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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.common;
using umi3d.common.interaction.form;
using UnityEngine;

namespace form_generator
{
    public class DivTypeTagger : MonoBehaviour
    {
        [Header("DivInfo")]
        [SerializeField] private string name;
        [SerializeField, TextArea] private string toolTip;
        [SerializeField, TextArea] private string description;
        [SerializeField] private DivType divType;
        public DivType DivType => divType;
        public string Name => name;
        public string ToolTip => toolTip;
        public string Description => description;

        /// <summary>
        /// For tool execution only
        /// </summary>
        [SerializeField] private List<DivTypeTagger> children = new();
        public List<DivTypeTagger> Children => children;

        [Header("Resources")]
        [SerializeField] List<Resource> resources = new();

        public ResourceDto GetResourceDto()
        {
            if (resources.Count == 0) return null;
            Debug.Log($"<color=orange>Well if this debug message stills there and you are trying to use this tool to gen some DivForm on the server side its not going to work," +
                $"the way the resourceDTO is handle here is just for the sake of testing nothing else, you have to handle it like for the variants of the " +
                $"UMI3DModel</color>");

            ResourceDto resourceDto = new ResourceDto();
            foreach (var res in resources)
            {
                resourceDto.variants.Add(CreateFileDto(res));
            }
            return resourceDto;
        }

        private FileDto CreateFileDto(Resource resource)
        {
            var dto = new FileDto
            {
                url = GetUrl(resource.path),
                format = resource.format,
                extension = resource.extension,
                metrics = new AssetMetricDto()
                {
                    resolution = resource.resolution,
                    size = resource.size,
                },
                pathIfInBundle = resource.isInBundle ? resource.pathIfInBundle : null,
                libraryKey = null
            };
            return dto;
        }

        public string GetUrl(string path)
        {
            path = path.Replace(@"\", "/");
            if (path != null && path != "" && !path.StartsWith("/") /*|| Path.StartsWith(@"\")*/)
            {
                path = "/" + path;
            }
            return System.Uri.EscapeUriString(Path.Combine(path));
        }

        internal List<StyleDto> GetStyles()
        {
            List<StyleDto> styleDtos = new();

            List<DivTypeStyler> styles = GetComponents<DivTypeStyler>().ToList();
            if (styles.Count()  == 0)
            {
                DivTypeStyler style = gameObject.AddComponent<DivTypeStyler>();
                styles.Add(style);
            }

            for (int i = 0; i< styles.Count(); i++)
            {
                styleDtos.Add(styles[i].GetStyle());
            }

            return styleDtos;
        }

        [Serializable]
        public class Resource
        {
            public bool isLocalFile = false;
            public string domain = "";
            public string path = "";
            [ConstEnum(typeof(UMI3DAssetFormat), typeof(string))]
            public string format;
            public string extension;
            public bool isInBundle = false;
            public string pathIfInBundle = "";

            /// <summary>
            /// Arbitrary level of resolution from low to higher resolution.
            /// </summary>
            public int resolution = 1;
            /// <summary>
            /// File size in Mb.
            /// </summary>
            public float size  = 0f;
        }
    }

    public enum DivType
    {
        Form, Page, Group, Button, Enum, Image, Range, Input_Text, Label
    }
}

