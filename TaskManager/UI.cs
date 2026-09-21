using System.Collections.Generic;
using UnityEngine;

namespace TaskManager;

internal enum Texture
{
    Background,
    Buttons
}

internal enum Style
{
    Window,
    Title,
    Header,
    Label,
    StockShelvesButton,
    StockShelvesButtonToggled,
    ManCountersButton,
    ManCountersButtonToggled,
    SetPricesButton,
    SetPricesButtonToggled,
    RefillScentButton,
    RefillScentButtonToggled,
    PackMachineButton,
    PackMachineButtonToggled,
    RestockCardsButton,
    RestockCardsButtonToggled,
    RestButton,
    RestButtonToggled,
    Button,
    ButtonToggled,
    ButtonText,
    TextShadow
}

internal enum Layout
{
    WidthLarge,
    WidthSemiLarge,
    WidthMedium,
    WidthSmall,
    WidthTiny,
    TinyBox
}

internal static class UI
{
    private const uint TaskButtonSize = 36;

    private static readonly Dictionary<Style, GUIStyle> Styles = new();
    private static readonly Dictionary<Texture, Texture2D> Textures = new();
    private static readonly Dictionary<Layout, GUILayoutOption[]> Layouts = new();

    private static readonly GUIStyle TaskButtonStyle = new(GUI.skin.button)
        { fixedWidth = TaskButtonSize, fixedHeight = TaskButtonSize };

    private static Texture2D _whiteTexture;
    private static bool _initialized;

    public static void Init()
    {
        if (_initialized) return;

        _initialized = true;

        var startTime = Time.realtimeSinceStartupAsDouble;

        _whiteTexture = new Texture2D(1, 1);
        _whiteTexture.SetPixel(0, 0, Color.white);
        _whiteTexture.Apply();

        LoadTexture(Texture.Background, TextureUtil.LoadEmbeddedTexture("TaskManager.Assets.Background.png"));
        LoadTexture(Texture.Buttons, TextureUtil.LoadEmbeddedTexture("TaskManager.Assets.Buttons.png"));

        CreateStyle(Style.Window, new GUIStyle(GUI.skin.window)
        {
            normal = { background = GetTexture(Texture.Background), textColor = Color.white },
            hover = { background = GetTexture(Texture.Background), textColor = Color.white },
            active = { background = GetTexture(Texture.Background), textColor = Color.white },
            onNormal = { background = GetTexture(Texture.Background), textColor = Color.white },
            onHover = { background = GetTexture(Texture.Background), textColor = Color.white },
            onActive = { background = GetTexture(Texture.Background), textColor = Color.white },
            border = new RectOffset(16, 16, 16, 16),
            padding = new RectOffset(12, 12, 28, 12),
            margin = new RectOffset(4, 4, 4, 4)
        });
        CreateStyle(Style.Title, new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold });
        CreateStyle(Style.Header,
            new GUIStyle(GUI.skin.label)
                { fixedHeight = TaskButtonSize, alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold });
        CreateStyle(Style.Label,
            new GUIStyle(GUI.skin.label) { fixedHeight = TaskButtonSize, alignment = TextAnchor.MiddleLeft });

        CreateButtonStyle(Style.StockShelvesButton, (0, 0), (1, 0), (2, 0));
        CreateButtonStyle(Style.StockShelvesButtonToggled, (3, 0), (4, 0), (5, 0));

        CreateButtonStyle(Style.ManCountersButton, (0, 1), (1, 1), (2, 1));
        CreateButtonStyle(Style.ManCountersButtonToggled, (3, 1), (4, 1), (5, 1));

        CreateButtonStyle(Style.SetPricesButton, (0, 2), (1, 2), (2, 2));
        CreateButtonStyle(Style.SetPricesButtonToggled, (3, 2), (4, 2), (5, 2));

        CreateButtonStyle(Style.RefillScentButton, (0, 3), (1, 3), (2, 3));
        CreateButtonStyle(Style.RefillScentButtonToggled, (3, 3), (4, 3), (5, 3));

        CreateButtonStyle(Style.PackMachineButton, (0, 4), (1, 4), (2, 4));
        CreateButtonStyle(Style.PackMachineButtonToggled, (3, 4), (4, 4), (5, 4));

        CreateButtonStyle(Style.RestockCardsButton, (0, 5), (1, 5), (2, 5));
        CreateButtonStyle(Style.RestockCardsButtonToggled, (3, 5), (4, 5), (5, 5));

        CreateButtonStyle(Style.RestButton, (0, 6), (1, 6), (2, 6));
        CreateButtonStyle(Style.RestButtonToggled, (3, 6), (4, 6), (5, 6));

