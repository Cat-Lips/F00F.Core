using Godot;

namespace F00F;

public static class TextureExtensions
{
    public static Image GetImageOrNull(this Texture2D source, out Vector2 size, out Vector2 offset)
    {
        //var image = source?.GetImage();
        var image = source.IsNull() ? null : RenderingServer.Texture2DGet(source.GetRid());
        if (image is null) { size = offset = default; return image; }
        if (image.IsCompressed()) image.Decompress();
        size = image.GetSize();
        offset = size * .5f;
        return image;
    }
}
