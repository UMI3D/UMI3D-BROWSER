/*
Copyright 2019 - 2022 Inetum

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
using System.Linq;
using UnityEngine;

namespace umi3dVRBrowsersBase.connection
{
    /// <summary>
    /// Class which all users' preferences.
    /// </summary>
    public static class PlayerPrefsManager
    {
        #region Ip

        public static readonly string Umi3dIp = "umi3d-ip";

        /// <summary>
        /// If an ip VirtualWorld is stored, returns it otherwise returns an empty string.
        /// </summary>
        /// <returns></returns>
        public static string GetUmi3dIp()
        {
            return PlayerPrefs.HasKey(Umi3dIp) 
                ? PlayerPrefs.GetString(Umi3dIp)
                : string.Empty;
        }

        /// <summary>
        /// Stores last environment ip used.
        /// </summary>
        /// <param name="ip"></param>
        public static void SaveUmi3dIp(string ip)
        {
            PlayerPrefs.SetString(Umi3dIp, ip);
        }

        #endregion

        #region Port

        public static readonly string Umi3dPort = "umi3d-port";

        /// <summary>
        /// If a port VirtualWorld is stored, returns it otherwise returns an empty string.
        /// </summary>
        /// <returns></returns>
        public static string GetUmi3DPort()
        {
            return PlayerPrefs.HasKey(Umi3dPort)
                ? PlayerPrefs.GetString(Umi3dPort)
                : string.Empty;
        }

        /// <summary>
        /// Stored last environment port used.
        /// </summary>
        /// <param name="port"></param>
        public static void SaveUmi3dPort(string port)
        {
            PlayerPrefs.SetString(Umi3dPort, port);
        }

        #endregion

        #region Virtual Worlds

        public static readonly string Umi3dVirtualWorlds = "umi3d-virtual-worlds";

        /// <summary>
        /// Returns true if there is <see cref="VirtualWorlds"/> stored.
        /// </summary>
        /// <returns></returns>
        public static bool HasVirtualWorldsStored()
        {
            return PlayerPrefs.HasKey(Umi3dVirtualWorlds);
        }

        /// <summary>
        /// Returns the <see cref="VirtualWorlds"/> stored.
        /// </summary>
        /// <returns></returns>
        public static VirtualWorlds GetVirtualWorlds()
        {
            return PlayerPrefs.HasKey(Umi3dVirtualWorlds)
                ? JsonUtility.FromJson<VirtualWorlds>(PlayerPrefs.GetString(Umi3dVirtualWorlds))
                : null;
        }

        /// <summary>
        /// Add a new VirtualWorld, if there is a VirtualWorld already stored with the same url update isFavorite and DateLastConnection.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        public static void SaveVirtualWorld(VirtualWorlds worlds)
        {
            PlayerPrefs.SetString(Umi3dVirtualWorlds, JsonUtility.ToJson(worlds));
            PlayerPrefs.Save();
        }

        #endregion


        /// <summary>
        /// Contains : environment name, ip and port.
        /// </summary>
        [System.Serializable]
        public class Data
        {
            public string environmentName;
            public string ip;
            public string port;

            public override string ToString() => $"name = {environmentName}, ip = {ip}, port = {port}";
        }
    }

    /// <summary>
    /// Stores data about a VirtualWorld.
    /// </summary>
    [System.Serializable]
    public class VirtualWorldData
    {
        public string worldName;
        public string worldUrl;

        public bool isFavorite;

        public string dateFirstConnection;
        public string dateLastConnection;
    }

    /// <summary>
    /// Stores all VirtualWorlds.
    /// </summary>
    [System.Serializable]
    public class VirtualWorlds
    {
        /// <summary>
        /// All the worlds.
        /// </summary>
        public List<VirtualWorldData> worlds = new();
        /// <summary>
        /// The list of favorite URLs.
        /// </summary>
        public List<string> favoriteURLs = new();

        /// <summary>
        /// The favorite worlds.
        /// </summary>
        public List<VirtualWorldData> FavoriteWorlds
        {
            get
            {
                if (worlds == null || worlds.Count == 0)
                {
                    return null;
                }
                else
                {
                    List<VirtualWorldData> result = new();
                    foreach (var url in favoriteURLs)
                    {
                        result.Add(worlds.Find(world => world.worldUrl == url));
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Whether or not the stored worlds contains <paramref name="world"/>.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public bool Contains(VirtualWorldData world)
        {
            return worlds.Find(_world => _world.worldUrl == world.worldUrl) != null;
        }

        public void AddWorld(VirtualWorldData world)
        {
            worlds.Add(world);
            PlayerPrefsManager.SaveVirtualWorld(this);
        }

        public void UpdateWorld(VirtualWorldData world)
        {
            var storedWorld = worlds.Find(_world => _world.worldUrl == world.worldUrl);
            if (storedWorld == null)
            {
                UnityEngine.Debug.LogError($"world is not stored.");
                return;
            }

            if (world.isFavorite && !storedWorld.isFavorite)
            {
                storedWorld.isFavorite = true;
                AddWorldToFavoriteWorlds(storedWorld);
            }

            storedWorld.dateLastConnection = world.dateLastConnection;
            PlayerPrefsManager.SaveVirtualWorld(this);
        }

        public void AddWorldToFavoriteWorlds(VirtualWorldData world, int index = -1)
        {
            UnityEngine.Debug.LogError($"here");
            if (index < 0)
            {
                index = favoriteURLs.Count;
            }
            favoriteURLs.Insert(index, world.worldUrl);
            PlayerPrefsManager.SaveVirtualWorld(this);
        }

        public void RemoveWorldFromFavoriteWorlds(VirtualWorldData world)
        {
            RemoveWorldFromFavoriteWorlds(world.worldUrl);
        }

        public void RemoveWorldFromFavoriteWorlds(string url)
        {
            worlds.Find(_world => _world.worldUrl == url).isFavorite = false;
            favoriteURLs.Remove(url);
            PlayerPrefsManager.SaveVirtualWorld(this);
        }

        public void DebugFavoriteListCreation()
        {
            foreach (var world in worlds) 
            {
                if (world.isFavorite && !favoriteURLs.Contains(world.worldUrl))
                {
                    favoriteURLs.Add(world.worldUrl);
                }
            }
        }
    }
}