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

using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Communication;
using VoltstroStudios.UnityWebBrowser.Core;
using VoltstroStudios.UnityWebBrowser.Helper;

namespace BrowserDesktop
{
    public class RuntimeWebBrowserBasic : RawImageUwbClientInputHandler
    {
        /// <summary>
        /// Default port buffer used if <see cref="FindFreeTcpPort"/> fails.
        /// </summary>
        static int basePort = 55000;

        void Awake()
        {
            TCPCommunicationLayer layer = ScriptableObject.CreateInstance<TCPCommunicationLayer>();

            layer.inPort = FindFreeTcpPort();
            layer.outPort = FindFreeTcpPort();

            browserClient.communicationLayer = layer;
            browserClient.CachePath = new(WebBrowserUtils.GetAdditionFilesDirectory() + "/cache-path/" + System.Guid.NewGuid().ToString());

            Debug.Log($"[ {nameof(RuntimeWebBrowserBasic)}] TCPCommunicationLayer created with ports {layer.inPort}; { layer.outPort } and cache path { browserClient.CachePath.FullName }");
        }

        /// <summary>
        /// Find a tcp port open.
        /// </summary>
        /// <returns></returns>
        static int FindFreeTcpPort()
        {
            int port;
            TcpListener l = new(IPAddress.Loopback, 0);
            try
            {
                l.Start();
                port = ((IPEndPoint)l.LocalEndpoint).Port;
            }
            catch (Exception e)
            {
                Debug.LogError("Error during webview tcp port generation." + e.Message);

                port = basePort++;
            }
            finally
            {
                l.Stop();
            }

            return port;
        }
    }
}