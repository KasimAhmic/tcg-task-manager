using System.IO;
using System.Reflection;
using UnityEngine;

namespace TaskManager;

internal static class TextureUtil
{
    internal static Texture2D LoadEmbeddedTexture(string resourceName)
    {
        var asm = Assembly.GetExecutingAssembly();

        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            Debug.LogWarning("Resource not found: " + resourceName);
            return FallbackTexture();
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
        return tex;
    }

    internal static Texture2D CreateTextureFromAtlas(Texture2D atlas, Rect rect)
    {
        var tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        tex.SetPixels(atlas.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
        return tex;
    }

    private static Texture2D FallbackTexture()
    {
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        var col = new Color(0f, 0f, 0f, 0.75f);
        tex.SetPixels(new[] { col, col, col, col });
        tex.Apply();
        return tex;
    }
}