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

public class PopupManager : MonoBehaviour
{
    [Header("Pop up list")]
    [SerializeField] GameObject closeApplication;
    [SerializeField] GameObject connectionToServer;
    [SerializeField] GameObject deleteLibrary;
    [SerializeField] GameObject downloadLibrary;
    [SerializeField] GameObject error;
    [SerializeField] GameObject failedToConnect;
    [SerializeField] GameObject reportBug;
    [SerializeField] GameObject tookLongResponse;
    [SerializeField] GameObject unableDeleteLibrary;
    [SerializeField] GameObject worldNotRespond;

    public PopupType activePopup;

    public enum PopupType
    {
        CloseApplication,
        ConnectionToServer,
        DeleteLibrary,
        DownloadLibrary,
        Error,
        FailedToConnect,
        ReportBug,
        TookLongResponse,
        UnableDeleteLibrary,
        WorldNotRespond
    }

    public void ShowPopup(PopupType type)
    {
        activePopup = type;
        switch (type)
        {
            case PopupType.CloseApplication:
                ActivatePopup(closeApplication);
                break;
            case PopupType.ConnectionToServer:
                ActivatePopup(connectionToServer);
                break;
            case PopupType.DeleteLibrary:
                ActivatePopup(deleteLibrary);
                break;
            case PopupType.DownloadLibrary:
                ActivatePopup(downloadLibrary);
                break;
            case PopupType.Error:
                ActivatePopup(error);
                break;
            case PopupType.FailedToConnect:
                ActivatePopup(failedToConnect);
                break;
            case PopupType.ReportBug:
                ActivatePopup(reportBug);
                break;
            case PopupType.TookLongResponse:
                ActivatePopup(tookLongResponse);
                break;
            case PopupType.UnableDeleteLibrary:
                ActivatePopup(unableDeleteLibrary);
                break;
            case PopupType.WorldNotRespond:
                ActivatePopup(worldNotRespond);
                break;
            default:
                Debug.LogWarning("Unknown popup type: " + type);
                break;
        }
    }

    private void ActivatePopup(GameObject popup)
    {
        GameObject[] allPopups = { closeApplication, connectionToServer, deleteLibrary,
                                downloadLibrary, error, failedToConnect, reportBug,
                                tookLongResponse, unableDeleteLibrary, worldNotRespond };

        foreach (GameObject p in allPopups)
        {
            p.SetActive(false);
        }

        popup.SetActive(true);
    }

}
