using System.Collections.Generic;
using UnityEngine;

namespace TaskManager
{
    internal enum Texture
    {
        Background,
        StockShelves,
        ManCounters,
        SetPrices
    }

    internal enum Style
    {
        Window,
        Title,
        Header,
        Button,
        StockShelvesButton,
        ManCountersButton,
        SetPricesButton
    }

    internal enum Layout
    {
        WidthLarge,
        WidthMedium,
        WidthSmall
    }

    public static class UI
    {
        private static readonly Dictionary<Style, GUIStyle> Styles = new Dictionary<Style, GUIStyle>();
        private static readonly Dictionary<Texture, Texture2D> Textures = new Dictionary<Texture, Texture2D>();
        private static readonly Dictionary<Layout, GUILayoutOption> Layouts = new Dictionary<Layout, GUILayoutOption>();
        private static bool _initialized;

        /**
         * Must be called within the OnGUI method.
         */
        public static void Init()
        {
            if (_initialized) return;

            _initialized = true;

            var startTime = Time.realtimeSinceStartupAsDouble;

            LoadTexture(Texture.Background, TextureUtil.LoadEmbeddedTexture("TaskManager.Assets.Background.png"));
            LoadTexture(Texture.StockShelves, TextureUtil.LoadEmbeddedTexture("TaskManager.Assets.Stock Shelves.png"));
            LoadTexture(Texture.ManCounters, TextureUtil.LoadEmbeddedTexture("TaskManager.Assets.Man Counters.png"));
            LoadTexture(Texture.SetPrices, TextureUtil.LoadEmbeddedTexture("TaskManager.Assets.Set Prices.png"));

            CreateStyle(Style.Window, new GUIStyle(GUI.skin.window)
            {
                normal = { background = GetTexture(Texture.Background) },
                onNormal = { background = GetTexture(Texture.Background) },
                active = { background = GetTexture(Texture.Background) },
                onActive = { background = GetTexture(Texture.Background) },

                border = new RectOffset(16, 16, 16, 16),
                padding = new RectOffset(12, 12, 28, 12),
                margin = new RectOffset(4, 4, 4, 4)
            });
            CreateStyle(Style.Title, new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold });
            CreateStyle(Style.Header, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            CreateStyle(Style.Button, new GUIStyle(GUI.skin.button) { fixedHeight = 36 });
            CreateStyle(Style.StockShelvesButton, new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 36,
                fixedHeight = 36,
                normal = { background = GetTexture(Texture.StockShelves) },
                hover = { background = GetTexture(Texture.StockShelves) },
                active = { background = GetTexture(Texture.StockShelves) },
                onNormal = { background = GetTexture(Texture.StockShelves) },
                onHover = { background = GetTexture(Texture.StockShelves) },
                onActive = { background = GetTexture(Texture.StockShelves) }
            });
            CreateStyle(Style.ManCountersButton, new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 36,
                fixedHeight = 36,
                normal = { background = GetTexture(Texture.ManCounters) },
                hover = { background = GetTexture(Texture.ManCounters) },
                active = { background = GetTexture(Texture.ManCounters) },
                onNormal = { background = GetTexture(Texture.ManCounters) },
                onHover = { background = GetTexture(Texture.ManCounters) },
                onActive = { background = GetTexture(Texture.ManCounters) }
            });
            CreateStyle(Style.SetPricesButton, new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 36,
                fixedHeight = 36,
                normal = { background = GetTexture(Texture.SetPrices) },
                hover = { background = GetTexture(Texture.SetPrices) },
                active = { background = GetTexture(Texture.SetPrices) },
                onNormal = { background = GetTexture(Texture.SetPrices) },
                onHover = { background = GetTexture(Texture.SetPrices) },
                onActive = { background = GetTexture(Texture.SetPrices) }
            });

            CreateLayout(Layout.WidthSmall, GUILayout.MinWidth(40));
            CreateLayout(Layout.WidthMedium, GUILayout.MinWidth(100));
            CreateLayout(Layout.WidthLarge, GUILayout.MinWidth(120));

            var endTime = Time.realtimeSinceStartupAsDouble;

            Debug.Log($"UI initialized in {(endTime - startTime) * 1000f} ms");
        }

        /**
         * Must be called within the OnGUI method.
         */
        public static void Unload()
        {
            foreach (var texture in Textures.Values)
            {
                Object.Destroy(texture);
            }
            Textures.Clear();
            Styles.Clear();
            Layouts.Clear();
            _initialized = false;
        }

        /**
         * Must be called within the OnGUI method.
         */
        internal static GUIStyle GetStyle(Style style)
        {
            return Styles[style];
        }

        /**
         * Must be called within the OnGUI method.
         */
        private static void CreateStyle(Style style, GUIStyle styleInstance)
        {
            Styles[style] = styleInstance;
        }

        /**
         * Must be called within the OnGUI method.
         */
        private static Texture2D GetTexture(Texture name)
        {
            return Textures[name];
        }

        /**
         * Must be called within the OnGUI method.
         */
        private static void LoadTexture(Texture name, Texture2D texture)
        {
            Textures[name] = texture;
        }

        /**
         * Must be called within the OnGUI method.
         */
        internal static GUILayoutOption GetLayout(Layout layout)
        {
            return Layouts[layout];
        }

        /**
         * Must be called within the OnGUI method.
         */
        private static void CreateLayout(Layout layout, GUILayoutOption layoutInstance)
        {
            Layouts[layout] = layoutInstance;
        }
    }
}