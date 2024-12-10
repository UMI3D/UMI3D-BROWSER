using NUnit.Framework;
using System;
using umi3d.browserRuntime.worldController;

public class DateTimeExtensionsTests
{
    [Test]
    public void GivenTodo_WhenIsToday_ThenTrue()
    {
        DateTime date = DateTime.Today;

        bool result = date.IsToday();

        Assert.IsTrue(result);
    }

    [Test]
    public void GivenYesterday_WhenIsToday_ThenFalse()
    {
        DateTime date = DateTime.Now.AddDays(-1);

        bool result = date.IsToday();

        Assert.IsFalse(result);
    }

    [Test]
    public void GivenTomorrow_WhenIsToday_ThenFalse()
    {
        DateTime date = DateTime.Now.AddDays(1);

        bool result = date.IsToday();

        Assert.IsFalse(result);
    }

    [Test]
    public void GivenYesterday_WhenIsWithin7Days_ThenTrue()
    {
        DateTime date = DateTime.Now.AddDays(-1);

        bool result = date.IsWithin7Days();

        Assert.IsTrue(result);
    }

    [Test]
    public void GivenYesterday_WhenIsWithin30Days_ThenTrue()
    {
        DateTime date = DateTime.Now.AddDays(-1);

        bool result = date.IsWithin30Days();

        Assert.IsTrue(result);
    }

    [Test]
    public void Given30DaysAgo_WhenIsWithin30Days_ThenTrue()
    {
        DateTime date = DateTime.Now.AddDays(-30);

        bool result = date.IsWithin30Days();

        Assert.IsTrue(result);
    }

    [Test]
    public void Given31DaysAgo_WhenIsWithin30Days_ThenFalse()
    {
        DateTime date = DateTime.Now.AddDays(-31);

        bool result = date.IsWithin30Days();

        Assert.IsFalse(result);
    }

    [Test]
    public void Given31DaysAgo_WhenIsWithin365Days_ThenTrue()
    {
        DateTime date = DateTime.Now.AddDays(-31);

        bool result = date.IsWithin365Days();

        Assert.IsTrue(result);
    }

    [Test]
    public void Given365DaysAgo_WhenIsWithin365Days_ThenTrue()
    {
        DateTime date = DateTime.Now.AddDays(-365);

        bool result = date.IsWithin365Days();

        Assert.IsTrue(result);
    }

    [Test]
    public void Given366DaysAgo_WhenIsWithin365Days_ThenFalse()
    {
        DateTime date = DateTime.Now.AddDays(-366);

        bool result = date.IsWithin365Days();

        Assert.IsFalse(result);
    }

    [Test]
    public void Given366DaysAgo_WhenIsOlderThan365Days_ThenTrue()
    {
        DateTime date = DateTime.Now.AddDays(-366);

        bool result = date.IsOlderThan365Days();

        Assert.IsTrue(result);
    }
}
