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

namespace umi3d.browserRuntime.ui
{
    public class KeyDepthController : MonoBehaviour
    {
        [SerializeField] float speed = 10f;
        [SerializeField] float keyDepth = 40f;

        Vector3 buttonBasePosition;
        Vector3 buttonDownPosition;
        RectTransform rectTransform;

        void Awake()
        {
            buttonBasePosition = gameObject.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition3D;
            buttonDownPosition = buttonBasePosition + new Vector3(0, 0, keyDepth);
            rectTransform = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        }

        void Update()
        {
            //if (buttonPressed)
            //{
            //    if (button.interactable)
            //    {
            //        if (rectTransform.anchoredPosition3D.z < buttonDownPosition.z)
            //        {
            //            rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, buttonDownPosition, Time.deltaTime * speed);

            //        }
            //    }
            //}
            //else
            //{
            //    if (rectTransform.anchoredPosition3D.z > buttonBasePosition.z)
            //    {
            //        rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, buttonBasePosition, Time.deltaTime * speed);

            //    }
            //}
        }
    }
}