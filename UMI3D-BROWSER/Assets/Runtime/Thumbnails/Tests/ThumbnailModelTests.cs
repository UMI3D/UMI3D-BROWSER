using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.thumbnails;
using UnityEngine;
using UnityEngine.TestTools;

public class ThumbnailModelTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void GivenNoSprite_WhenSetImage_ThenSprite()
    {
        ThumbnailModel model = new ThumbnailModel();
        Assert.IsNull(model.image);

        Texture2D texture = new Texture2D(100, 100);
        Sprite sprite = Sprite.Create(texture, Rect.zero, Vector2.zero);
        model.SetImage(sprite);

        Assert.NotNull(model.image);
    }

    [Test]
    public void GivenNoFavorite_WhenUpdateFavorite_ThenFavorite()
    {
        ThumbnailModel model = new ThumbnailModel();
        Assert.IsFalse(model.isFavorite);

        model.UpdateFavoriteStatus(true);

        Assert.IsTrue(model.isFavorite);
    }

    [Test]
    public void GivenFavorite_WhenUpdateFavorite_ThenNotFavorite()
    {
        ThumbnailModel model = new ThumbnailModel();
        model.isFavorite = true;
        Assert.IsTrue(model.isFavorite);

        model.UpdateFavoriteStatus(false);

        Assert.IsFalse(model.isFavorite);
    }

    [Test]
    public void GivenNoFavorite_WhenToggleFavorite_ThenFavorite()
    {
        ThumbnailModel model = new ThumbnailModel();
        Assert.IsFalse(model.isFavorite);

        model.ToggleFavorite();

        Assert.IsTrue(model.isFavorite);
    }

    [Test]
    public void GivenFavorite_WhenToggleFavorite_ThenNoFavorite()
    {
        ThumbnailModel model = new ThumbnailModel();
        model.isFavorite = true;
        Assert.IsTrue(model.isFavorite);

        model.ToggleFavorite();

        Assert.IsFalse(model.isFavorite);
    }

    [Test]
    public void GivenSubInputsHide_WhenSetVisible_ThenVisible()
    {
        ThumbnailModel model = new ThumbnailModel();
        Assert.IsFalse(model.isDisplaying);

        model.SetSubInputsVisibility(true);

        Assert.IsTrue(model.isDisplaying);
    }

    [Test]
    public void GivenSubInputsDisplay_WhenSetHide_ThenHide()
    {
        ThumbnailModel model = new ThumbnailModel();
        model.isDisplaying = true;
        Assert.IsTrue(model.isDisplaying);

        model.SetSubInputsVisibility(false);

        Assert.IsFalse(model.isDisplaying);
    }

    [Test]
    public void GivenNoName_WhenUpdateName_ThenNamed()
    {
        ThumbnailModel model = new ThumbnailModel();
        Assert.IsTrue(string.IsNullOrEmpty(model.name));

        string name = "A gorgeous name";
        model.UpdateName(name);

        Assert.AreEqual(model.name, name);
    }
}