        CreateStyle(Style.Button, new GUIStyle(GUI.skin.button)
        {
            fixedHeight = TaskButtonSize,
            fontStyle = FontStyle.Bold,
            normal = { background = GetButtonTexture(0, 7) },
            hover = { background = GetButtonTexture(1, 7) },
            active = { background = GetButtonTexture(2, 7) }
        });
        CreateStyle(Style.ButtonToggled, new GUIStyle(GUI.skin.button)
        {
            fixedHeight = TaskButtonSize,
            fontStyle = FontStyle.Bold,
            normal = { background = GetButtonTexture(3, 7) },
            hover = { background = GetButtonTexture(4, 7) },
            active = { background = GetButtonTexture(5, 7) }
        });

        CreateStyle(Style.ButtonText, new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        });

        CreateStyle(Style.TextShadow, new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0f, 0f, 0f, 0.15f) }
        });

        CreateLayout(Layout.WidthTiny, GUILayout.MinWidth(40));
        CreateLayout(Layout.WidthSmall, GUILayout.MinWidth(75));
        CreateLayout(Layout.WidthMedium, GUILayout.MinWidth(110));
        CreateLayout(Layout.WidthSemiLarge, GUILayout.MinWidth(150));
        CreateLayout(Layout.WidthLarge, GUILayout.MinWidth(300));
        CreateLayout(Layout.TinyBox, GUILayout.Width(TaskButtonSize), GUILayout.Height(TaskButtonSize));

        var endTime = Time.realtimeSinceStartupAsDouble;

        Debug.Log($"UI initialized in {(endTime - startTime) * 1000f} ms");
    }

    internal static void HorizontalSeparator(
        Color color,
        float thickness = 1f,
        float margin = 4f)
    {
        GUILayout.Space(margin);

        var rect = GUILayoutUtility.GetRect(
            0f,
            thickness,
            GUILayout.Height(thickness),
            GUILayout.ExpandWidth(true)
        );

        var oldColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, _whiteTexture);
        GUI.color = oldColor;

        GUILayout.Space(margin);
    }

    internal static bool TextShadowButton(
        string label,
        GUIStyle style,
        params GUILayoutOption[] options)
    {
        var content = new GUIContent(label);
        var rect = GUILayoutUtility.GetRect(content, style, options);

        var clicked = GUI.Button(rect, GUIContent.none, style);

        var shadowStyle = GetStyle(Style.TextShadow);
        var textStyle = GetStyle(Style.ButtonText);

        DrawBlurredShadow(rect, label, shadowStyle, 1f);

        GUI.Label(rect, label, textStyle);

        return clicked;
    }

    private static void DrawBlurredShadow(
        Rect rect,
        string text,
        GUIStyle style,
        float radius)
    {
        var offsets = new[]
        {
            new Vector2(-radius, 0),
            new Vector2(radius, 0),
            new Vector2(0, -radius),
            new Vector2(0, radius),
            new Vector2(-radius, -radius),
            new Vector2(radius, -radius),
            new Vector2(-radius, radius),
            new Vector2(radius, radius)
        };

        foreach (var offset in offsets)
        {
            var shadowRect = rect;
            shadowRect.position += offset;
            GUI.Label(shadowRect, text, style);
        }
    }

    internal static void Unload()
    {
        foreach (var texture in Textures.Values) Object.Destroy(texture);
        Textures.Clear();
        Styles.Clear();
        Layouts.Clear();
        _initialized = false;
    }

    private static Texture2D GetButtonTexture(uint x, uint y)
    {
        return TextureUtil.CreateTextureFromAtlas(
            GetTexture(Texture.Buttons),
            new Rect(x * TaskButtonSize, y * TaskButtonSize, TaskButtonSize, TaskButtonSize)
        );
    }

    private static void CreateButtonStyle(Style style, (uint x, uint y) normal, (uint x, uint y) hover,
        (uint x, uint y) active)
    {
        CreateStyle(style, new GUIStyle(TaskButtonStyle)
        {
            normal = { background = GetButtonTexture(normal.x, normal.y) },
            hover = { background = GetButtonTexture(hover.x, hover.y) },
            active = { background = GetButtonTexture(active.x, active.y) }
        });
    }

    internal static GUIStyle GetStyle(Style style)
    {
        return Styles[style];
    }

    private static void CreateStyle(Style style, GUIStyle styleInstance)
    {
        Styles[style] = styleInstance;
    }

    private static Texture2D GetTexture(Texture name)
    {
        return Textures[name];
    }

    private static void LoadTexture(Texture name, Texture2D texture)
    {
        Textures[name] = texture;
    }

    internal static GUILayoutOption[] GetLayout(Layout layout)
    {
        return Layouts[layout];
    }

    private static void CreateLayout(Layout layout, params GUILayoutOption[] options)
    {
        Layouts[layout] = options;
    }
}