using Godot;

namespace F00F;

public static class TextureExtensions
{
    public static Image GetImageOrNull(this Texture2D source)
    {
        var image = source is null ? null
            : RenderingServer.Texture2DGet(source.GetRid());
        if (image?.IsCompressed() is true)
            image.Decompress();
        return image;
    }

    public static Image GetImageOrNull(this Texture2D source, out Vector2 size)
    {
        var image = source.GetImageOrNull();
        size = image?.GetSize() ?? default;
        return image;
    }

    public static Image GetImageOrNull(this Texture2D source, out Vector2 size, out Vector2 offset)
    {
        var image = source.GetImageOrNull(out size);
        offset = size * .5f;
        return image;
    }
}
