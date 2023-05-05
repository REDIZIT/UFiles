using UnityEngine;

public static class Extensions
{
    public static Sprite ToSprite(this Texture tex)
    {
        return Sprite.Create((Texture2D)tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f);
    }
    public static Color SetTransparency(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
    /// <summary>Replace '\' with '/'. To Unix standard.</summary>
    public static string NormalizePath(this string rawPath)
    {
        return rawPath.Replace(@"\", "/");
    }
    /// <summary>Replace '/' with '\'. To Win standard.</summary>
    public static string NormalizeWinPath(this string rawPath)
    {
        return rawPath.Replace("/", @"\");
    }
}
