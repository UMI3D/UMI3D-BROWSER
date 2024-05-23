using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.common.interaction.form;
using UnityEngine;

public static class DivFormExtensionHelper
{
    public async static Task<Sprite> GetSprite(this ImageDto imageDto)
    {
        Sprite sprite = null;
        if (imageDto.resource != null)
            try
            {
                object spriteTask = await UMI3DResourcesManager.Instance._LoadFile(0,
                    imageDto.resource.variants[0],
                    new ImageDtoLoader()
                );

                Texture2D texture = spriteTask as Texture2D;
                sprite = Sprite.Create(texture,
                                    new Rect(0, 0, texture.Size().x, texture.Size().y),
                                    new Vector2());
            }
            catch (Exception ex)
            {
                Debug.LogException(new Exception("Make sure you are in play mode to load resource in the form," +
                    " or that every networking UMI3D behaviours are ready"));
            }

        return sprite;
    }

    public static Vector2 Size(this Texture2D texture)
    {
        return new Vector2(texture.width, texture.height);
    }
}
