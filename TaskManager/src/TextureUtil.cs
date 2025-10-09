using System.IO;
using System.Reflection;
using UnityEngine;

namespace TaskManager
{
    public static class TextureUtil
    {
        public static Texture2D LoadEmbeddedTexture(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();

            using (var stream = asm.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Debug.LogWarning("Resource not found: " + resourceName);
                    return FallbackTexture();
                }

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    var bytes = memoryStream.ToArray();
                    
                    var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    tex.LoadImage(bytes);
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.filterMode = FilterMode.Point;
                    tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    return tex;
                }
            }
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
}