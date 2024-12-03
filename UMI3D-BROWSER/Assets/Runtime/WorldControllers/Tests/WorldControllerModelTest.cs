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
        worldController1 = null;
        worldController2 = null;
        worldController3 = null;
        worldController4 = null;
        worldController5 = null;
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

    #region Favorites
    
    [Test]
    public void Given2FavoritesAnd3NonFavorites_WhenFilter_Then2()
    {
        model.data.worldControllers.Add(worldController1);
        model.data.worldControllers.Add(worldController2);
        model.data.worldControllers.Add(worldController3);
        model.data.worldControllers.Add(worldController4);
        model.data.worldControllers.Add(worldController5);

        List<WorldController> favorites = model.favorites.ToList();

        Assert.True(favorites.Contains(worldController1));
        Assert.True(favorites.Contains(worldController3));
        Assert.True(favorites.Count == 2);
    }

    #endregion

    #region Sorted by

    [Test]
    public void Given5WC_WhenSortByFirstConnection_ThenSorted()
    {
        model.data.worldControllers.Add(worldController1);
        model.data.worldControllers.Add(worldController2);
        model.data.worldControllers.Add(worldController3);
        model.data.worldControllers.Add(worldController4);
        model.data.worldControllers.Add(worldController5);

        List<WorldController> sort = model.sortByFirstConnection.ToList();

        Assert.True(sort.Count == 5);
        Assert.AreEqual(sort[0], worldController1);
        Assert.AreEqual(sort[1], worldController2);
        Assert.AreEqual(sort[2], worldController4);
        Assert.AreEqual(sort[3], worldController5);
        Assert.AreEqual(sort[4], worldController3);
    }

    [Test]
    public void Given5WC_WhenSortByLastConnection_ThenSorted()
    {
        model.data.worldControllers.Add(worldController1);
        model.data.worldControllers.Add(worldController2);
        model.data.worldControllers.Add(worldController3);
        model.data.worldControllers.Add(worldController4);
        model.data.worldControllers.Add(worldController5);

        List<WorldController> sort = model.sortByLastConnection.ToList();

        Assert.True(sort.Count == 5);
        Assert.AreEqual(sort[0], worldController3);
        Assert.AreEqual(sort[1], worldController2);
        Assert.AreEqual(sort[2], worldController4);
        Assert.AreEqual(sort[3], worldController5);
        Assert.AreEqual(sort[4], worldController1);
    }

    #endregion
}
