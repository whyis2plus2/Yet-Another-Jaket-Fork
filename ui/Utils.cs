namespace Jaket.UI;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

using Jaket.Assets;
using Jaket.Content;

public class Utils
{
    private static Sprite buttonImage, shadowImage, circleShadowImage, circleImage;
    private static ColorBlock colorBlock;

    public static void Load()
    {
        // TODO cringe
        buttonImage = OptionsMenuToManager.Instance.pauseMenu.transform.Find("Continue").GetComponent<Image>().sprite;
        shadowImage = Sandbox.SandboxAlterMenu.Instance.transform.Find("Shadow").GetComponent<Image>().sprite;
        circleShadowImage = WeaponWheel.Instance.background.GetComponent<UICircle>().sprite;
        circleImage = OptionsMenuToManager.Instance.transform.Find("Crosshair Filler").GetChild(0).GetChild(6).GetChild(0).GetComponent<Image>().sprite;
        colorBlock = OptionsMenuToManager.Instance.pauseMenu.transform.Find("Continue").GetComponent<Button>().colors;
    }

    #region base

    public static void Component<T>(GameObject obj, UnityAction<T> action) where T : Component
    {
        action.Invoke(obj.AddComponent<T>());
    }

    public static GameObject Object(string name, Transform parent)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent);

        return obj;
    }

    public static GameObject Rect(string name, Transform parent, float x, float y, float width, float height)
    {
        var obj = Object(name, parent);
        Component<RectTransform>(obj, rect =>
        {
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        });

        return obj;
    }

    public static GameObject Image(string name, Transform parent, float x, float y, float width, float height, Color? color = null, bool circle = false)
    {
        var obj = Rect(name, parent, x, y, width, height);
        Component<Image>(obj, image =>
        {
            image.sprite = circle ? circleImage : buttonImage;
            image.type = UnityEngine.UI.Image.Type.Sliced;
            image.color = color.HasValue ? color.Value : new Color(0f, 0f, 0f, .5f);
        });

        return obj;
    }

    public static GameObject Circle(string name, Transform parent, float x, float y, float width, float height, float arc, int rotation, float thickness, bool outline)
    {
        var obj = Rect(name, parent, x, y, width, height);
        Component<UICircle>(obj, circle =>
        {
            circle.Arc = arc;
            circle.ArcRotation = rotation;
            circle.Thickness = thickness;
            circle.Fill = false;
        });

        if (outline) Component<Outline>(obj, outline =>
        {
            outline.effectDistance = new(3f, -3f);
            outline.effectColor = Color.white;
        });

        return obj;
    }

    public static GameObject Canvas(string name, Transform parent)
    {
        var obj = Object(name, parent);
        Component<Canvas>(obj, canvas =>
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // move to the top (ultraimportant)
        });
        Component<CanvasScaler>(obj, scaler =>
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
        });
        obj.AddComponent<GraphicRaycaster>();

        return obj;
    }

    public static GameObject Canvas(string name, Transform parent, float width, float height, Vector3 position)
    {
        var obj = Rect(name, parent, 0f, 0f, width, height);
        Component<Canvas>(obj, canvas => canvas.renderMode = RenderMode.WorldSpace);
        obj.transform.position = position;

        return obj;
    }

    #endregion
    #region text

    public static GameObject Text(string name, Transform parent, float x, float y, float width, float height, int size, Color? color = null, TextAnchor align = TextAnchor.MiddleCenter)
    {
        var obj = Rect(name, parent, x, y, width, height);
        Component<Text>(obj, text =>
        {
            text.text = name;
            text.font = DollAssets.Font;
            text.fontSize = size;
            text.color = color.HasValue ? color.Value : Color.white;
            text.alignment = align;
        });

        return obj;
    }

    public static GameObject Text(string name, Transform parent, float x, float y)
    {
        return Text(name, parent, x, y, 320f, 64f, 36);
    }

    #endregion
    #region button

    public static GameObject Button(string name, Transform parent, float x, float y, float width, float height, int size, Color color, TextAnchor align, UnityAction clicked)
    {
        var obj = Image(name, parent, x, y, width, height, Color.white);
        Component<Button>(obj, button =>
        {
            button.targetGraphic = obj.GetComponent<Image>();
            button.colors = colorBlock;
            button.onClick.AddListener(clicked);
        });

        // add text to the button
        Text(name, obj.transform, 0f, 0f, width, height, size, color, align);

        return obj;
    }

    public static GameObject Button(string name, Transform parent, float x, float y, UnityAction clicked, int size = 36)
    {
        return Button(name, parent, x, y, 320f, 64f, size, Color.white, TextAnchor.MiddleCenter, clicked);
    }

    public static void SetText(GameObject obj, string text)
    {
        obj.GetComponentInChildren<Text>().text = text;
    }

    public static void SetInteractable(GameObject obj, bool interactable)
    {
        obj.GetComponent<Button>().interactable = interactable;
    }

    #endregion
    #region team button

    public static GameObject TeamButton(Team team, Transform parent, float x, float y, float width, float height, UnityAction clicked)
    {
        var obj = Image(team.ToString(), parent, x, y, width, height, team.Data().Color());
        Component<Button>(obj, button =>
        {
            button.targetGraphic = obj.GetComponent<Image>();
            button.onClick.AddListener(clicked);
        });

        if (team == Team.Pink) Text("UwU", obj.transform, 0f, 0f, width, height, 24);

        return obj;
    }

    public static GameObject TeamButton(Team team, Transform parent, float x, float y, UnityAction clicked)
    {
        return TeamButton(team, parent, x, y, 51f, 51f, clicked);
    }

    #endregion
    #region field

    public static InputField Field(string name, Transform parent, float x, float y, float width, float height, int size, UnityAction<string> enter)
    {
        var obj = Image(name, parent, x, y, width, height);

        var text = Text("", obj.transform, 8f, 1f, width, height, size, align: TextAnchor.MiddleLeft);
        var placeholder = Text(name, obj.transform, 8f, 1f, width, height, size, new Color(.8f, .8f, .8f, .8f), TextAnchor.MiddleLeft);

        Component<InputField>(obj, field =>
        {
            field.targetGraphic = obj.GetComponent<Image>();
            field.textComponent = text.GetComponent<Text>();
            field.placeholder = placeholder.GetComponent<Text>();

            field.onEndEdit.AddListener(enter);
        });

        return obj.GetComponent<InputField>();
    }

    #endregion
    #region shadow

    public static GameObject Shadow(string name, Transform parent, float x, float y, float width, float height)
    {
        var obj = Rect(name, parent, x, y, width, height);
        Component<Image>(obj, image =>
        {
            image.sprite = shadowImage;
            image.color = Color.black;
        });

        return obj;
    }

    public static GameObject Shadow(string name, Transform parent, float x, float y)
    {
        return Shadow(name, parent, x, y, 320f, 2000f);
    }

    public static GameObject CircleShadow(string name, Transform parent, float x, float y, float width, float height, float thickness)
    {
        var obj = Rect(name, parent, x, y, width, height);
        Component<UICircle>(obj, circle =>
        {
            circle.sprite = circleShadowImage;
            circle.color = Color.black;

            circle.Fill = false;
            circle.Thickness = thickness;
        });

        return obj;
    }

    #endregion
}
