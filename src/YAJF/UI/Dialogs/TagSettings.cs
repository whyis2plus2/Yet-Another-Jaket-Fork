namespace YAJF.UI.Dialogs;

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;

using Jaket.UI;
using Jaket.World;
using static Jaket.UI.Rect;
using static Jaket.UI.Pal;
using System;

public class TagSettings: CanvasSingleton<TagSettings>
{
    InputField field;
    Slider Red, Green, Blue;
    byte r = 255, g = 255, b = 255;

    private void Start()
    {
        if (Prefs.tagColor != null)
        {
            r = Convert.ToByte("0x" + Prefs.tagColor.Substring(1, 2), 16);
            g = Convert.ToByte("0x" + Prefs.tagColor.Substring(3, 2), 16);
            b = Convert.ToByte("0x" + Prefs.tagColor.Substring(5, 2), 16);
        }

        UIB.Table("Content", "---TAG SETTINGS---", transform, Size(640, 356), table =>
        {
            UIB.IconButton("X", table, Icon(292f, 24f), red, clicked: Toggle);

            field = UIB.Field("TAG", table, new(320f, -68f, 350f, 40f, new(0f, 1f)), cons: text =>
            {
                Prefs.tagColor = $"#{r:X}{g:X}{b:X}";
                Prefs.tag = Regex.Replace(text.Trim(), "<*.?>", "").Replace("[", "\\[");
            });

            field.characterLimit = Prefs.MAX_TAG_LEN - 9 /* length of color and terminator combined */;

            UIB.Text("Red", table, Btn(108), red, align: TextAnchor.MiddleLeft);
            var RText = UIB.Text("0", table, Btn(108), red, align: TextAnchor.MiddleRight);
            Red = UIB.Slider("Red", table, Sld(138) with {Width = 350}, 255, value =>
            {
                r = (byte)value;
                RText.text = $"{value/255f:n2}";
                Rebuild();
            });

            UIB.Text("Green", table, Btn(158), green, align: TextAnchor.MiddleLeft);
            var GText = UIB.Text("0", table, Btn(158), green, align: TextAnchor.MiddleRight);
            Green = UIB.Slider("Green", table, Sld(188) with {Width = 350}, 255, value =>
            {
                g = (byte)value;
                GText.text = $"{value/255f:n2}";
                Rebuild();
            });

            UIB.Text("Blue", table, Btn(208), blue, align: TextAnchor.MiddleLeft);
            var BText = UIB.Text("0", table, Btn(208), blue, align: TextAnchor.MiddleRight);
            Blue = UIB.Slider("Blue", table, Sld(238) with {Width = 350}, 255, value =>
            {
                b = (byte)value;
                BText.text = $"{value/255f:n2}";
                Rebuild();
            });

            UIB.IconButton("CONFIRM", table, Btn(300), green, clicked: () =>
            {
                Prefs.tagColor = $"#{r:X}{g:X}{b:X}";
                Prefs.tag = Regex.Replace(field.text.Trim(), "<*.?>", "").Replace("[", "\\[");
                HudMessageReceiver.Instance?.SendHudMessage($"Succesfully Set Tag to \"<color={Prefs.tagColor}>{Prefs.tag}</color>\"");
                Toggle();
            });
        });

        Rebuild();
    }

    private void Rebuild()
    {
        field.textComponent.color = new Color32(r, g, b, 0xFF);
        field.text = Prefs.tag;

        if (r != 0) Red.value = r;
        if (g != 0) Green.value = g;
        if (b != 0) Blue.value = b;
    }

    public void Toggle()
    {
        if (!Shown) UI.HideCentralGroup();

        gameObject.SetActive(Shown = !Shown);
        Movement.UpdateState();

        if (Shown && transform.childCount > 0) Rebuild();
    }
}