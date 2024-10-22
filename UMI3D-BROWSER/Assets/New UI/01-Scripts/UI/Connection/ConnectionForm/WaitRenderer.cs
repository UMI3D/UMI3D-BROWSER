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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.interaction;
using umi3dBrowsers.displayer;
using umi3dBrowsers.linker;
using UnityEngine;
using UnityEngine.Localization;

namespace umi3dBrowsers.container.formrenderer
{
    public class WaitRenderer : MonoBehaviour
    {
        [Header("Displayers")]
        [SerializeField] private GameObject WaitPrefab;

        [Header("Roots")]
        [SerializeField] private TabManager tabManager;
        [SerializeField] private SimpleButton validationButton;
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;

        [SerializeField] private LocalizedString refreshingText;
        [SerializeField] private LocalizedString refreshingInText;

        /// <summary>
        /// List of callbacks for every parts of the form to build the answer dto
        /// </summary>
        private List<Action> formBinding = new();

        private Progress progress;
        private float awaitedTime;
        private WaitConnectionDto waitDto;
        private float timeCount;

        private Action OnFormAnswer;

        public void Init(GameObject contentRoot)
        {
            OnFormAnswer = connectionServiceLinker.SendWaitAnswer;
        }

        private void Update()
        {
            if(progress != null)
            {
                if(Time.time > awaitedTime)
                {
                    progress.SetAsCompleted();
                    progress.SetStatus(refreshingText.GetLocalizedString());
                    ValidateForm();
                    return;
                }

                timeCount += Time.deltaTime;
                float percent = (timeCount * 100 / waitDto.waitTimeSecond);
                progress.SetCompleted(percent);
                progress.SetStatus(refreshingInText.GetLocalizedString(Mathf.Max(waitDto.waitTimeSecond - timeCount, 0)));
            }
        }

        internal async void Handle(WaitConnectionDto connectionFormDto)
        {
#if UMI3D_PC
            if (connectionFormDto is WebConnectionDto web && web.connectionUrl != null)
                Application.OpenURL(web.connectionUrl);
#else
                UnityEngine.Debug.LogError("Imposible to open a web page with a WebConnectionDto on a non PC device");
#endif

            validationButton.gameObject.SetActive(false);
            validationButton.OnClick.RemoveAllListeners();

            GameObject container = tabManager.AddNewTabForParamForm(connectionFormDto.name ?? "", false);

            GameObject gameObject = Instantiate(WaitPrefab, container.transform);
            formBinding.Add(() =>
            {
                if (gameObject != null)
                    Destroy(gameObject);
            });

            WaitContainer displayer = gameObject.GetComponentInChildren<WaitContainer>();

            displayer.SetMessage(connectionFormDto.message);

            awaitedTime = Time.time + connectionFormDto.waitTimeSecond;
            waitDto = connectionFormDto;
            timeCount = 0;

            await Task.Yield();

            progress = new Progress(100);
            displayer.Open(progress);

            tabManager.InitSelectedButtonById(0);
        }

        [ContextMenu("Validate form ")]
        public void ValidateForm()
        {
            formBinding.ForEach(action => action?.Invoke());
            OnFormAnswer?.Invoke();
            CleanContent();
        }

        internal void CleanContent()
        {
            formBinding.Clear();
            tabManager.Clear();
            progress = null;
        }
    }
}

