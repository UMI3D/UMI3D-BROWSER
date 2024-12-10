using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using umi3d.browserRuntime.worldController;
using UnityEngine;
using UnityEngine.TestTools;

public class WorldControllerModelTest
{
    WorldControllersModel model;
    WorldController worldController1;
    WorldController worldController2;
    WorldController worldController3;
    WorldController worldController4;
    WorldController worldController5;

    [SetUp]
    public void SetUp()
    {
        model = new(isTesting: true);
        worldController1 = new()
        {
            url = "test/url1.com",
            name = "WC 1",
            isFavorite = true,
            firstConnection = DateTime.Parse("2024.01.01"),
            lastConnection = DateTime.Parse("2024.01.01")
        };
        worldController2 = new()
        {
            url = "test/url2.com",
            name = "WC 2",
            isFavorite = false,
            firstConnection = DateTime.Parse("2024.01.15"),
            lastConnection = DateTime.Today.AddDays(-10)
        };
        worldController3 = new()
        {
            url = "test/url3.com",
            name = "WC 3",
            isFavorite = true,
            firstConnection = DateTime.Parse("2024.12.01"),
            lastConnection = DateTime.Now
        };
        worldController4 = new()
        {
            url = "test/url4.com",
            name = "WC 4",
            isFavorite = false,
            firstConnection = DateTime.Parse("2024.03.01"),
            lastConnection = DateTime.Today.AddDays(-15)
        };
        worldController5 = new()
        {
            url = "test/url5.com",
            name = "WC 5",
            isFavorite = false,
            firstConnection = DateTime.Parse("2024.05.01"),
            lastConnection = DateTime.Today.AddMonths(-4)
        };
    }

    [TearDown]
    public void TearDown()
    {
        model = null;
    }

    #region Add

    [Test]
    public void Given0WC_WhenAdd_Then1()
    {
        model.Add(worldController1);

        Assert.True(model.data.worldControllers.Contains(worldController1));
    }

    [Test]
    public void Given1WC_WhenAddSame_Then1()
    {
        model.data.worldControllers.Add(worldController1);

        model.Add(worldController1);

        Assert.True(model.data.worldControllers.Contains(worldController1));
        Assert.True(model.data.worldControllers.Count == 1);
    }

