using NUnit.Framework;
using umi3d.browserRuntime.ui.thumbnails;
using UnityEngine;

public class ThumbnailModelTests
{
    ThumbnailModel model;

    [SetUp]
    public void SetUp()
    {
        model = new ThumbnailModel();
    }

    [TearDown]
    public void TearDown()
    {
        model = null;
    }


    [Test]
    public void GivenNoSprite_WhenSetImage_ThenSprite()
    {
        Assert.IsNull(model.image);

        Texture2D texture = new Texture2D(100, 100);
        Sprite sprite = Sprite.Create(texture, Rect.zero, Vector2.zero);
        model.SetImage(sprite);

        Assert.NotNull(model.image);
    }

    [Test]
    public void GivenNoFavorite_WhenUpdateFavorite_ThenFavorite()
    {
        Assert.IsFalse(model.isFavorite);

        model.SetFavoriteStatus(true);

        Assert.IsTrue(model.isFavorite);
    }

    [Test]
    public void GivenFavorite_WhenUpdateFavorite_ThenNotFavorite()
    {
        model.isFavorite = true;
        Assert.IsTrue(model.isFavorite);

        model.SetFavoriteStatus(false);

        Assert.IsFalse(model.isFavorite);
    }

    [Test]
    public void GivenNoFavorite_WhenToggleFavorite_ThenFavorite()
    {
        Assert.IsFalse(model.isFavorite);

        model.ToggleFavorite();

        Assert.IsTrue(model.isFavorite);
    }

    [Test]
    public void GivenFavorite_WhenToggleFavorite_ThenNoFavorite()
    {
        model.isFavorite = true;
        Assert.IsTrue(model.isFavorite);

        model.ToggleFavorite();

        Assert.IsFalse(model.isFavorite);
    }

    [Test]
    public void GivenSubInputsHide_WhenSetVisible_ThenVisible()
    {
        Assert.IsFalse(model.areSubInputsVisible);

        model.SetSubInputsVisibility(true);

        Assert.IsTrue(model.areSubInputsVisible);
    }

    [Test]
    public void GivenSubInputsDisplay_WhenSetHide_ThenHide()
    {
        model.areSubInputsVisible = true;
        Assert.IsTrue(model.areSubInputsVisible);

        model.SetSubInputsVisibility(false);

        Assert.IsFalse(model.areSubInputsVisible);
    }

    [Test]
    public void GivenNoName_WhenUpdateName_ThenNamed()
    {
        Assert.IsTrue(string.IsNullOrEmpty(model.name));

        string name = "A gorgeous name";
        model.UpdateName(name);

        Assert.AreEqual(model.name, name);
    }
}
