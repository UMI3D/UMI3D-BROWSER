using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.worldController;
using UnityEngine;
using UnityEngine.TestTools;

public class WorldControllersFilteredTests
{
    WorldControllersFiltered worldControllersFiltered;
    List<WorldController> worldControllers;
    WorldController worldController1;
    WorldController worldController2;
    WorldController worldController3;
    WorldController worldController4;
    WorldController worldController5;

    [SetUp]
    public void SetUp()
    {
        worldController1 = new()
        {
            url = "test/url1.com",
            name = "Favorite, Today, Today",
            isFavorite = true,
            firstConnection = DateTime.Today,
            lastConnection = DateTime.Today
        };
        worldController2 = new()
        {
            url = "test/url2.com",
            name = "Not Favorite, 3D, Today",
            isFavorite = false,
            firstConnection = DateTime.Today.AddDays(-3),
            lastConnection = DateTime.Today
        };
        worldController3 = new()
        {
            url = "test/url3.com",
            name = "Favorite, 41D, 25D",
            isFavorite = true,
            firstConnection = DateTime.Today.AddDays(-41),
            lastConnection = DateTime.Today.AddDays(-25)
        };
        worldController4 = new()
        {
            url = "test/url4.com",
            name = "Not Favorite, 15D, 15D",
            isFavorite = false,
            firstConnection = DateTime.Today.AddDays(-15),
            lastConnection = DateTime.Today.AddDays(-15)
        };
        worldController5 = new()
        {
            url = "test/url5.com",
            name = "Not Favorite, 410D, 255D",
            isFavorite = false,
            firstConnection = DateTime.Today.AddDays(-410),
            lastConnection = DateTime.Today.AddMonths(-255)
        };
        worldControllers = new()
        {
            worldController1, worldController2, worldController3, worldController4, worldController5
        };
        worldControllersFiltered = new(worldControllers);
    }

    [TearDown]
    public void TearDown()
    {
        worldControllersFiltered = null;
        worldControllers.Clear();
        worldControllers = null;
    }

    [Test]
    public void GivenNoFilters_WhenGetFiltered_ThenSameList()
    {
        Assert.IsNull(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.None);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, worldControllers.Count);
        Assert.AreEqual(filtered.Count, 5);
        Assert.AreEqual(filtered[0], worldController1);
        Assert.AreEqual(filtered[1], worldController2);
        Assert.AreEqual(filtered[2], worldController3);
        Assert.AreEqual(filtered[3], worldController4);
        Assert.AreEqual(filtered[4], worldController5);
    }

    [Test]
    public void GivenIsFavorite_WhenGetFiltered_ThenOnlyFavorite()
    {
        worldControllersFiltered.isFavorite = true;
        Assert.IsTrue(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.None);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 2);
        Assert.AreEqual(filtered[0], worldController1);
        Assert.AreEqual(filtered[1], worldController3);
    }

    [Test]
    public void GivenIsNotFavorite_WhenGetFiltered_ThenAllExceptFavorite()
    {
        worldControllersFiltered.isFavorite = false;
        Assert.IsFalse(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.None);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 3);
        Assert.AreEqual(filtered[0], worldController2);
        Assert.AreEqual(filtered[1], worldController4);
        Assert.AreEqual(filtered[2], worldController5);
    }

    [Test]
    public void GivenFirstConnectionToday_WhenGetFiltered_ThenOnlyToday()
    {
        worldControllersFiltered.firstConnectionPeriod = Period.Today;
        Assert.IsNull(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.Today);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 1);
        Assert.AreEqual(filtered[0], worldController1);
    }

    [Test]
    public void GivenFirstConnection7Days_WhenGetFiltered_ThenOnly7Days()
    {
        worldControllersFiltered.firstConnectionPeriod = Period.Within7Days;
        Assert.IsNull(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.Within7Days);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 1);
        Assert.AreEqual(filtered[0], worldController2);
    }

    [Test]
    public void GivenFirstConnection30Days_WhenGetFiltered_ThenOnly30Days()
    {
        worldControllersFiltered.firstConnectionPeriod = Period.Within30Days;
        Assert.IsNull(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.Within30Days);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 1);
        Assert.AreEqual(filtered[0], worldController4);
    }

    [Test]
    public void GivenFirstConnection365Days_WhenGetFiltered_ThenOnly365Days()
    {
        worldControllersFiltered.firstConnectionPeriod = Period.Within365Days;
        Assert.IsNull(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.Within365Days);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 1);
        Assert.AreEqual(filtered[0], worldController3);
    }

    [Test]
    public void GivenFirstConnectionOlderThan365Days_WhenGetFiltered_ThenOnlyOlderThan365Days()
    {
        worldControllersFiltered.firstConnectionPeriod = Period.OlderThan365Days;
        Assert.IsNull(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.OlderThan365Days);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.None);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 1);
        Assert.AreEqual(filtered[0], worldController5);
    }

    [Test]
    public void GivenFavoriteAndLastConnectionToday_WhenGetFiltered_ThenOnlyFavoriteAndToday()
    {
        worldControllersFiltered.isFavorite = true;
        worldControllersFiltered.lastConnectionPeriod = Period.Today;
        Assert.IsTrue(worldControllersFiltered.isFavorite);
        Assert.AreEqual(worldControllersFiltered.firstConnectionPeriod, Period.None);
        Assert.AreEqual(worldControllersFiltered.lastConnectionPeriod, Period.Today);

        List<WorldController> filtered = worldControllersFiltered.filteredList;

        Assert.AreEqual(filtered.Count, 1);
        Assert.AreEqual(filtered[0], worldController1);
    }
}