    [Test]
    public void Given1WC_WhenAddSameUrlButOtherData_ThenNoChange()
    {
        model.data.worldControllers.Add(worldController1);
        string url = "test/url1.com";
        string name = "WC 1";
        bool isFavorite = true;
        DateTime firstConnection = DateTime.Parse("2024.01.01");
        DateTime lastConnection = DateTime.Parse("2024.01.01");
        Assert.AreEqual(model.data.worldControllers[0].url, url);
        Assert.AreEqual(model.data.worldControllers[0].name, name);
        Assert.AreEqual(model.data.worldControllers[0].isFavorite, isFavorite);
        Assert.AreEqual(model.data.worldControllers[0].firstConnection, firstConnection);
        Assert.AreEqual(model.data.worldControllers[0].lastConnection, lastConnection);

        WorldController copy = new()
        {
            url = worldController1.url,
            name = "Copy",
            isFavorite = false,
            firstConnection = DateTime.Now,
            lastConnection = DateTime.Now
        };
        model.Add(copy);

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers[0].url, url);
        Assert.AreEqual(model.data.worldControllers[0].name, name);
        Assert.AreEqual(model.data.worldControllers[0].isFavorite, isFavorite);
        Assert.AreEqual(model.data.worldControllers[0].firstConnection, firstConnection);
        Assert.AreEqual(model.data.worldControllers[0].lastConnection, lastConnection);
    }

    [Test]
    public void Given0WC_WhenAddWithHttp_Then1WithoutHttp()
    {
        WorldController worldController = new WorldController()
        {
            url = "http://test.com",
            name = "Test",
            isFavorite = true,
            firstConnection = DateTime.Now,
            lastConnection = DateTime.Now,
        };

        model.Add(worldController);

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers[0].url, "test.com");
        Assert.AreEqual(model.data.worldControllers[0].name, "Test");
        Assert.AreEqual(model.data.worldControllers[0].firstConnection, worldController.firstConnection);
        Assert.AreEqual(model.data.worldControllers[0].lastConnection, worldController.lastConnection);
    }

    [Test]
    public void Given0WC_WhenAddWithHttps_Then1WithoutHttps()
    {
        WorldController worldController = new WorldController()
        {
            url = "https://test.com",
            name = "Test",
            isFavorite = true,
            firstConnection = DateTime.Now,
            lastConnection = DateTime.Now,
        };

        model.Add(worldController);

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers[0].url, "test.com");
        Assert.AreEqual(model.data.worldControllers[0].name, "Test");
        Assert.AreEqual(model.data.worldControllers[0].firstConnection, worldController.firstConnection);
        Assert.AreEqual(model.data.worldControllers[0].lastConnection, worldController.lastConnection);
    }

    #endregion

    #region Remove

    [Test]
    public void Given1WC_WhenRemoveAnother_Then1()
    {
        model.data.worldControllers.Add(worldController1);

        model.Remove(worldController5);

        Assert.True(model.data.worldControllers.Contains(worldController1));
        Assert.True(model.data.worldControllers.Count == 1);
    }

    [Test]
    public void Given1WC_WhenRemove_Then0()
    {
        model.data.worldControllers.Add(worldController1);

        model.Remove(worldController1);

        Assert.False(model.data.worldControllers.Contains(worldController1));
        Assert.True(model.data.worldControllers.Count == 0);
    }

    #endregion

    #region Update

    [Test]
    public void Given1WC_WhenUpdate_Then1()
    {
        model.Add(worldController1);
        Assert.AreEqual(model.data.worldControllers[0].url, "test/url1.com");
        Assert.AreEqual(model.data.worldControllers[0].name, "WC 1");
        Assert.AreEqual(model.data.worldControllers[0].isFavorite, true);
        Assert.AreEqual(model.data.worldControllers[0].firstConnection, DateTime.Parse("2024.01.01"));
        Assert.AreEqual(model.data.worldControllers[0].lastConnection, DateTime.Parse("2024.01.01"));
        
        string name = "Copy";
        bool isFavorite = false;
        DateTime lastConnection = DateTime.Now;
        model.Update(worldController1.url, name, isFavorite, lastConnection);

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers[0].url, "test/url1.com");
        Assert.AreEqual(model.data.worldControllers[0].name, name);
        Assert.AreEqual(model.data.worldControllers[0].isFavorite, isFavorite);
        Assert.AreEqual(model.data.worldControllers[0].firstConnection, DateTime.Parse("2024.01.01"));
        Assert.AreEqual(model.data.worldControllers[0].lastConnection, lastConnection);
    }

    #endregion

    #region Connection Succeeded

    [Test]
    public void Given0WC_WhenConnectionSucceeded_Then1()
    {
        Assert.True(model.data.worldControllers.Count == 0);

        model.OnConnectionSucceeded("test.com", "Test");

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers.FindIndex(wc => wc.url == "test.com" && wc.name == "Test"), 0);
    }

    [Test]
    public void Given1WC_WhenConnectionSucceededWithSameUrl_Then1()
    {
        model.data.worldControllers.Add(worldController1);
        Assert.True(model.data.worldControllers.Count == 1);
        Assert.True(worldController1.name == "WC 1");

        model.OnConnectionSucceeded(worldController1.url, "Test");

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers.FindIndex(wc => wc.url == worldController1.url && wc.name == worldController1.name), 0);
    }

    [Test]
    public void Given0WC_WhenConnectionSucceededWithHttp_Then1WithoutHttp()
    {
        Assert.True(model.data.worldControllers.Count == 0);

        model.OnConnectionSucceeded("http://test.com", "Test");

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers.FindIndex(wc => wc.url == "test.com"), 0);
    }

    [Test]
    public void Given0WC_WhenConnectionSucceededWithHttps_Then1WithoutHttps()
    {
        Assert.True(model.data.worldControllers.Count == 0);

        model.OnConnectionSucceeded("https://test.com", "Test");

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers.FindIndex(wc => wc.url == "test.com"), 0);
    }

    [Test]
    public void Given1WC_WhenConnectionSucceededWithSameButHttp_Then1WithoutHttp()
    {
        model.Add(worldController1);

        model.OnConnectionSucceeded("http://test/url1.com", "Test");

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers.FindIndex(wc => wc.url == "test/url1.com"), 0);
    }

    [Test]
    public void Given1WC_WhenConnectionSucceededWithSameButHttps_Then1WithoutHttp()
    {
        model.Add(worldController1);

        model.OnConnectionSucceeded("https://test/url1.com", "Test");

        Assert.True(model.data.worldControllers.Count == 1);
        Assert.AreEqual(model.data.worldControllers.FindIndex(wc => wc.url == "test/url1.com"), 0);
    }

    #endregion
}
