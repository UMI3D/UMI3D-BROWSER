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
using NUnit.Framework;
using System.Numerics;
using umi3d.runtimeBrowser.filter;

public class CacheTests
{
    [Test]
    public void CreateCache()
    {
        var cache = new Cache<Vector3>();
        Assert.IsNotNull(cache);
    }

    [Test]
    public void AddOneToCache()
    {
        var cache = new Cache<Vector3>();
        cache.Add(new Vector3());
        Assert.AreEqual(1, cache.Count);
    }

    [Test]
    public void AddPastLimitcache()
    {
        var cache = new Cache<Vector3>(5);
        for (int i = 0; i < 10; i++)
        {
            cache.Add(new Vector3());
        }
        Assert.AreEqual(5, cache.Count);
    }

    [Test] 
    public void ClearCache()
    {
        var cache = new Cache<Vector3>();
        cache.Add(new Vector3());
        cache.Clear();
        Assert.AreEqual(0, cache.Count);
    }
}
